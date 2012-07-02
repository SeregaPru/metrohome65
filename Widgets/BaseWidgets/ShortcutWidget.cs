using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using Metrohome65.Settings.Controls;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Widget for applications launch.
    /// Looks like icon widget - icon with caption. 
    /// </summary>
    [TileInfo("Shortcut")]
    public class ShortcutWidget : IconWidget
    {
        private String _commandLine = "";

        
        /// <summary>
        /// parameter "CommandLine" - relative or absolute path to application with parameters.
        /// </summary>
        [TileParameter]
        public String CommandLine
        {
            get { return _commandLine; }
            set {
                if (_commandLine != value)
                {
                    _commandLine = value;
                    NotifyPropertyChanged("CommandLine");
                }
            }
        }

        protected override Size[] GetSizes()
        {
            var sizes = new[] { 
                new Size(1, 1), 
                new Size(2, 2),
                new Size(4, 2)
            };
            return sizes;
        }

        public override bool OnClick(Point location)
        {
            return (CommandLine != "") && FileRoutines.StartProcess(CommandLine);
        }

        // launch external application - play exit animation
        protected override bool GetDoExitAnimation() { return true; }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();

            var fileControl = new FileSettingsControl(settingsPage)
                                  {
                                      Caption = "Application", 
                                      Value = CommandLine,
                                  };
            controls.Add(fileControl);
            bindingManager.Bind(this, "CommandLineForEdit", fileControl, "Value");

            /*
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
             */ 

            return controls;
        }

        public String CommandLineForEdit
        {
            get { return _commandLine; }
            set
            {
                CommandLine = value;

                // when CommandLine changed, we have to change Caption and icon
                IconPath = _commandLine;
                Caption = Path.GetFileNameWithoutExtension(_commandLine);
            }
        }

    }
}
