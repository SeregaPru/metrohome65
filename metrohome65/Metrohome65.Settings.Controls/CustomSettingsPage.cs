using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using Fleux.UIElements.Pivot;
using MetroHome65.Routines;

namespace Metrohome65.Settings.Controls
{
    public class CustomSettingsPage : FleuxControlPage, INotifyPropertyChanged
    {
        private Pivot _pivot;

        protected BindingManager BindingManager { get; private set; }

        
        public CustomSettingsPage() : base(true)
        {
            CreateControls();
        }

        private void CreateControls()
        {
            ScreenRoutines.CursorWait();
            try
            {
                var appBar = new ApplicationBar
                {
                    Size = new Size(Size.Width, 48 + 2 * 10),
                    Location = new Point(0, Size.Height - 48 - 2 * 10)
                };
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.ok.bmp"));
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource("Metrohome65.Settings.Controls.Images.cancel.bmp"));
                appBar.ButtonTap += OnAppBarButtonTap;
                Control.AddElement(appBar.AnimateHorizontalEntrance(false));

                BindingManager = new BindingManager();

                _pivot = new Pivot("SETTINGS") { Size = new Size(Size.Width, Size.Height - appBar.Size.Height) };

                Control.AddElement(_pivot);

                CreateSettingsControls();
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }


        protected void AddPage(UIElement page, string title)
        {
            var pivotItem = new PivotItem
                                {
                                    Title = title, 
                                    Body = page,
                                };
            
            _pivot.AddPivotItem(pivotItem);
        }

        protected UIElement Separator()
        {
            return new Canvas() {Size = new Size(10, 40),};
        }

        protected virtual void CreateSettingsControls()
        { }

        protected virtual void ApplySettings()
        { }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            if (e.ButtonID == 0) // ok button
            {
                ApplySettings();

                if (OnApplySettings != null)
                    OnApplySettings(this, new EventArgs());

                Close();
            }
            else
                // close button
                Close();
        }

        // event triggered when selected item changed
        public event EventHandler OnApplySettings;
        
        
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
