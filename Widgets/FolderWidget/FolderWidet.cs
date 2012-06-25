using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.UIElements;
using FolderWidget;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Widgets
{
    [TileInfo("Folder")]
    public class FolderWidget : IconWidget
    {

        public FolderWidget() 
        {
        }


        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(1, 1),
                new Size(2, 2), 
                new Size(4, 2), 
            };
        }

        // launch external application - play exit animation
        protected override bool GetDoExitAnimation() { return true; }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();
           
            return controls;
        }

        public override bool OnClick(Point location)
        {
            var hubPage = new HubPage();

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new ShowPageMessage(hubPage));

            return true;
        }

    }

}
