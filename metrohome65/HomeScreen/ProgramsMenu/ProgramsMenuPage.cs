using System;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen.ProgramsMenu
{
    public sealed class ProgramsMenuPage : Canvas, IActive
    {
        private readonly UIElement _programsSv;

        public ProgramsMenuPage(MainSettings mainSettings)
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

    }
}
