using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace AsusFanControlGUI.Localization
{
    public static class LocalizationManager
    {
        public static string CurrentLanguage { get; private set; } = "en";
        public static event Action LanguageChanged;

        private static readonly string NL = Environment.NewLine;

        // 鈹€鈹€ English 鈹€鈹€
        private static readonly Dictionary<string, string> EN = new Dictionary<string, string>
        {
            ["App.Title"] = "ASUS Fan Control Enhanced",
            ["Menu.Advanced"] = "Advanced",
            ["Menu.Advanced.ForbidUnsafe"] = "Forbid unsafe settings",
            ["Menu.Advanced.AllowTextCurve"] = "Allow Fan Curve Setting via Text",
            ["Menu.Options"] = "Options",
            ["Menu.Options.StartupSettings"] = "Startup settings",
            ["Menu.Options.StartMinimised"] = "Start minimised",
            ["Menu.Options.StartWithWindows"] = "Start with Windows",
            ["Menu.Options.Restart"] = "Restart application",
            ["Menu.Options.ResetDefaults"] = "Reset to defaults",
            ["Menu.Language"] = "Language",
            ["Menu.Language.English"] = "English",
            ["Menu.Language.Chinese"] = "绠€浣撲腑鏂?,
            ["Menu.Theme"] = "Theme",
            ["Menu.Theme.Light"] = "Light",
            ["Menu.Theme.Dark"] = "Dark",
            ["Menu.CheckUpdates"] = "Check for updates",
            ["Menu.About"] = "About",
            ["Fan.TurnOff"] = "Turn off",
            ["Fan.TurnOnControl"] = "Turn on fan control",
            ["Fan.TurnOnCurve"] = "Turn on fan curve",
            ["Fan.CurrentValue"] = "Current value:",
            ["Fan.TurnedOff"] = "turned off",
            ["Fan.PWM"] = "% (PWM Fan)",
            ["Fan.PercentLabel"] = "% Fan",
            ["Fan.Temperature"] = "Temperature (掳C)",
            ["Fan.FanSpeed"] = "Fan Speed (%)",
            ["Status.CurrentRPM"] = "Current RPM:",
            ["Status.CurrentCPUTemp"] = "Current CPU temp:",
            ["Status.SettingSpeed"] = "Setting speed to: {0}%",
            ["Status.CurveSpeed"] = "Set fan speed to {0}%, current temp: {1}掳C",
            ["Status.YieldedToSystem"] = "Control yielded to system 鈥?temperature outside range.",
            ["Curve.Reset"] = "Reset",
            ["Curve.SetCurve"] = "Set",
            ["Curve.ResetTooltip"] = "Reset curve points",
            ["Curve.SetTooltip"] = "Set fan curve from text",
            ["Curve.GraphTooltip"] = "Fan Curve Graph",
            ["Curve.PointTooltip"] = "Temp: {0}掳C  |  Fan: {1}%",
            ["Curve.MaxPointsReached"] = "Maximum number of points (20) reached.",
            ["Advanced.Group"] = "Advanced",
            ["Advanced.Hysteresis"] = "Hysteresis:",
            ["Advanced.HysteresisUnit"] = "s",
            ["Advanced.UpdateSpeed"] = "Update Speed:",
            ["Advanced.UpdateSpeedUnit"] = "ms",
            ["Tray.Open"] = "Open",
            ["Tray.Close"] = "Close",
            ["Tray.Off"] = "ASUS Fan Control - Off",
            ["Tray.Manual"] = "Fan: {0}%  |  Temp: {1}掳C",
            ["Dialog.SaveSuccess"] = "Save successful.",
        };

        // 鈹€鈹€ 绠€浣撲腑鏂?鈹€鈹€
        private static readonly Dictionary<string, string> ZH = new Dictionary<string, string>
        {
            ["App.Title"] = "鍗庣椋庢墖鎺у埗澧炲己鐗?,
            ["Menu.Advanced"] = "楂樼骇",
            ["Menu.Advanced.ForbidUnsafe"] = "绂佹涓嶅畨鍏ㄨ缃?,
            ["Menu.Advanced.AllowTextCurve"] = "鍏佽閫氳繃鏂囨湰璁剧疆椋庢墖鏇茬嚎",
            ["Menu.Options"] = "閫夐」",
            ["Menu.Options.StartupSettings"] = "鍚姩璁剧疆",
            ["Menu.Options.StartMinimised"] = "鍚姩鏃舵渶灏忓寲",
            ["Menu.Options.StartWithWindows"] = "寮€鏈鸿嚜鍚?,
            ["Menu.Options.Restart"] = "閲嶅惎绋嬪簭",
            ["Menu.Options.ResetDefaults"] = "鎭㈠榛樿璁剧疆",
            ["Menu.Language"] = "璇█",
            ["Menu.Language.English"] = "English",
            ["Menu.Language.Chinese"] = "绠€浣撲腑鏂?,
            ["Menu.Theme"] = "涓婚",
            ["Menu.Theme.Light"] = "娴呰壊",
            ["Menu.Theme.Dark"] = "娣辫壊",
            ["Menu.CheckUpdates"] = "妫€鏌ユ洿鏂?,
            ["Menu.About"] = "鍏充簬",
            ["Fan.TurnOff"] = "鍏抽棴椋庢墖鎺у埗",
            ["Fan.TurnOnControl"] = "鎵嬪姩椋庢墖鎺у埗",
            ["Fan.TurnOnCurve"] = "椋庢墖鏇茬嚎鎺у埗",
            ["Fan.CurrentValue"] = "褰撳墠鍊硷細",
            ["Fan.TurnedOff"] = "宸插叧闂?,
            ["Fan.PWM"] = "% (PWM 椋庢墖)",
            ["Fan.PercentLabel"] = "% 椋庢墖",
            ["Fan.Temperature"] = "娓╁害 (掳C)",
            ["Fan.FanSpeed"] = "椋庢墖杞€?(%)",
            ["Status.CurrentRPM"] = "褰撳墠杞€燂細",
            ["Status.CurrentCPUTemp"] = "褰撳墠 CPU 娓╁害锛?,
            ["Status.SettingSpeed"] = "姝ｅ湪璁剧疆杞€燂細{0}%",
            ["Status.CurveSpeed"] = "椋庢墖杞€熷凡璁句负 {0}%锛屽綋鍓嶆俯搴︼細{1}掳C",
            ["Status.YieldedToSystem"] = "娓╁害瓒呭嚭鑼冨洿锛屽凡浜よ繕绯荤粺鎺у埗銆?,
            ["Curve.Reset"] = "閲嶇疆",
            ["Curve.SetCurve"] = "璁剧疆",
            ["Curve.ResetTooltip"] = "閲嶇疆鏇茬嚎鎺у埗鐐?,
            ["Curve.SetTooltip"] = "浠庢枃鏈缃鎵囨洸绾?,
            ["Curve.GraphTooltip"] = "椋庢墖鏇茬嚎鍥?,
            ["Curve.PointTooltip"] = "娓╁害锛歿0}掳C  |  椋庢墖锛歿1}%",
            ["Curve.MaxPointsReached"] = "宸茶揪鍒版渶澶ф帶鍒剁偣鏁?(20)銆?,
            ["Advanced.Group"] = "楂樼骇璁剧疆",
            ["Advanced.Hysteresis"] = "婊炲悗锛?,
            ["Advanced.HysteresisUnit"] = "绉?,
            ["Advanced.UpdateSpeed"] = "鏇存柊閫熷害锛?,
            ["Advanced.UpdateSpeedUnit"] = "姣",
            ["Tray.Open"] = "鎵撳紑",
            ["Tray.Close"] = "閫€鍑?,
            ["Tray.Off"] = "鍗庣椋庢墖鎺у埗 - 宸插叧闂?,
            ["Tray.Manual"] = "椋庢墖锛歿0}%  |  娓╁害锛歿1}掳C",
            ["Dialog.SaveSuccess"] = "淇濆瓨鎴愬姛銆?,
        };

        // 鈹€鈹€ 鍚崲琛岀殑闀挎枃鏈紙杩愯鏃舵嫾鎺ワ級鈹€鈹€
        private static string GetLongText(string key, string lang)
        {
            if (lang == "zh-CN")
            {
                switch (key)
                {
                    case "Dialog.About.Title":
                        return "鍏充簬 鍗庣椋庢墖鎺у埗澧炲己鐗?;
                    case "Dialog.About.Text":
                        return "鍗庣椋庢墖鎺у埗澧炲己鐗? + NL + NL
                            + "鐗堟湰锛? + "{0}" + NL + NL
                            + "浣滆€咃細Darren80" + NL
                            + "绀惧尯 UI 浼樺寲鐗? + NL + NL
                            + "https://github.com/Darren80/AsusFanControlEnhanced";
                    case "Dialog.Error.Title":
                        return "鏈鐞嗗紓甯?- 鍗庣椋庢墖鎺у埗澧炲己鐗?;
                    case "Dialog.Error.Message":
                        return "绋嬪簭鍙戠敓閿欒锛岃灏濊瘯閲嶅惎銆? + NL + NL
                            + "濡傛灉闂鎸佺画瀛樺湪锛岃鍦?GitHub 鎻愪氦 Issue锛? + NL
                            + "https://github.com/Darren80/AsusFanControlEnhanced/issues" + NL + NL
                            + "鍫嗘爤璺熻釜锛? + NL + "{1}" + NL + NL
                            + "绋嬪簭璁剧疆锛? + NL + "{2}" + NL + NL
                            + "閿欒淇℃伅锛? + NL + "{0}" + NL;
                    case "Dialog.Error.Restart":
                        return "閲嶅惎";
                    case "Dialog.Error.Exit":
                        return "閫€鍑?;
                    case "Error.TempOutOfRange":
                        return "CPU 娓╁害瓒呭嚭瀹夊叏鑼冨洿 ({0}掳C)銆? + NL
                            + "璇峰皾璇曢噸鍚▼搴忥細閫夐」 鈫?閲嶅惎绋嬪簭";
                    case "Error.InvalidCurveChars":
                        return "椋庢墖鏇茬嚎瀛楃涓插寘鍚棤鏁堝瓧绗︺€? + NL
                            + "鍏佽鐨勫瓧绗︼細鏁板瓧 0-9銆侀€楀彿銆佺煭妯嚎銆?;
                    case "Error.InvalidCurveFormat":
                        return "鏃犳晥鐨勬洸绾挎牸寮忋€傛湡鏈涙牸寮忥細娓╁害,杞€?娓╁害,杞€?...";
                    case "Error.TempOutOfRangeCurve":
                        return "娓╁害 {0}掳C 瓒呭嚭鑼冨洿 ({1}鈥搟2}掳C)銆?;
                    case "Error.FanOutOfRangeCurve":
                        return "椋庢墖杞€?{0}% 瓒呭嚭鑼冨洿 ({1}鈥搟2}%)銆?;
                    case "Error.CurveParse":
                        return "瑙ｆ瀽椋庢墖鏇茬嚎鐐规椂鍑洪敊锛? + NL + NL + "{0}";
                }
            }
            else
            {
                switch (key)
                {
                    case "Dialog.About.Title":
                        return "About ASUS Fan Control Enhanced";
                    case "Dialog.About.Text":
                        return "ASUS Fan Control Enhanced" + NL + NL
                            + "Version: " + "{0}" + NL + NL
                            + "Author: Darren80" + NL
                            + "Improved UI by community" + NL + NL
                            + "https://github.com/Darren80/AsusFanControlEnhanced";
                    case "Dialog.Error.Title":
                        return "Unhandled Exception - ASUS Fan Control Enhanced";
                    case "Dialog.Error.Message":
                        return "An error occurred, please try restarting the application." + NL + NL
                            + "If the error persists, create a GitHub issue at:" + NL
                            + "https://github.com/Darren80/AsusFanControlEnhanced/issues" + NL + NL
                            + "Stack Trace:" + NL + "{1}" + NL + NL
                            + "Application Settings:" + NL + "{2}" + NL + NL
                            + "Error Message:" + NL + "{0}" + NL;
                    case "Dialog.Error.Restart":
                        return "Restart";
                    case "Dialog.Error.Exit":
                        return "Exit";
                    case "Error.TempOutOfRange":
                        return "CPU temperature is outside safe range at {0}掳C." + NL
                            + "Try restarting the application: Options 鈫?Restart application";
                    case "Error.InvalidCurveChars":
                        return "The fan curve string contains invalid characters." + NL
                            + "Allowed: 0-9, comma, dash.";
                    case "Error.InvalidCurveFormat":
                        return "Invalid curve format. Expected: temp,fanSpeed-temp,fanSpeed ...";
                    case "Error.TempOutOfRangeCurve":
                        return "Temperature {0}掳C is out of range ({1}鈥搟2}掳C).";
                    case "Error.FanOutOfRangeCurve":
                        return "Fan speed {0}% is out of range ({1}鈥搟2}%).";
                    case "Error.CurveParse":
                        return "An error occurred while parsing the fan curve points:" + NL + NL + "{0}";
                }
            }
            return key;
        }

        private static readonly Dictionary<string, Dictionary<string, string>> All =
            new Dictionary<string, Dictionary<string, string>> { ["en"] = EN, ["zh-CN"] = ZH };

        // 鈹€鈹€ 鐭枃鏈紙鏃犳崲琛岋級鈹€鈹€
        public static string Get(string key)
        {
            if (All.TryGetValue(CurrentLanguage, out var d) && d.TryGetValue(key, out var v))
                return v;
            if (CurrentLanguage != "en" && All.TryGetValue("en", out var ed) && ed.TryGetValue(key, out var ev))
                return ev;
            return "[[" + key + "]]";
        }

        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        // 鈹€鈹€ 闀挎枃鏈紙鍚崲琛岋級鈹€鈹€
        public static string GetLong(string key)
        {
            return GetLongText(key, CurrentLanguage);
        }

        public static string GetLong(string key, params object[] args)
        {
            return string.Format(GetLongText(key, CurrentLanguage), args);
        }

        public static void SetLanguage(string langCode)
        {
            if (!All.ContainsKey(langCode)) return;
            if (CurrentLanguage == langCode) return;
            CurrentLanguage = langCode;
            Properties.Settings.Default.uiLanguage = langCode;
            Properties.Settings.Default.Save();
            if (LanguageChanged != null) LanguageChanged();
        }

        public static void Initialize()
        {
            string saved = Properties.Settings.Default.uiLanguage;
            if (!string.IsNullOrEmpty(saved) && All.ContainsKey(saved))
                CurrentLanguage = saved;
            else
                CurrentLanguage = "en";
        }

        public static void ApplyToForm(Form form)
        {
            form.Text = Get("App.Title");
            ApplyToControl(form);
        }

        private static void ApplyToControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrlMap.TryGetValue(ctrl.Name ?? "", out var key))
                    ctrl.Text = Get(key);
                if (ctrl is MenuStrip m) ApplyMenu(m);
                else if (ctrl is ContextMenuStrip c) ApplyCtx(c);
                else if (ctrl.Controls.Count > 0) ApplyToControl(ctrl);
            }
        }

        private static void ApplyMenu(MenuStrip menu)
        {
            foreach (ToolStripItem item in menu.Items) ApplyItem(item);
        }

        private static void ApplyCtx(ContextMenuStrip menu)
        {
            foreach (ToolStripItem item in menu.Items) ApplyItem(item);
        }

        private static void ApplyItem(ToolStripItem item)
        {
            if (ctrlMap.TryGetValue(item.Name ?? "", out var key))
                item.Text = Get(key);
            if (item is ToolStripMenuItem dd && dd.DropDownItems.Count > 0)
                foreach (ToolStripItem child in dd.DropDownItems) ApplyItem(child);
        }

        private static readonly Dictionary<string, string> ctrlMap = new Dictionary<string, string>
        {
            ["toolStripMenuItem1"] = "Menu.Advanced",
            ["toolStripMenuItemForbidUnsafeSettings"] = "Menu.Advanced.ForbidUnsafe",
            ["allowFanCurveSettingViaTextToolStripMenuItem"] = "Menu.Advanced.AllowTextCurve",
            ["optionsToolStripMenuItem"] = "Menu.Options",
            ["startupSettingsToolStripMenuItem"] = "Menu.Options.StartupSettings",
            ["startMinimisedToolStripMenuItem"] = "Menu.Options.StartMinimised",
            ["startWithWindowsToolStripMenuItem"] = "Menu.Options.StartWithWindows",
            ["restartApplicationToolStripMenuItem"] = "Menu.Options.Restart",
            ["resetToDefaultsToolStripMenuItem"] = "Menu.Options.ResetDefaults",
            ["languageToolStripMenuItem"] = "Menu.Language",
            ["englishToolStripMenuItem"] = "Menu.Language.English",
            ["chineseToolStripMenuItem"] = "Menu.Language.Chinese",
            ["themeToolStripMenuItem"] = "Menu.Theme",
            ["lightThemeToolStripMenuItem"] = "Menu.Theme.Light",
            ["darkThemeToolStripMenuItem"] = "Menu.Theme.Dark",
            ["toolStripMenuItemCheckForUpdates"] = "Menu.CheckUpdates",
            ["aboutToolStripMenuItem"] = "Menu.About",
            ["radioButton1"] = "Fan.TurnOff",
            ["fanControlRadioButton"] = "Fan.TurnOnControl",
            ["fanCurveRadioButton"] = "Fan.TurnOnCurve",
            ["label1"] = "Fan.CurrentValue",
            ["label2"] = "Status.CurrentRPM",
            ["label4"] = "Status.CurrentCPUTemp",
            ["groupBox2"] = "Advanced.Group",
            ["label6"] = "Advanced.Hysteresis",
            ["label9"] = "Advanced.HysteresisUnit",
            ["label7"] = "Advanced.UpdateSpeed",
            ["label8"] = "Advanced.UpdateSpeedUnit",
            ["ResetCurvePoints"] = "Curve.Reset",
            ["button3"] = "Curve.SetCurve",
            ["openToolStripMenuItem"] = "Tray.Open",
            ["closeToolStripMenuItem"] = "Tray.Close",
        };
    }
}
