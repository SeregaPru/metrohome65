using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using Fleux.UIElements.Pivot;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.HomeScreen.Settings
{
    public sealed class FrmMainSettings2 : FleuxControlPage
    {
        private readonly MainSettings _editSettings;

        private const int PaddingHor = 20;

        private BindingManager _bindingManager;
        private ApplicationBar _appBar;

        public FrmMainSettings2() : base(true)
        {
            _editSettings = MainSettings.Clone();

            CreateControls();
        }

        public void CreateControls()
        {
            _appBar = new ApplicationBar
            {
                Size = new Size(Size.Width, 48 + 2 * 10),
                Location = new Point(0, Size.Height - 48 - 2 * 10)
            };
            _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("MetroHome65.Images.ok.bmp"));
            _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("MetroHome65.Images.cancel.bmp"));
            _appBar.ButtonTap += OnAppBarButtonTap;
            Control.AddElement(_appBar.AnimateHorizontalEntrance(false));

            _bindingManager = new BindingManager();

            var pivot = new Pivot("SETTINGS") { Size = new Size(Size.Width, Size.Height - 150 - _appBar.Size.Height) };

            pivot.AddPivotItem(CreateThemeControls());
            pivot.AddPivotItem(CreateLockScreenControls());
            Control.AddElement(pivot);
        }

        private PivotItem CreateThemeControls()
        {
            var page = new PivotItem { Title = "theme", };
            var stackPanel = new StackPanel { Size = new Size(Size.Width - PaddingHor * 2, 1), };
            page.Body = stackPanel; 

            /*
            var scroller = new ScrollViewer
            {
                Content = stackPanel,
                Location = new Point(PaddingHor, 0),
                Size = new Size(this.Size.Width - PaddingHor, this.Size.Height - _appBar.Size.Height),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };
            */
            
            // intro
            var txtIntro =
                new TextElement(
                    "Change your phone's background and accent color to match your mood today, this week, or all month.")
                    {
                        Style = new TextStyle(MetroTheme.PhoneFontFamilyNormal, MetroTheme.PhoneFontSizeSmall, MetroTheme.PhoneForegroundBrush),
                        Size = new Size(stackPanel.Size.Width - 10, 50),
                        AutoSizeMode = TextElement.AutoSizeModeOptions.WrapText,
                    };
            stackPanel.AddElement(txtIntro);

            // vertival span
            stackPanel.AddElement(new TextElement(" "));

            // light/dark theme switcher
            var ctrThemeType = new FlagSettingsControl
                                   {
                                        Caption = "Dark",
                                        Value = _editSettings.ThemeIsDark,
                                    };
            stackPanel.AddElement(ctrThemeType);
            _bindingManager.Bind(_editSettings, "ThemeIsDark", ctrThemeType, "Value");

            // theme background
            var ctrThemeImage = new ImageSettingsControl
            {
                Caption = "Theme background",
                Value = _editSettings.ThemeImage,
            };
            stackPanel.AddElement(ctrThemeImage);
            _bindingManager.Bind(_editSettings, "ThemeImage", ctrThemeImage, "Value");

            return page;
        }

        private PivotItem CreateLockScreenControls()
        {
            var page = new PivotItem { Title = "lockscreen", };
            var stackPanel = new StackPanel();
            page.Body = stackPanel;
            
            // lock screen bg image
            var ctrLockScreenImage = new ImageSettingsControl
            {
                Caption = "Lock screen background",
                Value = _editSettings.LockScreenImage,
            };
            stackPanel.AddElement(ctrLockScreenImage);
            _bindingManager.Bind(_editSettings, "LockScreenImage", ctrLockScreenImage, "Value");

            return page;
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // ok
            {
                _editSettings.ApplyTheme();

                // write new settings to file
                (new MainSettingsProvider()).WriteSettings();
            }
            else
                Close();
        }


    }
}
