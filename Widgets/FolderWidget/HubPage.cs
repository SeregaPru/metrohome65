using System;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines;
using MetroHome65.Routines.UIControls;

namespace FolderWidget
{
    public class HubPage : FleuxControlPage
    {

        public Canvas Content { get; set; }

        public ScaledBackground Background { get; set; }

        private ApplicationBar _appBar;

        public HubPage() : base(false)
        {
            ScreenRoutines.CursorWait();
            try
            {
                theForm.Menu = null;

                Control.ShadowedAnimationMode = Fleux.Controls.FleuxControl.ShadowedAnimationOptions.FromRight;

                this.Background = new ScaledBackground("") { Size = this.Size };
                this.Control.AddElement(this.Background);

                this.Content = new Canvas
                {
                    Size = new Size(this.Size.Width, this.Size.Height),
                    Location = new Point(0, 0)
                };
                this.Control.AddElement(this.Content);

                _appBar = new ApplicationBar
                {
                    Size = new Size(Content.Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Content.Size.Height - 48 - 2 * 10)
                };
                _appBar.ButtonTap += OnAppBarButtonTap;
                _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.back.bmp"));
                _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.add.bmp"));
                _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.settings.bmp"));
                _appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.del.bmp"));
                Content.AddElement(_appBar.AnimateHorizontalEntrance(false));

                var title = new TextElement("Folder hub")
                {
                    Style = MetroTheme.PhoneTextPageTitle2Style,
                    Location = new Point(24 - 3, 5) // -3 is a correction for Segoe fonts
                };
                Content.AddElement(title);

                var tileGrid = new HubPageTileGrid()
                                    {
                                        Location = new Point(0, 100),
                                        Size = new Size(Content.Size.Width, 300),
                                    };
                Content.AddElement(tileGrid);

                try
                {
                    ReadSettings();
                }
                catch (Exception) { }

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void ReadSettings()
        {
            Background.Image = FileRoutines.CoreDir + @"\wallpapers\leaves.jpg";

        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // exit
                Close();
        }

    }
}
