using System.Drawing;
using Fleux.UIElements;
using MetroHome65.LockScreen;
using Metrohome65.Settings.Controls;

namespace MetroHome65.HomeScreen.Settings
{
    public sealed class MainSettingsForm : CustomSettingsPage<MainSettings>
    {
        public MainSettingsForm(MainSettings settings) : base(settings) { }

        protected override void CreateSettingsControls()
        {
            AddPage(CreateThemePage(), "theme");
            AddPage(CreateLockScreenPage(), "lockscreen");
        }

        private UIElement CreateThemePage()
        {
            var scroller = new SolidScrollViewer
            {
                Content = new MainSettingsPage(this),
                Location = new Point(SettingsConsts.PaddingHor, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };
            return scroller;
        }

        private UIElement CreateLockScreenPage()
        {
            var scroller = new SolidScrollViewer
            {
                Content = new LockScreenSettingsPage(this),
                Location = new Point(SettingsConsts.PaddingHor, 0),
                ShowScrollbars = true,
                HorizontalScroll = false,
                VerticalScroll = true,
            };
            return scroller;
        }

    }
}
