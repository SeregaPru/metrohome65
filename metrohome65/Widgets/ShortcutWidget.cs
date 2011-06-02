using System;
using System.Drawing;
using System.Windows.Forms;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Widget for applications launch.
    /// Looks like icon widget - icon with caption. 
    /// </summary>
    [WidgetInfo("Shortcut")]
    public class ShortcutWidget : IconWidget
    {
        private String _CommandLine = "";

        
        /// <summary>
        /// parameter "CommandLine" - relative or absolute path to application with parameters.
        /// </summary>
        [WidgetParameter]
        public String CommandLine
        {
            get { return _CommandLine; }
            set { _CommandLine = value; }
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        public override void OnClick(Point Location)
        {
            if (CommandLine != "")
                MetroHome65.Routines.StartProcess(CommandLine);
        }

    }
}
