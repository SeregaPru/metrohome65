// MicronSys and Dev 2012 year

using System.Drawing;
using System.Collections.Generic;
using Microsoft.Win32;
using Fleux.Controls;
using Fleux.Core.Scaling;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("WP Operator Name")]
    public class WPOperatorName : ShortcutWidget, IActive
    {
        private ThreadTimer _updateTimer;

        private string _oldOperatorName = "WP Operator Name";

        protected override Size[] GetSizes()
        {
            return new[]
                {
                    new Size(2, 1), 
                    new Size(2, 2), 
                    new Size(4, 1), 
                    new Size(4, 2)
                };
        }

        public bool UpdateStatus()
        {
            var name = "";
            var hwKey = Registry.LocalMachine.OpenSubKey("System\\State\\Phone", false);
            if (hwKey != null)
            {
                name = (string)hwKey.GetValue("Current Operator Name", "Not");
                hwKey.Close();
            }
            else
            {
                name = "";
            }

            if (_oldOperatorName != name)
            {
                _oldOperatorName = name; 
                return true;
            }

            return false;
        }


        public bool Active
        {
            get { return (_updateTimer != null); }
            set
            {
                if (value)
                {
                    if (_updateTimer == null) _updateTimer = new ThreadTimer(2000, () =>
                        {
                            if (UpdateStatus()) ForceUpdate();
                        });
                }
                else
                {
                    if (_updateTimer != null) _updateTimer.Stop();
                    _updateTimer = null;
                }
            }
        }

        // overriding paint icon method - don't paint icon
        protected override void PaintIcon(Graphics g, Rectangle rect)
        { }

        // overriding paint caption method - don't paint caption
        protected override void PaintCaption(Graphics g, Rectangle rect)
        {
            Caption = _oldOperatorName;
            base.PaintCaption(g, rect);
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);

            // hide control for icon / caption selection
            foreach (var control in controls)
                if (control.Name.Contains("Icon"))
                {
                    controls.Remove(control);
                    break;
                }
            foreach (var control in controls)
                if (control.Name.Contains("Caption"))
                {
                    controls.Remove(control);
                    break;
                }
           
            return controls;
        }
        
    }

}
