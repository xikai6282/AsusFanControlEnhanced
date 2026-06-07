using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using AsusFanControl;
using Microsoft.Win32;
using LibreHardwareMonitor.Hardware;
using System.Management;
using AsusFanControl.Domain.services;
using AsusFanControlGUI.Localization;
using AsusFanControlGUI.Theming;

namespace AsusFanControlGUI
{
    public partial class Form1 : Form
    {
        // 渚濊禆娉ㄥ叆
        private readonly FanCurve _fanCurve;

        private readonly Random rnd = new Random();
        readonly AsusControl asusControl = new AsusControl();
        int currentFanRPM = 0;
        ulong currentTemp = 0;

        int temperatureLowerBound = 20;
        int temperatureUpperBound = 105;

        int fanSpeedLowerBound = 1;
        int fanSpeedUpperBound = 100;

        public Form1(FanCurve fanCurve)
        {
            _fanCurve = fanCurve;

            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            // 鍒濆鍖栨湰鍦板寲鍜屼富棰?            LocalizationManager.Initialize();
            ThemeManager.Initialize();
            // 娣诲姞璇█/涓婚鑿滃崟锛堢敱 Form1.Menus.cs 閮ㄥ垎绫诲疄鐜帮級
            AddLocalizationAndThemeMenus();
            SubscribeToLocalizationAndThemeEvents();
            // 鍒濇搴旂敤涓婚
            OnLanguageOrThemeChanged();

            init();
        }

        private async void init()
        {
            if (IsHandleCreated)
            {
                // Load settings from the settings file
                toolStripMenuItemForbidUnsafeSettings.Checked = Properties.Settings.Default.forbidUnsafeSettings;
                startMinimisedToolStripMenuItem.Checked = Properties.Settings.Default.startMinimised;
                checkIfAppIsInStartup();
                trackBarFanSpeed.Value = Properties.Settings.Default.fanSpeed;
                radioButton1.Checked = Properties.Settings.Default.fanControlState == "Off";
                fanControlRadioButton.Checked = Properties.Settings.Default.fanControlState == "Manual";
                fanCurveRadioButton.Checked = Properties.Settings.Default.fanControlState == "Curve";
                allowFanCurveSettingViaTextToolStripMenuItem.Checked = Properties.Settings.Default.allowFanCurveSettingViaText;
                numericUpDown1.Value = Properties.Settings.Default.hysteresis;
                numericUpDown2.Value = Properties.Settings.Default.updateSpeed;

                // 鍒濆鍖栨湰鍦板寲鏂囨湰
                OnLanguageChanged();
                // 鍒濆鍖栦富棰?                OnThemeChanged(ThemeManager.CurrentTheme);

                // Manually trigger events
                radioButton1_CheckedChanged(radioButton1, EventArgs.Empty);
                fanCurve_CheckedChanged(fanCurveRadioButton, EventArgs.Empty);
                fanControl_CheckedChanged(fanControlRadioButton, EventArgs.Empty);
                allowFanCurveSettingViaTextToolStripMenuItem_Click(allowFanCurveSettingViaTextToolStripMenuItem, EventArgs.Empty);

                Properties.Settings.Default.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "FanCurvePoints")
                    {
                        CurvePointsTextbox.Text = Properties.Settings.Default.FanCurvePoints;
                    }
                };
                SetFanCurvePoints(Properties.Settings.Default.FanCurvePoints);
                AutoRefreshStats();
                startErrorHandler(temperatureLowerBound, temperatureUpperBound);
            }
            else
            {
                await Task.Delay(20);
                init();
            }
        }

        public void checkForNewVersion()
        {
            // Check for new version via GitHub releases
        }

        private async void startErrorHandler(int minTemp, int maxTemp, bool debug = false)
        {
            while (true)
            {
                await Task.Delay(3000);

                Console.WriteLine($"Current temp: {currentTemp}");
                if ((currentTemp < (ulong)minTemp || currentTemp > (ulong)maxTemp) || debug)
                {
                    Console.WriteLine("Error!");

                    Properties.Settings.Default.wasError = true;
                    string errorMsg = LocalizationManager.GetLong("Error.TempOutOfRange", currentTemp);
                    Properties.Settings.Default.errorMsg = errorMsg;
                    Properties.Settings.Default.Save();

                    Console.WriteLine(errorMsg);
                    InvisibleLabel.Text = errorMsg;
                    InvisibleLabel.Visible = true;
                }
                else
                {
                    InvisibleLabel.Text = "";
                    InvisibleLabel.Visible = false;
                }
            }
        }

        public async Task updateFanRPMLabel()
        {
            var speeds = await Task.Run(() => asusControl.GetFanSpeeds());
            Console.WriteLine($"Fan speeds: {string.Join(" ", speeds)}");
            labelRPM.Text = string.Join(" ", speeds);
        }

        public async Task updateCPUTempLabel()
        {
            var temp = await Task.Run(() => asusControl.Thermal_Read_Cpu_Temperature());
            Console.WriteLine($"CPU temp: {temp}");
            currentTemp = temp;
            labelCPUTemp.Text = $"{temp}掳C";

            // 娓╁害棰滆壊缂栫爜
            if (temp < 50)
                labelCPUTemp.ForeColor = ThemeManager.CurrentTheme == AppTheme.Dark
                    ? ThemeManager.DarkPalette.StatusGood
                    : ThemeManager.LightPalette.StatusGood;
            else if (temp < 75)
                labelCPUTemp.ForeColor = ThemeManager.CurrentTheme == AppTheme.Dark
                    ? ThemeManager.DarkPalette.StatusWarn
                    : ThemeManager.LightPalette.StatusWarn;
            else
                labelCPUTemp.ForeColor = ThemeManager.CurrentTheme == AppTheme.Dark
                    ? ThemeManager.DarkPalette.StatusDanger
                    : ThemeManager.LightPalette.StatusDanger;
        }

        private async void AutoRefreshStats()
        {
            while (true)
            {
                if (WindowState == FormWindowState.Minimized)
                    await Task.Delay(1000);
                else
                    await Task.Delay(500);

                Console.WriteLine($"Refreshing stats");

                await Task.WhenAll(updateFanRPMLabel(), updateCPUTempLabel());
            }
        }

        private void checkIfAppIsInStartup()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            startWithWindowsToolStripMenuItem.Checked = registryKey.GetValue(appName) != null;

            if (startWithWindowsToolStripMenuItem.Checked)
            {
                string appPath = Assembly.GetExecutingAssembly().Location;
                string registryValue = registryKey.GetValue(appName).ToString();
                if (registryValue != appPath)
                {
                    Console.WriteLine("Updating startup entry");
                    registryKey.SetValue(appName, appPath);
                }
            }
        }

        private void AddToStartup()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;
            string appPath = Assembly.GetExecutingAssembly().Location;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue(appName, appPath);
        }

        private void RemoveFromStartup()
        {
            string appName = Assembly.GetExecutingAssembly().GetName().Name;

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.DeleteValue(appName, false);
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            // if (Properties.Settings.Default.turnOffControlOnExit)
            //     asusControl.SetFanSpeeds(0);
        }

        private void toolStripMenuItemForbidUnsafeSettings_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.forbidUnsafeSettings =
                toolStripMenuItemForbidUnsafeSettings.Checked;
            Properties.Settings.Default.Save();
        }

        private void toolStripMenuItemCheckForUpdates_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(
                "https://github.com/Darren80/AsusFanControlEnhanced/releases");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            MessageBox.Show(
                LocalizationManager.GetLong("Dialog.About.Text", version),
                LocalizationManager.GetLong("Dialog.About.Title"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void setNotifyIconText(string text)
        {
            Console.WriteLine($"Setting notify icon text to: {text}");
            notifyIcon1.Text = text;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Properties.Settings.Default.fanControlState = "Off";
                Properties.Settings.Default.Save();

                setNotifyIconText(LocalizationManager.Get("Tray.Off"));
                setFanSpeed(0, null);
            }
        }

        private async void fanControl_CheckedChanged(object sender, EventArgs e)
        {
            if (fanControlRadioButton.Checked)
            {
                Properties.Settings.Default.fanControlState = "Manual";
                Properties.Settings.Default.Save();
                trackBarFanSpeed.Enabled = true;

                trackBarSetFanSpeed();
                await Task.Delay(2000);
                fanControl_CheckedChanged(sender, e);
            }
            else
            {
                trackBarFanSpeed.Enabled = false;
            }
        }

        bool firstRun = true;
        private async void setFanSpeed(int value, bool? xyz)
        {
            if (currentFanRPM == value)
                return;

            await Task.Run(() => asusControl.SetFanSpeeds(value));
            currentFanRPM = value;

            if (value == 0)
                labelValue.Text = LocalizationManager.Get("Fan.TurnedOff");
            else
                labelValue.Text = value.ToString() + LocalizationManager.Get("Fan.PWM");

            if (firstRun)
            {
                await Task.Delay(1000);
                currentFanRPM = 999999;
                setFanSpeed(value, null);
                firstRun = false;
            }
        }

        private void trackBarSetFanSpeed()
        {
            if (Properties.Settings.Default.forbidUnsafeSettings)
            {
                if (trackBarFanSpeed.Value < 40)
                    trackBarFanSpeed.Value = 40;
                else if (trackBarFanSpeed.Value > 99)
                    trackBarFanSpeed.Value = 99;
            }

            Properties.Settings.Default.fanSpeed = trackBarFanSpeed.Value;
            Properties.Settings.Default.Save();

            Decimal trackBarFanSpeedValue = trackBarFanSpeed.Value;
            label5.Text = trackBarFanSpeedValue.ToString() + LocalizationManager.Get("Fan.PercentLabel");
            Console.WriteLine($"Setting speed to: {(int)trackBarFanSpeedValue}");

            UpdateStatusLabel();

            setNotifyIconText(LocalizationManager.Get("Tray.Manual",
                (int)trackBarFanSpeedValue, currentTemp));

            if ((int)trackBarFanSpeedValue == 0)
                setNotifyIconText(LocalizationManager.Get("Tray.Off"));

            setFanSpeed((int)trackBarFanSpeedValue, fanControlRadioButton.Checked);
        }

        private void UpdateStatusLabel()
        {
            if (fanCurveRadioButton.Checked)
            {
                double fs = CalculateFanSpeed(currentTemp);
                label3.Text = LocalizationManager.Get("Status.CurveSpeed", (int)fs, currentTemp);
            }
            else if (fanControlRadioButton.Checked)
            {
                label3.Text = LocalizationManager.Get("Status.SettingSpeed",
                    trackBarFanSpeed.Value);
            }
            else
            {
                label3.Text = "";
            }
        }

        private void trackBarFanSpeed_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                return;

            trackBarSetFanSpeed();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            List<int> fanSpeed = await Task.Run(() => asusControl.GetFanSpeeds());
            labelRPM.Text = string.Join(" ", fanSpeed);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            ulong temp = await Task.Run(() => asusControl.Thermal_Read_Cpu_Temperature());
            currentTemp = temp;
            labelCPUTemp.Text = $"{temp}掳C";
            startErrorHandler(temperatureLowerBound, temperatureUpperBound);
        }

        private Dictionary<int, Point> fanCurvePoints = new Dictionary<int, Point>()
        {
            { 1, new Point(20, 1) },
            { 4, new Point(60, 1) },
            { 5, new Point(61, 20) },
            { 7, new Point(70, 20) },
            { 8, new Point(71, 30) },
            { 9, new Point(80, 55) },
        };

        private void pictureBoxFanCurve_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int padding = 50;
            int graphWidth = pictureBoxFanCurve.Width - padding - 20;
            int graphHeight = pictureBoxFanCurve.Height - padding - 30;

            int originX = padding;
            int originY = pictureBoxFanCurve.Height - padding;
            int endX = padding + graphWidth;
            int endY = padding;

            // 鈹€鈹€ 鑳屾櫙濉厖 鈹€鈹€
            using (Brush bgBrush = new SolidBrush(ThemeManager.GraphBg))
                g.FillRectangle(bgBrush, 0, 0, pictureBoxFanCurve.Width, pictureBoxFanCurve.Height);

            // 鈹€鈹€ 鍥捐〃鍖哄煙寰濉厖 鈹€鈹€
            using (Brush chartBg = new SolidBrush(
                ThemeManager.CurrentTheme == AppTheme.Dark
                    ? Color.FromArgb(34, 34, 40)
                    : Color.FromArgb(250, 250, 252)))
            {
                g.FillRectangle(chartBg, originX, endY, graphWidth, graphHeight);
            }

            // 鈹€鈹€ 缃戞牸绾?鈹€鈹€
            using (Pen gridPen = new Pen(ThemeManager.GraphGrid, 0.5f))
            {
                gridPen.DashStyle = DashStyle.Dot;

                // 鍨傜洿缃戞牸绾?(娓╁害)
                for (int temp = 20; temp <= 100; temp += 10)
                {
                    int x = originX + (temp - 20) * graphWidth / 80;
                    g.DrawLine(gridPen, x, endY, x, originY);
                }

                // 姘村钩缃戞牸绾?(椋庢墖閫熷害)
                for (int speed = 0; speed <= 100; speed += 20)
                {
                    int y = originY - speed * graphHeight / 100;
                    g.DrawLine(gridPen, originX, y, endX, y);
                }
            }

            // 鈹€鈹€ 鍧愭爣杞?鈹€鈹€
            using (Pen axisPen = new Pen(ThemeManager.CtrlFore, 1.5f))
            {
                // X 杞?                g.DrawLine(axisPen, originX, originY, endX, originY);
                // Y 杞?                g.DrawLine(axisPen, originX, originY, originX, endY);
            }

            // 鈹€鈹€ X 杞村埢搴︽爣绛?鈹€鈹€
            using (Brush labelBrush = new SolidBrush(ThemeManager.CtrlFore))
            using (Font labelFont = new Font("Segoe UI", 7.5f))
            {
                for (int temp = 20; temp <= 100; temp += 20)
                {
                    int x = originX + (temp - 20) * graphWidth / 80;
                    g.DrawLine(Pens.Gray, x, originY - 4, x, originY + 4);
                    string label = temp.ToString();
                    SizeF size = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, labelBrush, x - size.Width / 2, originY + 6);
                }

                // X 杞存爣棰?                string xTitle = LocalizationManager.Get("Fan.Temperature");
                SizeF xSize = g.MeasureString(xTitle, labelFont);
                g.DrawString(xTitle, labelFont, labelBrush,
                    originX + graphWidth / 2 - xSize.Width / 2,
                    originY + 22);

                // Y 杞存爣棰橈紙鍨傜洿锛?                string yTitle = LocalizationManager.Get("Fan.FanSpeed");
                StringFormat sf = new StringFormat(StringFormatFlags.DirectionVertical);
                SizeF ySize = g.MeasureString(yTitle, labelFont, 100, sf);
                g.DrawString(yTitle, labelFont, labelBrush,
                    6f, originY - graphHeight / 2 - ySize.Height / 2, sf);

                // Y 杞村埢搴︽爣绛?                for (int speed = 0; speed <= 100; speed += 20)
                {
                    int y = originY - speed * graphHeight / 100;
                    g.DrawLine(Pens.Gray, originX - 4, y, originX + 4, y);
                    string label = speed.ToString();
                    SizeF size = g.MeasureString(label, labelFont);
                    g.DrawString(label, labelFont, labelBrush,
                        originX - size.Width - 8, y - size.Height / 2);
                }
            }

            // 鈹€鈹€ 缁樺埗鏇茬嚎 鈹€鈹€
            if (fanCurvePoints.Count >= 2)
            {
                Point[] graphPoints = fanCurvePoints.Values
                    .OrderBy(p => p.X)
                    .Select(p => new Point(
                        originX + (p.X - 20) * graphWidth / 80,
                        originY - p.Y * graphHeight / 100))
                    .ToArray();

                // 娓愬彉濉厖鏇茬嚎涓嬫柟鍖哄煙
                using (GraphicsPath fillPath = new GraphicsPath())
                {
                    fillPath.AddLines(graphPoints);
                    fillPath.AddLine(graphPoints.Last().X, graphPoints.Last().Y,
                        graphPoints.Last().X, originY);
                    fillPath.AddLine(graphPoints.Last().X, originY,
                        graphPoints.First().X, originY);
                    fillPath.CloseFigure();

                    using (PathGradientBrush gradientBrush = new PathGradientBrush(fillPath))
                    {
                        gradientBrush.CenterPoint = new PointF(
                            originX + graphWidth / 2, originY - graphHeight / 2);
                        gradientBrush.CenterColor = Color.FromArgb(18,
                            ThemeManager.GraphLine.R, ThemeManager.GraphLine.G,
                            ThemeManager.GraphLine.B);
                        gradientBrush.SurroundColors = new Color[]
                        {
                            Color.FromArgb(2, ThemeManager.GraphLine.R,
                                ThemeManager.GraphLine.G, ThemeManager.GraphLine.B)
                        };
                        g.FillPath(gradientBrush, fillPath);
                    }
                }

                // 鏇茬嚎绾挎潯
                using (Pen curvePen = new Pen(ThemeManager.GraphLine, 2.5f))
                {
                    curvePen.LineJoin = LineJoin.Round;
                    curvePen.StartCap = LineCap.Round;
                    curvePen.EndCap = LineCap.Round;
                    g.DrawLines(curvePen, graphPoints);
                }
            }

            // 鈹€鈹€ 缁樺埗鎺у埗鐐?鈹€鈹€
            foreach (Point point in fanCurvePoints.Values)
            {
                int x = originX + (point.X - 20) * graphWidth / 80;
                int y = originY - point.Y * graphHeight / 100;

                // 澶栧湀鍏夋檿
                using (Brush glowBrush = new SolidBrush(Color.FromArgb(40,
                    ThemeManager.GraphPt.R, ThemeManager.GraphPt.G,
                    ThemeManager.GraphPt.B)))
                {
                    g.FillEllipse(glowBrush, x - 7, y - 7, 14, 14);
                }

                // 瀹炲績鐐?                bool isSelected = selectedPointId != 0 && fanCurvePoints.ContainsKey(selectedPointId) && fanCurvePoints[selectedPointId] == point;

                Color pointColor = isSelected ? ThemeManager.GraphPtDr : ThemeManager.GraphPt;
                using (Brush ptBrush = new SolidBrush(pointColor))
                    g.FillEllipse(ptBrush, x - 4, y - 4, 8, 8);

                // 鐧借竟
                using (Pen edgePen = new Pen(Color.White, 1.5f))
                    g.DrawEllipse(edgePen, x - 4, y - 4, 8, 8);
            }
        }

        private void pictureBoxFanCurve_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            (int temperature, int fanSpeed) =
                convertMousePositionToTemperatureAndFanSpeed(e.Location);

            if (temperature < temperatureLowerBound || temperature > temperatureUpperBound)
                temperature = Math.Max(temperatureLowerBound,
                    Math.Min(temperature, temperatureUpperBound));

            if (fanSpeed < fanSpeedLowerBound || fanSpeed > fanSpeedUpperBound)
                fanSpeed = Math.Max(fanSpeedLowerBound,
                    Math.Min(fanSpeed, fanSpeedUpperBound));

            foreach (KeyValuePair<int, Point> point in fanCurvePoints)
            {
                if (point.Value.X == temperature)
                {
                    fanCurvePoints[point.Key] = new Point(temperature, fanSpeed);
                    pictureBoxFanCurve.Invalidate();
                    return;
                }
            }

            if (fanCurvePoints.Count >= 20)
            {
                MessageBox.Show(LocalizationManager.Get("Curve.MaxPointsReached"));
                return;
            }

            int newID = fanCurvePoints.Count > 0 ? fanCurvePoints.Keys.Max() + 1 : 1;
            fanCurvePoints[newID] = new Point(temperature, fanSpeed);
            pictureBoxFanCurve.Invalidate();
            runFanCurve(true, true);
        }

        private int selectedPointId = 0;

        private (int temperature, int fanSpeed) convertMousePositionToTemperatureAndFanSpeed(Point e)
        {
            int padding = 50;
            int graphWidth = pictureBoxFanCurve.Width - padding - 20;
            int graphHeight = pictureBoxFanCurve.Height - padding - 30;
            int tempMin = 20;
            int tempMax = 100;
            int speedMax = 100;

            int temperature = tempMin + (e.X - padding) * (tempMax - tempMin) / graphWidth;
            int fanSpeed = speedMax - (e.Y - padding) * speedMax / graphHeight;

            return (temperature, fanSpeed);
        }

        private KeyValuePair<int, Point> nearestPointToMouse(MouseEventArgs e, int maxDistance)
        {
            (int temperature, int fanSpeed) =
                convertMousePositionToTemperatureAndFanSpeed(e.Location);

            var nearestPoints = fanCurvePoints
                .OrderBy(p => Distance(p.Value, new Point(temperature, fanSpeed)));

            return nearestPoints.FirstOrDefault(p =>
                Distance(p.Value, new Point(temperature, fanSpeed)) <= maxDistance);
        }

        private int maxDistance = 8;

        private void pictureBoxFanCurve_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                KeyValuePair<int, Point> reachablePoint = nearestPointToMouse(e, maxDistance);
                if (reachablePoint.Value != Point.Empty)
                {
                    fanCurvePoints.Remove(reachablePoint.Key);
                    pictureBoxFanCurve.Invalidate();
                    Console.WriteLine($"Point ID: {reachablePoint.Key} deleted.");
                }
            }
        }

        private void pictureBoxFanCurve_MouseDown(object sender, MouseEventArgs e)
        {
            Console.Write("MouseDown: ");

            if (e.Button == MouseButtons.Left)
            {
                KeyValuePair<int, Point> reachablePoint = nearestPointToMouse(e, maxDistance);
                if (reachablePoint.Value != Point.Empty)
                {
                    selectedPointId = reachablePoint.Key;
                }
                else
                {
                    selectedPointId = 0;
                }
            }
        }

        private void pictureBoxFanCurve_MouseMove(object sender, MouseEventArgs e)
        {
            byte minimumTemperature = 20;
            byte maximumTemperature = 105;
            byte minFanSpeed = 1;
            byte maxFanSpeed = 100;

            if (selectedPointId != 0)
            {
                (int temperature, int fanSpeed) =
                    convertMousePositionToTemperatureAndFanSpeed(e.Location);

                if (temperature < minimumTemperature || temperature > maximumTemperature)
                    temperature = Math.Max(minimumTemperature,
                        Math.Min(temperature, maximumTemperature));

                if (fanSpeed < minFanSpeed || fanSpeed > maxFanSpeed)
                    fanSpeed = Math.Max(minFanSpeed, Math.Min(fanSpeed, maxFanSpeed));

                fanCurvePoints[selectedPointId] = new Point(temperature, fanSpeed);

                toolTip1.SetToolTip(pictureBoxFanCurve,
                    LocalizationManager.Get("Curve.PointTooltip", temperature, fanSpeed));

                pictureBoxFanCurve.Invalidate();
            }
        }

        private void pictureBoxFanCurve_MouseUp(object sender, MouseEventArgs e)
        {
            selectedPointId = 0;
            toolTip1.SetToolTip(pictureBoxFanCurve,
                LocalizationManager.Get("Curve.GraphTooltip"));
            SaveFanCurvePoints(fanCurvePoints);
            runFanCurve(true, true);
        }

        private double Distance(Point p1, Point p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private void fanCurve_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.fanControlState = "Curve";
            Properties.Settings.Default.Save();
            Console.WriteLine(fanCurveRadioButton.Checked);
            runFanCurve();
        }

        public enum CurveType { Linear, Quadratic, Cubic }

        double curvatureFactor = 0.5;

        double CalculateFanSpeed(double currentTemp)
        {
            KeyValuePair<int, Point> lowerPoint = fanCurvePoints
                .OrderByDescending(p => p.Value.X)
                .FirstOrDefault(p => (ulong)p.Value.X <= currentTemp);
            KeyValuePair<int, Point> upperPoint = fanCurvePoints
                .OrderBy(p => p.Value.X)
                .FirstOrDefault(p => (ulong)p.Value.X >= currentTemp);

            if (lowerPoint.Key == upperPoint.Key)
                return lowerPoint.Value.Y;

            if (lowerPoint.Key == 0 || upperPoint.Key == 0)
            {
                label3.Text = LocalizationManager.Get("Status.YieldedToSystem");
                Console.WriteLine("Temperature is outside the range, yield control to the system.");
                setNotifyIconText(LocalizationManager.Get("Tray.Off"));
                return 0;
            }

            double ratio = (currentTemp - lowerPoint.Value.X)
                / (upperPoint.Value.X - lowerPoint.Value.X);
            return lowerPoint.Value.Y
                + (upperPoint.Value.Y - lowerPoint.Value.Y) * ratio;
        }

        private async void runFanCurve(bool bypassHysteresisCheck = false,
            bool runOnce = false)
        {
            if (!fanCurveRadioButton.Checked)
            {
                label3.Text = $"";
                return;
            }

            double fanSpeed = CalculateFanSpeed(currentTemp);

            int hysteresis = (int)numericUpDown1.Value;
            if ((int)currentTemp > lastTemperature + hysteresis
                || (int)currentTemp < lastTemperature - hysteresis
                || fanSpeed < 10 || bypassHysteresisCheck)
            {
                fanSpeed = Math.Max(0, Math.Min(100, fanSpeed));
                setFanSpeed((int)fanSpeed, true);

                Console.WriteLine($"Set fan speed to {(int)fanSpeed}% {rnd.Next(1000)}");

                if (fanSpeed != 0)
                {
                    label3.Text = LocalizationManager.Get("Status.CurveSpeed",
                        (int)fanSpeed, currentTemp);
                    setNotifyIconText(LocalizationManager.Get("Tray.Manual",
                        (int)fanSpeed, currentTemp));
                }
                lastTemperature = (int)currentTemp;
            }

            if (!runOnce)
            {
                await Task.Delay((int)numericUpDown2.Value);
                runFanCurve();
            }
        }

        private int lastTemperature = 0;

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                MinimizeToTray();
        }

        public void MinimizeToTray()
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            mayShowError();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
                mayShowError();
            }
        }

        private void SaveFanCurvePoints(Dictionary<int, Point> _fanCurvePoints)
        {
            fanCurvePoints = _fanCurvePoints;

            string fanCurvePointsString = string.Join("-",
                _fanCurvePoints.OrderBy(x => x.Value.X)
                    .Select(x => $"{x.Value.X},{x.Value.Y}"));

            Properties.Settings.Default.FanCurvePoints = fanCurvePointsString;
            Properties.Settings.Default.Save();

            Console.WriteLine(fanCurvePointsString);
        }

        private void SetFanCurvePoints(String fanCurvePointsString)
        {
            Dictionary<int, Point> fanCurvePointsDictionaryFromString;
            try
            {
                fanCurvePointsDictionaryFromString = _fanCurve.convertStringToPointsDictionary(
                    fanCurvePointsString, temperatureLowerBound, temperatureUpperBound,
                    fanSpeedLowerBound, fanSpeedUpperBound);
            }
            catch (Exception ex)
            {
                throw;
            }

            SaveFanCurvePoints(fanCurvePointsDictionaryFromString);
            pictureBoxFanCurve.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SetFanCurvePoints(Properties.Settings.Default.DefaultFanCurvePoints);
                CurvePointsTextbox.Text = Properties.Settings.Default.DefaultFanCurvePoints;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SetFanCurvePoints(CurvePointsTextbox.Text);
                MessageBox.Show(LocalizationManager.Get("Dialog.SaveSuccess"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trackBarFanSpeed_MouseUp(object sender, MouseEventArgs e)
        {
            trackBarSetFanSpeed();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            try
            {
                SetFanCurvePoints(CurvePointsTextbox.Text);
                MessageBox.Show(LocalizationManager.Get("Dialog.SaveSuccess"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void trackBarFanSpeed_ValueChanged(object sender, EventArgs e)
        {
            trackBarSetFanSpeed();
        }

        private void trackBarFanSpeed_MouseMove(object sender, MouseEventArgs e)
        {
            // Reserved
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.hysteresis = (int)numericUpDown1.Value;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.updateSpeed = (int)numericUpDown2.Value;
            Properties.Settings.Default.Save();
        }

        private void mayShowError()
        {
            if (Properties.Settings.Default.wasError)
            {
                Properties.Settings.Default.wasError = false;
                Properties.Settings.Default.Save();

                MessageBox.Show(Properties.Settings.Default.errorMsg);
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (startMinimisedToolStripMenuItem.Checked)
                MinimizeToTray();

            mayShowError();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                MinimizeToTray();
            }
        }

        private void contextMenuStrip1_Opening(object sender,
            System.ComponentModel.CancelEventArgs e)
        {
            // Reserved
        }

        private void InvisibleLabel_Click(object sender, EventArgs e)
        {
            // Reserved
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Already handled via dropdown
        }

        private void allowFanCurveSettingViaTextToolStripMenuItem_Click(
            object sender, EventArgs e)
        {
            if (allowFanCurveSettingViaTextToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.allowFanCurveSettingViaText = true;
                Properties.Settings.Default.Save();
                button3.Enabled = true;
                CurvePointsTextbox.ReadOnly = false;
            }
            else
            {
                Properties.Settings.Default.allowFanCurveSettingViaText = false;
                Properties.Settings.Default.Save();
                button3.Enabled = false;
                CurvePointsTextbox.ReadOnly = true;
            }
        }

        private void textBox1_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(CurvePointsTextbox.Text, CurvePointsTextbox);
        }

        private void startWithWindowsToolStripMenuItem1_Click_1(object sender, EventArgs e)
        {
            if (startWithWindowsToolStripMenuItem.Checked)
            {
                Properties.Settings.Default.startWithWindows = true;
                Properties.Settings.Default.Save();
                RemoveFromStartup();
                AddToStartup();
            }
            else
            {
                Properties.Settings.Default.startWithWindows = false;
                Properties.Settings.Default.Save();
                RemoveFromStartup();
            }
        }

        private void startMinimisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.startMinimised =
                startMinimisedToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void restartApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();

            string applicationPath = Application.ExecutablePath;
            System.Diagnostics.Process.Start(applicationPath);
            Environment.Exit(0);
        }

        private void resetToDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Reset all settings to defaults? This will restart the application.",
                "Reset to defaults",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                restartApplicationToolStripMenuItem_Click(sender, e);
            }
        }

        private void backgroundWorker1_DoWork(object sender,
            System.ComponentModel.DoWorkEventArgs e)
        {
            // Reserved
        }
    }
}
