using System;
using System.Drawing;
using FolderWidget;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.Widgets
{
    [TileInfo("Folder")]
    public class FolderWidget : IconWidget
    {
        private string _folderGuid;

        protected override Size[] GetSizes()
        {
            return new Size[] { 
                new Size(1, 1),
                new Size(2, 2), 
                new Size(4, 2), 
            };
        }

        /// <summary>
        /// unique auto-generated GUID for folder
        /// </summary>
        [TileParameter]
        public string FolderGuid
        {
            get
            {
                if (String.IsNullOrEmpty(_folderGuid))
                    _folderGuid = Guid.NewGuid().ToString();
                return _folderGuid;
            }
            set { _folderGuid = value; }
        }


        // launch external application - play exit animation
        protected override bool GetDoExitAnimation() { return true; }

        public override bool OnClick(Point location)
        {
            var hubPage = new HubPage(_folderGuid);

            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new ShowPageMessage(hubPage));

            return true;
        }

    }

}
