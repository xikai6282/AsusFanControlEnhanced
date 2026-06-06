using System;
using System.Drawing;
using System.Windows.Forms;

namespace AsusFanControlGUI.Theming
{
    /// <summary>
    /// 涓婚鏋氫妇
    /// </summary>
    public enum AppTheme
    {
        Light,
        Dark
    }

    /// <summary>
    /// 涓婚绠＄悊鍣?鈥?娣辫壊/娴呰壊涓婚涓€閿垏鎹?    /// Theme manager 鈥?one-click dark/light theme switching
    /// </summary>
    public static class ThemeManager
    {
        public static AppTheme CurrentTheme { get; private set; } = AppTheme.Light;
        public static event Action<AppTheme> ThemeChanged;

        // 鈹€鈹€ 娴呰壊涓婚鑹叉澘 鈹€鈹€
        public static class LightPalette
        {
            public static readonly Color FormBack         = Color.FromArgb(245, 245, 248);
            public static readonly Color ControlBack      = Color.White;
            public static readonly Color ControlFore      = Color.FromArgb(30, 30, 30);
            public static readonly Color Accent           = Color.FromArgb(0, 120, 212);
            public static readonly Color AccentLight      = Color.FromArgb(229, 241, 251);
            public static readonly Color Border           = Color.FromArgb(210, 210, 215);
            public static readonly Color GraphBack        = Color.FromArgb(248, 248, 250);
            public static readonly Color GraphGrid        = Color.FromArgb(220, 220, 226);
            public static readonly Color GraphLine        = Color.FromArgb(0, 120, 212);
            public static readonly Color GraphPoint       = Color.FromArgb(0, 180, 100);
            public static readonly Color GraphPointDrag   = Color.FromArgb(220, 50, 50);
            public static readonly Color StatusGood       = Color.FromArgb(0, 160, 80);
            public static readonly Color StatusWarn       = Color.FromArgb(240, 150, 0);
            public static readonly Color StatusDanger     = Color.FromArgb(220, 50, 50);
            public static readonly Color Separator        = Color.FromArgb(200, 200, 206);
        }

        // 鈹€鈹€ 娣辫壊涓婚鑹叉澘 鈹€鈹€
        public static class DarkPalette
        {
            public static readonly Color FormBack         = Color.FromArgb(32, 32, 36);
            public static readonly Color ControlBack      = Color.FromArgb(45, 45, 50);
            public static readonly Color ControlFore      = Color.FromArgb(230, 230, 235);
            public static readonly Color Accent           = Color.FromArgb(80, 165, 245);
            public static readonly Color AccentLight      = Color.FromArgb(40, 60, 80);
            public static readonly Color Border           = Color.FromArgb(60, 60, 66);
            public static readonly Color GraphBack        = Color.FromArgb(38, 38, 44);
            public static readonly Color GraphGrid        = Color.FromArgb(55, 55, 60);
            public static readonly Color GraphLine        = Color.FromArgb(80, 165, 245);
            public static readonly Color GraphPoint       = Color.FromArgb(80, 220, 140);
            public static readonly Color GraphPointDrag   = Color.FromArgb(255, 90, 90);
            public static readonly Color StatusGood       = Color.FromArgb(60, 200, 120);
            public static readonly Color StatusWarn       = Color.FromArgb(255, 180, 40);
            public static readonly Color StatusDanger     = Color.FromArgb(255, 90, 90);
            public static readonly Color Separator        = Color.FromArgb(70, 70, 76);
        }

        // 鈹€鈹€ 渚挎嵎鑾峰彇褰撳墠鑹叉澘 鈹€鈹€
        public static Color Back      => CurrentTheme == AppTheme.Dark ? DarkPalette.FormBack      : LightPalette.FormBack;
        public static Color CtrlBack  => CurrentTheme == AppTheme.Dark ? DarkPalette.ControlBack   : LightPalette.ControlBack;
        public static Color CtrlFore  => CurrentTheme == AppTheme.Dark ? DarkPalette.ControlFore   : LightPalette.ControlFore;
        public static Color Accent    => CurrentTheme == AppTheme.Dark ? DarkPalette.Accent        : LightPalette.Accent;
        public static Color AccentLt  => CurrentTheme == AppTheme.Dark ? DarkPalette.AccentLight   : LightPalette.AccentLight;
        public static Color Border    => CurrentTheme == AppTheme.Dark ? DarkPalette.Border        : LightPalette.Border;
        public static Color GraphBg   => CurrentTheme == AppTheme.Dark ? DarkPalette.GraphBack     : LightPalette.GraphBack;
        public static Color GraphGrid => CurrentTheme == AppTheme.Dark ? DarkPalette.GraphGrid     : LightPalette.GraphGrid;
        public static Color GraphLine => CurrentTheme == AppTheme.Dark ? DarkPalette.GraphLine     : LightPalette.GraphLine;
        public static Color GraphPt   => CurrentTheme == AppTheme.Dark ? DarkPalette.GraphPoint    : LightPalette.GraphPoint;
        public static Color GraphPtDr => CurrentTheme == AppTheme.Dark ? DarkPalette.GraphPointDrag: LightPalette.GraphPointDrag;
        public static Color Sep       => CurrentTheme == AppTheme.Dark ? DarkPalette.Separator     : LightPalette.Separator;

        // 鈹€鈹€ 鏍稿績鏂规硶 鈹€鈹€

        public static void Initialize()
        {
            string saved = Properties.Settings.Default.uiTheme;
            if (!string.IsNullOrEmpty(saved))
            {
                if (saved == "dark") CurrentTheme = AppTheme.Dark;
                else CurrentTheme = AppTheme.Light;
            }
        }

        public static void SetTheme(AppTheme theme)
        {
            if (CurrentTheme == theme) return;
            CurrentTheme = theme;
            Properties.Settings.Default.uiTheme = theme == AppTheme.Dark ? "dark" : "light";
            Properties.Settings.Default.Save();
            ThemeChanged?.Invoke(theme);
        }

        public static void Toggle()
        {
            SetTheme(CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark);
        }

        /// <summary>
        /// 灏嗕富棰樺簲鐢ㄥ埌 Form 鍙婃墍鏈夊瓙鎺т欢
        /// </summary>
        public static void ApplyToForm(Form form)
        {
            form.BackColor = Back;
            form.ForeColor = CtrlFore;
            ApplyToControl(form);
        }

        private static void ApplyToControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                StyleControl(ctrl);
                if (ctrl.Controls.Count > 0)
                    ApplyToControl(ctrl);
            }
        }

        private static void StyleControl(Control ctrl)
        {
            // 璺宠繃鑿滃崟鏉″拰鐘舵€佹潯锛堝畠浠湁鑷繁鐨勬覆鏌擄級
            if (ctrl is MenuStrip || ctrl is ToolStrip || ctrl is StatusStrip
                || ctrl is ContextMenuStrip)
                return;

            ctrl.BackColor = CtrlBack;
            ctrl.ForeColor = CtrlFore;

            // 鎸夐挳
            if (ctrl is Button btn)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = Accent;
                btn.FlatAppearance.BorderSize = 1;
                btn.BackColor = CtrlBack;
                btn.ForeColor = Accent;
                btn.FlatAppearance.MouseOverBackColor = AccentLt;
                btn.FlatAppearance.MouseDownBackColor = Accent;
            }

            // RadioButton
            if (ctrl is RadioButton rb)
            {
                rb.FlatStyle = FlatStyle.Flat;
                rb.ForeColor = CtrlFore;
            }

            // GroupBox
            if (ctrl is GroupBox gb)
            {
                gb.ForeColor = Accent;
                foreach (Control inner in gb.Controls)
                    StyleControl(inner);
            }

            // Label
            if (ctrl is Label lbl)
            {
                lbl.ForeColor = CtrlFore;
            }

            // NumericUpDown
            if (ctrl is NumericUpDown nud)
            {
                nud.BackColor = CtrlBack;
                nud.ForeColor = CtrlFore;
                nud.BorderStyle = BorderStyle.FixedSingle;
            }

            // TrackBar
            if (ctrl is TrackBar tb)
            {
                tb.BackColor = Back;
            }

            // TextBox
            if (ctrl is TextBox tb2)
            {
                tb2.BackColor = CtrlBack;
                tb2.ForeColor = CtrlFore;
                tb2.BorderStyle = BorderStyle.FixedSingle;
            }

            // PictureBox
            if (ctrl is PictureBox pb)
            {
                pb.BackColor = GraphBg;
            }

            // Panel
            if (ctrl is Panel pnl)
            {
                pnl.BackColor = Back;
            }
        }

        /// <summary>
        /// 璁剧疆鑿滃崟鏉＄殑棰滆壊锛堥渶瑕侀澶栨覆鏌擄級
        /// </summary>
        public static void ApplyToMenuStrip(MenuStrip menu)
        {
            menu.BackColor = CtrlBack;
            menu.ForeColor = CtrlFore;
            menu.Renderer = new ModernToolStripRenderer();

            foreach (ToolStripItem item in menu.Items)
                StyleMenuItem(item);
        }

        private static void StyleMenuItem(ToolStripItem item)
        {
            item.ForeColor = CtrlFore;
            item.BackColor = CtrlBack;

            if (item is ToolStripMenuItem dropDown)
            {
                dropDown.DropDown.BackColor = CtrlBack;
                dropDown.DropDown.ForeColor = CtrlFore;

                foreach (ToolStripItem child in dropDown.DropDownItems)
                    StyleMenuItem(child);
            }
        }

        /// <summary>
        /// 鐜颁唬椋庢牸鐨?ToolStrip 娓叉煋鍣?        /// </summary>
        private class ModernToolStripRenderer : ToolStripProfessionalRenderer
        {
            public ModernToolStripRenderer() : base(new ModernColorTable()) { }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    using (Brush b = new SolidBrush(
                        CurrentTheme == AppTheme.Dark
                            ? Color.FromArgb(60, 60, 68)
                            : Color.FromArgb(229, 241, 251)))
                    {
                        e.Graphics.FillRectangle(b, rc);
                    }
                    using (Pen p = new Pen(Accent))
                    {
                        e.Graphics.DrawRectangle(p, 0, 0, rc.Width - 1, rc.Height - 1);
                    }
                }
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                using (Pen p = new Pen(Border))
                    e.Graphics.DrawRectangle(p, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                int y = e.Item.Height / 2;
                using (Pen p = new Pen(Border))
                    e.Graphics.DrawLine(p, 4, y, e.Item.Width - 8, y);
            }
        }

        private class ModernColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => CurrentTheme == AppTheme.Dark
                ? Color.FromArgb(60, 60, 68)
                : Color.FromArgb(229, 241, 251);

            public override Color MenuItemBorder => CurrentTheme == AppTheme.Dark
                ? Color.FromArgb(50, 50, 58)
                : Color.FromArgb(200, 200, 210);

            public override Color MenuBorder => CurrentTheme == AppTheme.Dark
                ? Color.FromArgb(50, 50, 58)
                : Color.FromArgb(200, 200, 210);

            public override Color ToolStripDropDownBackground => CurrentTheme == AppTheme.Dark
                ? Color.FromArgb(45, 45, 50)
                : Color.White;

            public override Color ImageMarginGradientBegin => ToolStripDropDownBackground;
            public override Color ImageMarginGradientMiddle => ToolStripDropDownBackground;
            public override Color ImageMarginGradientEnd => ToolStripDropDownBackground;

            public override Color MenuItemSelectedGradientBegin => MenuItemSelected;
            public override Color MenuItemSelectedGradientEnd => MenuItemSelected;
            public override Color MenuItemPressedGradientBegin => MenuItemSelected;
            public override Color MenuItemPressedGradientEnd => MenuItemSelected;
            public override Color CheckBackground => MenuItemSelected;
            public override Color CheckPressedBackground => MenuItemSelected;
            public override Color CheckSelectedBackground => MenuItemSelected;
        }
    }
}
