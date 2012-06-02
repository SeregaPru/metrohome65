using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;

namespace MetroHome65.Controls
{
    public class ThemedImageButton : FlatButton
    {
        private string _resourceName;

        public ThemedImageButton(string resourceName) : 
            base(GetImageRes(resourceName))
        {
            _resourceName = resourceName;
            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
        }

        protected static string GetImageRes(string resName)
        {
            return string.Format("MetroHome65.Images.{0}_{1}.png", resName,
                                 (MetroTheme.PhoneBackgroundBrush == Color.White) ? "light" : "dark");
        }

        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhoneBackgroundBrush")
            {
                // update button image
                ResourceName = GetImageRes(_resourceName);
            }
        }

    }
}
