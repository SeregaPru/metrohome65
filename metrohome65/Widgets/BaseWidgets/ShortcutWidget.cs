using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MetroHome65.Pages;

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

                    // when CommandLine changed, we have to change Caption and icon
                    Caption = Path.GetFileNameWithoutExtension(_CommandLine);
                    IconPath = "";
                }
            }
        }

        protected override Size[] GetSizes()
        {
            Size[] sizes = new Size[] { 
                new Size(1, 1), 
                new Size(2, 2) 
            };
            return sizes;
        }

        protected override void PaintIcon(Graphics g, Rectangle Rect)
        {
            // if image is set - draw image from specified file
            if (IconPath != "")
                base.PaintIcon(g, Rect);
            else
            {
                // else get icon from file
                String fname = CommandLine;
                Routines.structa refa = new Routines.structa();
                IntPtr ptr = Routines.SHGetFileInfo(ref fname, 0, ref refa, Marshal.SizeOf(refa), 0x100);
                Icon icon = Icon.FromHandle(refa.a);

                g.DrawIcon(icon, (Rect.Left + Rect.Right - icon.Width) / 2, (Rect.Top + Rect.Bottom - icon.Height - 10) / 2);

                icon.Dispose();
                icon = null;
            }
        }

        public override void OnClick(Point Location)
        {
            if (CommandLine != "")
                MetroHome65.Routines.StartProcess(CommandLine);
        }

        public override Control[] EditControls
        {
            get
            {
                Control[] BaseControls = base.EditControls;
                Control[] Controls = new Control[BaseControls.Length + 1];
                for (int i = 0; i < BaseControls.Length; i++)
                    Controls[i] = BaseControls[i];                        

                Settings_file EditControl = new Settings_file();
                EditControl.Caption = "Application";
                EditControl.Value = CommandLine;
                EditControl.OnValueChanged += new Settings_file.ValueChangedHandler(EditControl_OnValueChanged);
                Controls[BaseControls.Length] = EditControl;

                return Controls;
            }
        }

        void EditControl_OnValueChanged(String Value)
        {
            CommandLine = Value;
        }

    }
}
