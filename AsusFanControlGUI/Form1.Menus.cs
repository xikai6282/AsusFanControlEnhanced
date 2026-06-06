using System;
using System.Drawing;
using System.Windows.Forms;
using AsusFanControlGUI.Localization;
using AsusFanControlGUI.Theming;

namespace AsusFanControlGUI
{
    /// <summary>
    /// Form1 鎵╁睍 鈥?璇█/涓婚鑿滃崟椤?(閫氳繃浠ｇ爜鑰岄潪 Designer 娣诲姞锛屼繚鎸?Designer.cs 绋冲畾)
    /// </summary>
    public partial class Form1
    {
        // 鈹€鈹€ 鏂板鑿滃崟椤瑰０鏄?鈹€鈹€
        private ToolStripMenuItem languageToolStripMenuItem;
        private ToolStripMenuItem englishToolStripMenuItem;
        private ToolStripMenuItem chineseToolStripMenuItem;
        private ToolStripMenuItem themeToolStripMenuItem;
        private ToolStripMenuItem lightThemeToolStripMenuItem;
        private ToolStripMenuItem darkThemeToolStripMenuItem;

        /// <summary>
        /// 鍦?init() 涔嬪墠璋冪敤锛屽姩鎬佹坊鍔犺瑷€鍜屼富棰樿彍鍗曞埌鑿滃崟鏍?        /// </summary>
        private void AddLocalizationAndThemeMenus()
        {
            // 鈹€鈹€ 璇█鑿滃崟 鈹€鈹€
            languageToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "languageToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Language"),
                Size = new Size(80, 24),
            };

            englishToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "englishToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Language.English"),
                Size = new Size(160, 26),
                CheckOnClick = false,
            };
            englishToolStripMenuItem.Click += (s, e) => LocalizationManager.SetLanguage("en");

            chineseToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "chineseToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Language.Chinese"),
                Size = new Size(160, 26),
                CheckOnClick = false,
            };
            chineseToolStripMenuItem.Click += (s, e) => LocalizationManager.SetLanguage("zh-CN");

            languageToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                englishToolStripMenuItem,
                chineseToolStripMenuItem,
            });

            // 鈹€鈹€ 涓婚鑿滃崟 鈹€鈹€
            themeToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "themeToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Theme"),
                Size = new Size(80, 24),
            };

            lightThemeToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "lightThemeToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Theme.Light"),
                Size = new Size(140, 26),
                CheckOnClick = false,
            };
            lightThemeToolStripMenuItem.Click += (s, e) => ThemeManager.SetTheme(AppTheme.Light);

            darkThemeToolStripMenuItem = new ToolStripMenuItem
            {
                Name = "darkThemeToolStripMenuItem",
                Text = LocalizationManager.Get("Menu.Theme.Dark"),
                Size = new Size(140, 26),
                CheckOnClick = false,
            };
            darkThemeToolStripMenuItem.Click += (s, e) => ThemeManager.SetTheme(AppTheme.Dark);

            themeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[]
            {
                lightThemeToolStripMenuItem,
                darkThemeToolStripMenuItem,
            });

            // 鈹€鈹€ 鎻掑叆鍒拌彍鍗曟爮 (鍦?About 涔嬪墠) 鈹€鈹€
            // menuStrip1.Items 椤哄簭: Advanced, Options, CheckForUpdates, About
            // 鍦?CheckForUpdates 涔嬪墠鎻掑叆 Language 鍜?Theme
            int insertIndex = menuStrip1.Items.IndexOf(toolStripMenuItemCheckForUpdates);
            if (insertIndex >= 0)
            {
                menuStrip1.Items.Insert(insertIndex, languageToolStripMenuItem);
                menuStrip1.Items.Insert(insertIndex, themeToolStripMenuItem);
            }
            else
            {
                // 鍥為€€锛氭彃鍦?Options 涔嬪悗
                int optIndex = menuStrip1.Items.IndexOf(optionsToolStripMenuItem);
                menuStrip1.Items.Insert(optIndex + 1, languageToolStripMenuItem);
                menuStrip1.Items.Insert(optIndex + 2, themeToolStripMenuItem);
            }
        }

        /// <summary>
        /// 鏈湴鍖?涓婚鍒囨崲鏃跺埛鏂拌彍鍗曢」鏂囨湰鍜屼富棰?        /// </summary>
        private void RefreshLocalizedUI()
        {
            // 鍒锋柊鎵€鏈夋帶浠舵枃鏈?            LocalizationManager.ApplyToForm(this);

            // 鎵嬪姩鍒锋柊涓嶄細鑷姩鏄犲皠鐨勫姩鎬佹枃鏈?            RefreshDynamicLabels();
        }

        /// <summary>
        /// 鏈湴鍖?涓婚鍒囨崲鍚庡埛鏂?        /// </summary>
        private void OnLanguageOrThemeChanged()
        {
            // 鍏堝埛鏂版枃鏈?            RefreshLocalizedUI();

            // 鍐嶅簲鐢ㄤ富棰?            ThemeManager.ApplyToForm(this);
            ThemeManager.ApplyToMenuStrip(menuStrip1);

            // 鍒锋柊鏇茬嚎鍥?            pictureBoxFanCurve.Invalidate();

            // 鍒锋柊鐘舵€佹爣绛?            RefreshDynamicLabels();
        }

        /// <summary>
        /// 鍒锋柊鍔ㄦ€佹爣绛撅紙闈?Designer 鏄犲皠鐨勬枃鏈級
        /// </summary>
        private void RefreshDynamicLabels()
        {
            // 鍒锋柊 labelValue
            if (currentFanRPM == 0)
                labelValue.Text = LocalizationManager.Get("Fan.TurnedOff");
            else
                labelValue.Text = currentFanRPM.ToString() + LocalizationManager.Get("Fan.PWM");

            // 鍒锋柊鐘舵€佹爮
            UpdateStatusLabel();

            // 鍒锋柊 label5 鐧惧垎姣旀枃鏈?            label5.Text = trackBarFanSpeed.Value.ToString() + LocalizationManager.Get("Fan.PercentLabel");
        }

        /// <summary>
        /// 璁㈤槄鏈湴鍖栧拰涓婚鍙樺寲浜嬩欢
        /// </summary>
        private void SubscribeToLocalizationAndThemeEvents()
        {
            LocalizationManager.LanguageChanged += () =>
            {
                // UI 绾跨▼瀹夊叏璋冪敤
                if (InvokeRequired)
                    Invoke(new Action(OnLanguageOrThemeChanged));
                else
                    OnLanguageOrThemeChanged();
            };

            ThemeManager.ThemeChanged += (theme) =>
            {
                if (InvokeRequired)
                    Invoke(new Action(OnLanguageOrThemeChanged));
                else
                    OnLanguageOrThemeChanged();
            };
        }
    }
}
