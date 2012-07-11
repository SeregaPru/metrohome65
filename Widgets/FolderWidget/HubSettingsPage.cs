using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

namespace FolderWidget
{
    public sealed class HubSettingsPage : CustomSettingsPage<HubSettings>
    {
        public HubSettingsPage(HubSettings settings) : base(settings) { }

        protected override void CreateSettingsControls()
        {
            AddPage(CreateThemeControls(), "Hub settings");
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
            
            // title
            var ctrTitle = new StringSettingsControl(this)
                               {
                                   Caption = "Hub title",
                                   Value = Settings.Title,
                               };
            stackPanel.AddElement(ctrTitle);
            BindingManager.Bind(Settings, "Title", ctrTitle, "Value");

            stackPanel.AddElement(new Separator());

            // hub background
            var ctrBackground = new ImageSettingsControl
            {
                Caption = "Background",
                Value = Settings.Background,
            };
            stackPanel.AddElement(ctrBackground);
            BindingManager.Bind(Settings, "Background", ctrBackground, "Value");

            stackPanel.AddElement(new Separator());

            // tiles grid top offset
            var ctrOffset = new StringSettingsControl(this)
            {
                Caption = "Tiles top offset",
                Value = Settings.Offset,
            };
            stackPanel.AddElement(ctrOffset);
            BindingManager.Bind(Settings, "Offset", ctrOffset, "Value");

            stackPanel.AddElement(new Separator());

            return scroller;
        }

    }
}
