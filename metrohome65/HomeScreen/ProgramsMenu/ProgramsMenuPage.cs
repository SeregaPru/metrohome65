using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.Controls;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using TinyIoC;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    public sealed class ProgramsMenuPage : Canvas, IActive
    {
        private UIElement _programsSv;

        private TinyIoCContainer _container;

        public ProgramsMenuPage(TinyIoCContainer container)
        {
            _container = container;
            MetroTheme.PropertyChanged += OnThemeSettingsChanged;
            CreateList();
        }

        private void CreateList()
        {
            const int programsSvPos = 18 + 48 + 18;
            _programsSv = new ProgramsMenu(_container)
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
                Clear();
                CreateList();
            }
        }

    }
}
