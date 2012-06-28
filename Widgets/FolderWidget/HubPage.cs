using System;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Core.Scaling;
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

        private static readonly int AppBarHeight = 48 + 2 * 10;

        public HubPage() : base(false)
        {
            ScreenRoutines.CursorWait();
            try
            {
                theForm.Menu = null;

                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                Background = new ScaledBackground("") { Size = this.Size };
                Control.AddElement(Background);

                Content = new Canvas
                {
                    Size = new Size(this.Size.Width, this.Size.Height),
                    Location = new Point(0, 0)
                };
                Control.AddElement(Content);

                _appBar = new ApplicationBar
                {
                    Size = new Size(Content.Size.Width, AppBarHeight),
                    Location = new Point(0, Content.Size.Height - AppBarHeight)
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
                    Location = new Point(24 - 3, 5), // -3 is a correction for Segoe fonts
                    AutoSizeMode = TextElement.AutoSizeModeOptions.OneLineAutoHeight,
                };
                title.ResizeForWidth(Content.Size.Width);
                Content.AddElement(title);

                var tileGrid = new HubPageTileGrid(Background, "", 4, 100)
                                    {
                                        Location = new Point(0, title.Bounds.Bottom + 50),
                                        Size = new Size(Content.Size.Width, Content.Size.Height - title.Bounds.Bottom - 50 - _appBar.Size.Height),
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
            Background.Image = FileRoutines.CoreDir + @"\wallpapers\games.jpg";
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // exit
                Close();
        }

    }
}
