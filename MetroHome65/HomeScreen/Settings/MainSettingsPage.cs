using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

namespace MetroHome65.HomeScreen.Settings
{
    public class MainSettingsPage: StackPanel, INotifyPropertyChanged
    {
        private readonly CustomSettingsPage<MainSettings> _page;

        // internal wrapper for combobox selected index to boolean value
        public int ThemeIndex
        {
            get { return (_page.Settings.ThemeIsDark ? 0 : 1); }
            set { _page.Settings.ThemeIsDark = (value == 0); }
        }

        public Color AccentColor
        {
            get { return _page.Settings.AccentColor; }
            set { _page.Settings.AccentColor = value; }
        }

        public MainSettingsPage(CustomSettingsPage<MainSettings> page)
        {
            _page = page;
            _page.OnApplySettings += (sender, settings) => OnApplySettings(settings);

            //!!Size = new Size(Size.Width - SettingsConsts.PaddingHor * 2, 1);

            // intro
            var txtIntro =
                new TextElement(
                    "Change your phone's background and accent color to match your mood today, this week, or all month.")
                {
                    Size = new Size(this.Size.Width - 10, 50),
                    Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeSmall, MetroTheme.PhoneForegroundBrush),
                    AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                };
            this.AddElement(txtIntro);

            this.AddElement(new Separator());


            // light/dark theme switcher
            var ctrTheme = new SelectSettingsControl
            {
                Caption = "Theme",
                Items = new List<object> { "dark", "light" },
            };
            this.AddElement(ctrTheme);
            _page.BindingManager.Bind(this, "ThemeIndex", ctrTheme, "SelectedIndex", true);

            this.AddElement(new Separator());


            // accent coolor
            var ctrAccent = new ColorSettingsControl(false)
            {
                Caption = "Accent Color",
            };
            this.AddElement(ctrAccent);
            _page.BindingManager.Bind(this, "AccentColor", ctrAccent, "Value", true);

            this.AddElement(new Separator());


            // theme background
            var ctrThemeImage = new ImageSettingsControl
            {
                Caption = "Theme background",
            };
            this.AddElement(ctrThemeImage);
            _page.BindingManager.Bind(_page.Settings, "ThemeImage", ctrThemeImage, "Value", true);

            this.AddElement(new Separator());


            // tile screen style
            var ctrTileTheme = new SelectSettingsControl
            {
                Caption = "Tiles style",
                Items = new List<object> { "Windows Phone 7", "Windows 8" },
            };
            this.AddElement(ctrTileTheme);
            _page.BindingManager.Bind(_page.Settings, "TileThemeIndex", ctrTileTheme, "SelectedIndex", true);

            this.AddElement(new Separator());
        }


        private void OnApplySettings(MainSettings settings)
        {
            settings.ApplyTheme();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }
}