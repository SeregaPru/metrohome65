using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using TinyIoC;

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
                Location = new Point(PaddingTop, 5),
                Size = new Size(ScreenConsts.ScreenWidth - PaddingTop, ScreenConsts.ScreenHeight - 5),
            };
            AddElement(_programsSv);
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
        /// e-create programs menu with new setings
        /// </summary>
        /// <param name="e"></param>
        private void OnThemeSettingsChanged(PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "PhoneAccentBrush") || (e.PropertyName == "PhoneForegroundBrush"))
            {
                Clear();
                CreateList();
            }
        }

    }
}
