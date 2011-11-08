using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using MetroHome65.Routines;
using MetroHome65.Settings.Controls;

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
            set {
                if (_CommandLine != value)
                {
                    _CommandLine = value;
                    NotifyPropertyChanged("CommandLine");
                }
            }
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2),
                new Size(4, 2)
            };
            return sizes;
        }

        public override bool OnClick(Point Location)
        {
            if (CommandLine != "")
                return FileRoutines.StartProcess(CommandLine);
            else
                return false;
        }

        public override List<Control> EditControls
        {
            get
            {
                List<Control> Controls = base.EditControls;

                Settings_file FileControl = new Settings_file();
                FileControl.Caption = "Application";
                FileControl.Value = CommandLine;
                Controls.Add(FileControl);

                BindingManager BindingManager = new BindingManager();
                BindingManager.Bind(this, "CommandLineForEdit", FileControl, "Value");

                return Controls;
            }
        }

        public String CommandLineForEdit
        {
            get { return _CommandLine; }
            set
            {
                CommandLine = value;

                // when CommandLine changed, we have to change Caption and icon
                IconPath = _CommandLine;
                Caption = Path.GetFileNameWithoutExtension(_CommandLine);
            }
        }

    }
}
