using System;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    public class HubPage : FleuxControlPage
    {

        public Canvas Content { get; set; }

        public Canvas Background { get; set; }


        public HubPage() : base(false)
        {
            ScreenRoutines.CursorWait();
            try
            {
                theForm.Menu = null;

                Control.ShadowedAnimationMode = Fleux.Controls.FleuxControl.ShadowedAnimationOptions.FromRight;

                this.Content = new Canvas
                {
                    Size = new Size(this.Size.Width, this.Size.Height),
                    Location = new Point(0, 0)
                };
                this.Control.AddElement(this.Content);

                this.Background = new Canvas { Size = this.Size };
                this.Control.AddElement(this.Background);

                var appBar = new ApplicationBar
                {
                    Size = new Size(Content.Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Content.Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("FolderWidget.Images.cancel.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Content.AddElement(appBar.AnimateHorizontalEntrance(false));

                var title = new TextElement("Folder hub")
                {
                    Style = MetroTheme.PhoneTextPageTitle2Style,
                    Location = new Point(24 - 3, 5) // -3 is a correction for Segoe fonts
                };
                Content.AddElement(title);

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
            
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // exit
                Close();
        }

    }
}
