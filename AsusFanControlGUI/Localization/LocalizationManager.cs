using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace AsusFanControlGUI.Localization
{
    /// <summary>
    /// 鏈湴鍖栫鐞嗗櫒 鈥?鏀寔杩愯鏃朵腑/鑻卞垏鎹?    /// Localization manager 鈥?runtime English/Chinese switching
    /// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// 褰撳墠璇█浠ｇ爜锛?en" / "zh-CN"
        /// </summary>
        public static string CurrentLanguage { get; private set; } = "en";

        /// <summary>
        /// 璇█鍒囨崲鏃惰Е鍙戯紝Form 璁㈤槄浠ュ埛鏂版墍鏈?UI 鏂囧瓧
        /// </summary>
        public static event Action LanguageChanged;

        // 鈹€鈹€ 鑻辨枃锛堥粯璁わ級 鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€
        private static readonly Dictionary<string, string> English = new Dictionary<string, string>
        {
            // 鈹€鈹€ 绐楀彛鏍囬 鈹€鈹€
            ["App.Title"]                       = "ASUS Fan Control Enhanced",

            // 鈹€鈹€ 鑿滃崟 鈹€鈹€
            ["Menu.Advanced"]                   = "Advanced",
            ["Menu.Advanced.ForbidUnsafe"]      = "Forbid unsafe settings",
            ["Menu.Advanced.AllowTextCurve"]    = "Allow Fan Curve Setting via Text",
            ["Menu.Options"]                    = "Options",
            ["Menu.Options.StartupSettings"]    = "Startup settings",
            ["Menu.Options.StartMinimised"]     = "Start minimised",
            ["Menu.Options.StartWithWindows"]   = "Start with Windows",
            ["Menu.Options.Restart"]            = "Restart application",
            ["Menu.Options.ResetDefaults"]      = "Reset to defaults",
            ["Menu.Language"]                   = "Language",
            ["Menu.Language.English"]           = "English",
            ["Menu.Language.Chinese"]           = "绠€浣撲腑鏂?,
            ["Menu.Theme"]                      = "Theme",
            ["Menu.Theme.Light"]                = "Light",
            ["Menu.Theme.Dark"]                 = "Dark",
            ["Menu.CheckUpdates"]               = "Check for updates",
            ["Menu.About"]                      = "About",

            // 鈹€鈹€ 椋庢墖鎺у埗鍖?鈹€鈹€
            ["Fan.TurnOff"]                     = "Turn off",
            ["Fan.TurnOnControl"]               = "Turn on fan control",
            ["Fan.TurnOnCurve"]                 = "Turn on fan curve",
            ["Fan.CurrentValue"]                = "Current value:",
            ["Fan.TurnedOff"]                   = "turned off",
            ["Fan.PWM"]                         = "% (PWM Fan)",
            ["Fan.PercentLabel"]                = "% Fan",
            ["Fan.Temperature"]                 = "Temperature (掳C)",
            ["Fan.FanSpeed"]                    = "Fan Speed (%)",

            // 鈹€鈹€ 鐘舵€佷俊鎭?鈹€鈹€
            ["Status.CurrentRPM"]               = "Current RPM:",
            ["Status.CurrentCPUTemp"]           = "Current CPU temp:",
            ["Status.SettingSpeed"]             = "Setting speed to: {0}%",
            ["Status.CurveSpeed"]               = "Set fan speed to {0}%, current temp: {1}掳C",
            ["Status.YieldedToSystem"]          = "Control yielded to system 鈥?temperature outside range.",

            // 鈹€鈹€ 椋庢墖鏇茬嚎 鈹€鈹€
            ["Curve.Reset"]                     = "Reset",
            ["Curve.SetCurve"]                  = "Set",
            ["Curve.ResetTooltip"]              = "Reset curve points",
            ["Curve.SetTooltip"]                = "Set fan curve from text",
            ["Curve.GraphTooltip"]              = "Fan Curve Graph",
            ["Curve.PointTooltip"]              = "Temp: {0}掳C  |  Fan: {1}%",
            ["Curve.MaxPointsReached"]          = "Maximum number of points (20) reached.",

            // 鈹€鈹€ 楂樼骇璁剧疆 鈹€鈹€
            ["Advanced.Group"]                  = "Advanced",
            ["Advanced.Hysteresis"]             = "Hysteresis:",
            ["Advanced.HysteresisUnit"]         = "s",
            ["Advanced.UpdateSpeed"]            = "Update Speed:",
            ["Advanced.UpdateSpeedUnit"]        = "ms",

            // 鈹€鈹€ 绯荤粺鎵樼洏 鈹€鈹€
            ["Tray.Open"]                       = "Open",
            ["Tray.Close"]                      = "Close",
            ["Tray.Off"]                        = "ASUS Fan Control - Off",
            ["Tray.Manual"]                     = "Fan: {0}%  |  Temp: {1}掳C",

            // 鈹€鈹€ 寮圭獥 / 娑堟伅 鈹€鈹€
            ["Dialog.SaveSuccess"]              = "Save successful.",
            ["Dialog.About.Title"]              = "About ASUS Fan Control Enhanced",
            ["Dialog.About.Text"]               = "ASUS Fan Control Enhanced\n\nVersion: {0}\n\nAuthor: Darren80\nImproved UI by community\n\nhttps://github.com/Darren80/AsusFanControlEnhanced",
            ["Dialog.Error.Title"]              = "Unhandled Exception - ASUS Fan Control Enhanced",
            ["Dialog.Error.Message"]            = "An error occurred, please try restarting the application.\n\nIf the error persists, create a GitHub issue at:\nhttps://github.com/Darren80/AsusFanControlEnhanced/issues\n\nStack Trace:\n{1}\n\nApplication Settings:\n{2}\n\nError Message:\n{0}\n\n",
            ["Dialog.Error.Restart"]            = "Restart",
            ["Dialog.Error.Exit"]               = "Exit",

            // 鈹€鈹€ 閿欒娑堟伅 鈹€鈹€
            ["Error.TempOutOfRange"]            = "CPU temperature is outside safe range at {0}掳C.\nTry restarting the application: Options 鈫?Restart application",
            ["Error.InvalidCurveChars"]         = "The fan curve string contains invalid characters.\nAllowed: 0-9, comma, dash.",
            ["Error.InvalidCurveFormat"]        = "Invalid curve format. Expected: temp,fanSpeed-temp,fanSpeed ...",
            ["Error.TempOutOfRangeCurve"]       = "Temperature {0}掳C is out of range ({1}鈥搟2}掳C).",
            ["Error.FanOutOfRangeCurve"]        = "Fan speed {0}% is out of range ({1}鈥搟2}%).",
            ["Error.CurveParse"]                = "An error occurred while parsing the fan curve points:\n\n{0}",
        };

        // 鈹€鈹€ 绠€浣撲腑鏂?鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€鈹€
        private static readonly Dictionary<string, string> Chinese = new Dictionary<string, string>
        {
            ["App.Title"]                       = "鍗庣椋庢墖鎺у埗澧炲己鐗?,

            ["Menu.Advanced"]                   = "楂樼骇",
            ["Menu.Advanced.ForbidUnsafe"]      = "绂佹涓嶅畨鍏ㄨ缃?,
            ["Menu.Advanced.AllowTextCurve"]    = "鍏佽閫氳繃鏂囨湰璁剧疆椋庢墖鏇茬嚎",
            ["Menu.Options"]                    = "閫夐」",
            ["Menu.Options.StartupSettings"]    = "鍚姩璁剧疆",
            ["Menu.Options.StartMinimised"]     = "鍚姩鏃舵渶灏忓寲",
            ["Menu.Options.StartWithWindows"]   = "寮€鏈鸿嚜鍚?,
            ["Menu.Options.Restart"]            = "閲嶅惎绋嬪簭",
            ["Menu.Options.ResetDefaults"]      = "鎭㈠榛樿璁剧疆",
            ["Menu.Language"]                   = "璇█",
            ["Menu.Language.English"]           = "English",
            ["Menu.Language.Chinese"]           = "绠€浣撲腑鏂?,
            ["Menu.Theme"]                      = "涓婚",
            ["Menu.Theme.Light"]                = "娴呰壊",
            ["Menu.Theme.Dark"]                 = "娣辫壊",
            ["Menu.CheckUpdates"]               = "妫€鏌ユ洿鏂?,
            ["Menu.About"]                      = "鍏充簬",

            ["Fan.TurnOff"]                     = "鍏抽棴椋庢墖鎺у埗",
            ["Fan.TurnOnControl"]               = "鎵嬪姩椋庢墖鎺у埗",
            ["Fan.TurnOnCurve"]                 = "椋庢墖鏇茬嚎鎺у埗",
            ["Fan.CurrentValue"]                = "褰撳墠鍊硷細",
            ["Fan.TurnedOff"]                   = "宸插叧闂?,
            ["Fan.PWM"]                         = "% (PWM 椋庢墖)",
            ["Fan.PercentLabel"]                = "% 椋庢墖",
            ["Fan.Temperature"]                 = "娓╁害 (掳C)",
            ["Fan.FanSpeed"]                    = "椋庢墖杞€?(%)",

            ["Status.CurrentRPM"]               = "褰撳墠杞€燂細",
            ["Status.CurrentCPUTemp"]           = "褰撳墠 CPU 娓╁害锛?,
            ["Status.SettingSpeed"]             = "姝ｅ湪璁剧疆杞€燂細{0}%",
            ["Status.CurveSpeed"]               = "椋庢墖杞€熷凡璁句负 {0}%锛屽綋鍓嶆俯搴︼細{1}掳C",
            ["Status.YieldedToSystem"]          = "娓╁害瓒呭嚭鑼冨洿锛屽凡浜よ繕绯荤粺鎺у埗銆?,

            ["Curve.Reset"]                     = "閲嶇疆",
            ["Curve.SetCurve"]                  = "璁剧疆",
            ["Curve.ResetTooltip"]              = "閲嶇疆鏇茬嚎鎺у埗鐐?,
            ["Curve.SetTooltip"]                = "浠庢枃鏈缃鎵囨洸绾?,
            ["Curve.GraphTooltip"]              = "椋庢墖鏇茬嚎鍥?,
            ["Curve.PointTooltip"]              = "娓╁害锛歿0}掳C  |  椋庢墖锛歿1}%",
            ["Curve.MaxPointsReached"]          = "宸茶揪鍒版渶澶ф帶鍒剁偣鏁?(20)銆?,

            ["Advanced.Group"]                  = "楂樼骇璁剧疆",
            ["Advanced.Hysteresis"]             = "婊炲悗锛?,
            ["Advanced.HysteresisUnit"]         = "绉?,
            ["Advanced.UpdateSpeed"]            = "鏇存柊閫熷害锛?,
            ["Advanced.UpdateSpeedUnit"]        = "姣",

            ["Tray.Open"]                       = "鎵撳紑",
            ["Tray.Close"]                      = "閫€鍑?,
            ["Tray.Off"]                        = "鍗庣椋庢墖鎺у埗 - 宸插叧闂?,
            ["Tray.Manual"]                     = "椋庢墖锛歿0}%  |  娓╁害锛歿1}掳C",

            ["Dialog.SaveSuccess"]              = "淇濆瓨鎴愬姛銆?,
            ["Dialog.About.Title"]              = "鍏充簬 鍗庣椋庢墖鎺у埗澧炲己鐗?,
            ["Dialog.About.Text"]               = "鍗庣椋庢墖鎺у埗澧炲己鐗圽n\n鐗堟湰锛歿0}\n\n浣滆€咃細Darren80\n绀惧尯 UI 浼樺寲鐗圽n\nhttps://github.com/Darren80/AsusFanControlEnhanced",
            ["Dialog.Error.Title"]              = "鏈鐞嗗紓甯?- 鍗庣椋庢墖鎺у埗澧炲己鐗?,
            ["Dialog.Error.Message"]            = "绋嬪簭鍙戠敓閿欒锛岃灏濊瘯閲嶅惎銆俓n\n濡傛灉闂鎸佺画瀛樺湪锛岃鍦?GitHub 鎻愪氦 Issue锛歕nhttps://github.com/Darren80/AsusFanControlEnhanced/issues\n\n鍫嗘爤璺熻釜锛歕n{1}\n\n绋嬪簭璁剧疆锛歕n{2}\n\n閿欒淇℃伅锛歕n{0}\n\n",
            ["Dialog.Error.Restart"]            = "閲嶅惎",
            ["Dialog.Error.Exit"]               = "閫€鍑?,

            ["Error.TempOutOfRange"]            = "CPU 娓╁害瓒呭嚭瀹夊叏鑼冨洿 ({0}掳C)銆俓n璇峰皾璇曢噸鍚▼搴忥細閫夐」 鈫?閲嶅惎绋嬪簭",
            ["Error.InvalidCurveChars"]         = "椋庢墖鏇茬嚎瀛楃涓插寘鍚棤鏁堝瓧绗︺€俓n鍏佽鐨勫瓧绗︼細鏁板瓧 0-9銆侀€楀彿銆佺煭妯嚎銆?,
            ["Error.InvalidCurveFormat"]        = "鏃犳晥鐨勬洸绾挎牸寮忋€傛湡鏈涙牸寮忥細娓╁害,杞€?娓╁害,杞€?...",
            ["Error.TempOutOfRangeCurve"]       = "娓╁害 {0}掳C 瓒呭嚭鑼冨洿 ({1}鈥搟2}掳C)銆?,
            ["Error.FanOutOfRangeCurve"]        = "椋庢墖杞€?{0}% 瓒呭嚭鑼冨洿 ({1}鈥搟2}%)銆?,
            ["Error.CurveParse"]                = "瑙ｆ瀽椋庢墖鏇茬嚎鐐规椂鍑洪敊锛歕n\n{0}",
        };

        // 鈹€鈹€ 瀛楀吀琛?鈹€鈹€
        private static readonly Dictionary<string, Dictionary<string, string>> AllStrings =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["en"] = English,
                ["zh-CN"] = Chinese,
            };

        /// <summary>
        /// 鎸?key 鑾峰彇鏈湴鍖栧瓧绗︿覆
        /// </summary>
        public static string Get(string key)
        {
            if (AllStrings.TryGetValue(CurrentLanguage, out var langDict)
                && langDict.TryGetValue(key, out var value))
                return value;
            // 鍥為€€鍒拌嫳鏂?            if (CurrentLanguage != "en"
                && AllStrings.TryGetValue("en", out var enDict)
                && enDict.TryGetValue(key, out var enValue))
                return enValue;
            return $"[[{key}]]"; // 缂哄け鏍囪
        }

        /// <summary>
        /// 鑾峰彇鏍煎紡鍖栧悗鐨勬湰鍦板寲瀛楃涓?        /// </summary>
        public static string Get(string key, params object[] args)
        {
            return string.Format(Get(key), args);
        }

        /// <summary>
        /// 鍒囨崲璇█
        /// </summary>
        public static void SetLanguage(string langCode)
        {
            if (!AllStrings.ContainsKey(langCode)) return;
            if (CurrentLanguage == langCode) return;

            CurrentLanguage = langCode;
            Properties.Settings.Default.uiLanguage = langCode;
            Properties.Settings.Default.Save();
            LanguageChanged?.Invoke();
        }

        /// <summary>
        /// 鍒濆鍖栨椂浠庤缃仮澶嶈瑷€
        /// </summary>
        public static void Initialize()
        {
            string saved = Properties.Settings.Default.uiLanguage;
            if (!string.IsNullOrEmpty(saved) && AllStrings.ContainsKey(saved))
                CurrentLanguage = saved;
            else
                CurrentLanguage = "en";
        }

        /// <summary>
        /// 涓?Form 鍙婂叾鎵€鏈夊瓙鎺т欢閫掑綊搴旂敤鏈湴鍖栨枃鏈€?        /// 璋冪敤鏂瑰湪 LanguageChanged 浜嬩欢涓皟鐢ㄦ鏂规硶銆?        /// </summary>
        public static void ApplyToForm(Form form)
        {
            form.Text = Get("App.Title");
            ApplyToControl(form);
        }

        private static void ApplyToControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                ApplyControlText(ctrl);

                // 閫掑綊瀛愭帶浠?                if (ctrl is MenuStrip menu)
                    ApplyToMenuStrip(menu);
                else if (ctrl is ContextMenuStrip ctx)
                    ApplyToContextMenu(ctx);
                else if (ctrl.Controls.Count > 0)
                    ApplyToControl(ctrl);
            }
        }

        private static void ApplyControlText(Control ctrl)
        {
            // 閫氳繃 Tag 鎴?Name 鏉ュ尮閰嶁€斺€斾紭鍏堢敤 Name
            // 杩欓噷閲囩敤闈欐€佹槧灏勮〃
            if (_controlMap.TryGetValue(ctrl.Name ?? "", out var key))
                ctrl.Text = Get(key);
        }

        private static void ApplyToMenuStrip(MenuStrip menu)
        {
            foreach (ToolStripItem item in menu.Items)
                ApplyToMenuItem(item);
        }

        private static void ApplyToContextMenu(ContextMenuStrip menu)
        {
            foreach (ToolStripItem item in menu.Items)
                ApplyToMenuItem(item);
        }

        private static void ApplyToMenuItem(ToolStripItem item)
        {
            if (_controlMap.TryGetValue(item.Name ?? "", out var key))
                item.Text = Get(key);

            if (item is ToolStripMenuItem dropDown && dropDown.DropDownItems.Count > 0)
            {
                foreach (ToolStripItem child in dropDown.DropDownItems)
                    ApplyToMenuItem(child);
            }
        }

        // 鈹€鈹€ 鎺т欢 Name 鈫?鏈湴鍖?key 鏄犲皠 鈹€鈹€
        private static readonly Dictionary<string, string> _controlMap = new Dictionary<string, string>
        {
            // 鑿滃崟
            ["toolStripMenuItem1"]                  = "Menu.Advanced",
            ["toolStripMenuItemForbidUnsafeSettings"] = "Menu.Advanced.ForbidUnsafe",
            ["allowFanCurveSettingViaTextToolStripMenuItem"] = "Menu.Advanced.AllowTextCurve",
            ["optionsToolStripMenuItem"]            = "Menu.Options",
            ["startupSettingsToolStripMenuItem"]    = "Menu.Options.StartupSettings",
            ["startMinimisedToolStripMenuItem"]     = "Menu.Options.StartMinimised",
            ["startWithWindowsToolStripMenuItem"]   = "Menu.Options.StartWithWindows",
            ["restartApplicationToolStripMenuItem"]  = "Menu.Options.Restart",
            ["resetToDefaultsToolStripMenuItem"]    = "Menu.Options.ResetDefaults",
            ["languageToolStripMenuItem"]           = "Menu.Language",
            ["englishToolStripMenuItem"]            = "Menu.Language.English",
            ["chineseToolStripMenuItem"]            = "Menu.Language.Chinese",
            ["themeToolStripMenuItem"]              = "Menu.Theme",
            ["lightThemeToolStripMenuItem"]         = "Menu.Theme.Light",
            ["darkThemeToolStripMenuItem"]          = "Menu.Theme.Dark",
            ["toolStripMenuItemCheckForUpdates"]    = "Menu.CheckUpdates",
            ["aboutToolStripMenuItem"]              = "Menu.About",

            // 椋庢墖鎺у埗
            ["radioButton1"]                        = "Fan.TurnOff",
            ["fanControlRadioButton"]               = "Fan.TurnOnControl",
            ["fanCurveRadioButton"]                 = "Fan.TurnOnCurve",
            ["label1"]                              = "Fan.CurrentValue",
            ["label2"]                              = "Status.CurrentRPM",
            ["label4"]                              = "Status.CurrentCPUTemp",

            // 楂樼骇璁剧疆
            ["groupBox2"]                           = "Advanced.Group",
            ["label6"]                              = "Advanced.Hysteresis",
            ["label9"]                              = "Advanced.HysteresisUnit",
            ["label7"]                              = "Advanced.UpdateSpeed",
            ["label8"]                              = "Advanced.UpdateSpeedUnit",

            // 鏇茬嚎鎸夐挳
            ["ResetCurvePoints"]                    = "Curve.Reset",
            ["button3"]                             = "Curve.SetCurve",

            // 鎵樼洏
            ["openToolStripMenuItem"]               = "Tray.Open",
            ["closeToolStripMenuItem"]              = "Tray.Close",
        };
    }
}
