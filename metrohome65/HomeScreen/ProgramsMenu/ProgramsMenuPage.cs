using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    public sealed class ProgramsMenuPage : Canvas, IActive
    {
        private UIElement _programsSv;

        public ProgramsMenuPage()
        {
            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
            CreateList();
        }

        private void CreateList()
        {
            const int programsSvPos = 18 + 48 + 18;
            _programsSv = new ProgramsMenu()
            {
                Location = new Point(programsSvPos, 5),
                Size = new Size(ScreenConsts.ScreenWidth - programsSvPos, ScreenConsts.ScreenHeight - 5),
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
                if (_programsSv != null)
                    DeleteElement(_programsSv);
                CreateList();
            }
        }

    }
}
