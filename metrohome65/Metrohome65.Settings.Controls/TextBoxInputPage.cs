using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using MetroHome65.Routines;

namespace Metrohome65.Settings.Controls
{
    public class TextBoxInputPage : FleuxControlPage
    {

        public TextBoxInputPage() : base(true)
        {
            CreateControls();
        }

        private void CreateControls()
        {
            ScreenRoutines.CursorWait();
            try
            {
                Control.ShadowedAnimationMode = FleuxControl.ShadowedAnimationOptions.FromRight;

                var appBar = new ApplicationBar
                {
                    Size = new Size(Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.ok.bmp"));
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.cancel.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Control.AddElement(appBar.AnimateHorizontalEntrance(false));

            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // ok button
            {
                ApplySettings();

                Close();
            }
            else
                // close button
                Close();
        }

        private void ApplySettings()
        {
            //
        }
    }
}