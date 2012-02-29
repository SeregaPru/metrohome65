using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    public sealed class ProgramsMenuPage : Canvas, IActive
    {
        private UIElement _programsSv;

        public ProgramsMenuPage(MainSettings mainSettings)
        {
            mainSettings.PropertyChanged += OnMainSettingsChanged;
            CreateList(mainSettings);
        }

        private void CreateList(MainSettings mainSettings)
        {
            const int programsSvPos = 18 + 48 + 18;
            _programsSv = new ProgramsMenu(mainSettings)
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            var mainSettings = sender as MainSettings;
            if (mainSettings == null) return;

            if ((e.PropertyName == "FontColor") || (e.PropertyName == "TileColor"))
            {
                if (_programsSv != null)
                    DeleteElement(_programsSv);
                CreateList(sender as MainSettings);
            }
        }

    }
}
