using System.ComponentModel;
using System.Drawing;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core;
using Fleux.Styles;
using Fleux.UIElements;
using Fleux.UIElements.Events;
using Fleux.UIElements.Pivot;
using MetroHome65.Routines;
using MetroHome65.Routines.Screen;

namespace Metrohome65.Settings.Controls
{
    public class CustomSettingsPage<T> : FleuxControlPage, INotifyPropertyChanged
    {
        public delegate void ApplySettingsHandler(CustomSettingsPage<T> sender, T settings);


        #region Fields

        private Pivot _pivot;

        private T _settings;

        #endregion



        #region Properties

        public BindingManager BindingManager { get; private set; }

        // event triggered when selected item changed
        public event ApplySettingsHandler OnApplySettings;

        #endregion



        public T Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }


        public CustomSettingsPage(T settings) : base(true)
        {
            _settings = settings;

            CreateControls();
        }

        public CustomSettingsPage() : this(default(T)) { }

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
                appBar.AddButton(ResourceManager.Instance.GetBitmapFromEmbeddedResource(
                    (MetroTheme.PhoneBackgroundBrush == Color.White) ?
                        "Metrohome65.Settings.Controls.Images.back-light.bmp" : "Metrohome65.Settings.Controls.Images.back-dark.bmp"
                    ));
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

        protected virtual void CreateSettingsControls()
        { }

        protected virtual void ClearSettingsControls()
        {
            Control.RemoveElement(_pivot);
        }

        public override void Close()
        {
            ClearSettingsControls();
            base.Close();
        }

        protected virtual void ApplySettings()
        {
            ScreenRoutines.CursorWait();
            try
            {
                if (OnApplySettings != null)
                    OnApplySettings(this, Settings);
            }
            finally
            {
                ScreenRoutines.CursorNormal();
            }
        }

        private void OnAppBarButtonTap(object sender, ButtonTapEventArgs e)
        {
            ApplySettings();
            Close();
        }


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
