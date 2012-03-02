using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;

namespace MetroHome65.HomeScreen
{
    public class Arrow : FlatButton
    {
        private const string NextImage = "next";
        private const string PrevImage = "back";

        public Arrow()
            : base(GetImageRes(NextImage))
        {
            Next();

            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
        }

        public void Next()
        {
            ResourceName = GetImageRes(NextImage);
            this.Size = new Size(48, 48);
        }

        public void Prev()
        {
            ResourceName = GetImageRes(PrevImage);
            this.Size = new Size(48, 48);
        }

        private static string GetImageRes(string resName)
        {
            return string.Format("MetroHome65.Images.{0}_{1}.png", resName,
                                 (MetroTheme.PhoneBackgroundBrush == Color.White) ? "light" : "dark");
        }

        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PhoneBackgroundBrush")
            {
                // update button image
                Next();
            }
        }


    }
}
