using System.Collections.Generic;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using Metrohome65.Settings.Controls;

namespace MetroHome65.HomeScreen.Settings
{
    public sealed class MainSettingsPage : CustomSettingsPage<MainSettings>
    {
        public MainSettingsPage(MainSettings settings) : base(settings) { }

        protected override void CreateSettingsControls()
        {
            AddPage(CreateThemeControls(), "theme");
            AddPage(CreateLockScreenControls(), "lockscreen");
        }

        private UIElement CreateThemeControls()
        {
            var stackPanel = new StackPanel { Size = new Size(Size.Width - SettingsConsts.PaddingHor * 2, 1), };

            var scroller = new ScrollViewer
            {
                Content = stackPanel,
                Location = new Point(SettingsConsts.PaddingHor, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };
            
            // intro
            var txtIntro =
                new TextElement(
                    "Change your phone's background and accent color to match your mood today, this week, or all month.")
                    {
                        Size = new Size(stackPanel.Size.Width - 10, 50),
                        Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeSmall, MetroTheme.PhoneForegroundBrush),
                        AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                    };
            stackPanel.AddElement(txtIntro);

            // vertival span
            stackPanel.AddElement(Separator());


            // light/dark theme switcher
            var ctrTheme = new SelectSettingsControl
            {
                Caption = "Theme",
                Items = new List<object> { "dark", "light" },
                SelectedIndex = this.ThemeIndex,
            };
            stackPanel.AddElement(ctrTheme);
            BindingManager.Bind(this, "ThemeIndex", ctrTheme, "SelectedIndex");

            stackPanel.AddElement(Separator());


            // accent coolor
            var ctrAccent = new ColorSettingsControl(false)
            {
                Caption = "Accent Color",
                Value = Settings.AccentColor,
            };
            stackPanel.AddElement(ctrAccent);
            BindingManager.Bind(this, "AccentColor", ctrAccent, "Value");

            stackPanel.AddElement(Separator());


            // theme background
            var ctrThemeImage = new ImageSettingsControl
            {
                Caption = "Theme background",
                Value = Settings.ThemeImage,
            };
            stackPanel.AddElement(ctrThemeImage);
            BindingManager.Bind(Settings, "ThemeImage", ctrThemeImage, "Value");

            return scroller;
        }

        private UIElement CreateLockScreenControls()
        {
            var stackPanel = new StackPanel();
            
            // lock screen bg image
            var ctrLockScreenImage = new ImageSettingsControl
            {
                Caption = "Lock screen background",
                Value = Settings.LockScreenImage,
            };
            stackPanel.AddElement(ctrLockScreenImage);
            BindingManager.Bind(Settings, "LockScreenImage", ctrLockScreenImage, "Value");

            return stackPanel;
        }

        // internal wrapper from combobox selectedindex to boolean value
        public int ThemeIndex
        {
            get { return (Settings.ThemeIsDark ? 0 : 1); }
            set { Settings.ThemeIsDark = (value == 0); }
        }

        public Color AccentColor
        {
            get { return Settings.AccentColor; }
            set { Settings.AccentColor = value; }
        }

    }
}
