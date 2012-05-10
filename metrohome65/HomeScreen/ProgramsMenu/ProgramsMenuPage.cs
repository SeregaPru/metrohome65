using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    public sealed class ProgramsMenuPage : Canvas, IActive
    {
        private UIElement _programsSv;
        private const int PaddingTop = 98;

        public ProgramsMenuPage()
        {
            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
            CreateList();
        }

        private void CreateList()
        {
            _programsSv = new ProgramsMenu()
            {
                Location = new Point(PaddingTop, 0),
                Size = new Size(100, 100), // initial fake size
            };
            this.SizeChanged += (sender, args) => SetSize();

            AddElement(_programsSv);
        }

        private void SetSize()
        {
            _programsSv.Size = new Size(this.Size.Width - PaddingTop, this.Size.Height);
        }

        // IActive
        public Boolean Active
        {
            get { return true; }
            set
            {
                if (!value)
                {
                    // stop scroll animation
                    _programsSv.Pressed(new Point(-1, -1));
                }
            }
        }

        /// <summary>
        /// re-create programs menu with new theme setings
        /// </summary>
        /// <param name="e"></param>
        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "PhoneAccentBrush") || (e.PropertyName == "PhoneForegroundBrush"))
            {
                Clear();
                CreateList();
                SetSize();
            }
        }

    }
}
