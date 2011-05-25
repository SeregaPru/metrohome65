using System;
using System.Drawing;
using System.Windows.Forms;

namespace MetroHome65.Widgets
{
    /// <summary>
    /// Widget for applications launch.
    /// Looks like icon widget - icon with caption. 
    /// </summary>
    public class ShortcutWidget : IconWidget
    {
        private String _CommandLine = "";

        
        /// <summary>
        /// parameter "CommandLine" - relative or absolute path to application with parameters.
        /// </summary>
        [WidgetParameter(WidgetParameterEditType.edFile, "Application")]
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
                StartProcess(CommandLine);
        }

        private static void StartProcess(string FileName)
        {
            try
            {
                System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = FileName;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                //!! write to log  (e.StackTrace, "StartProcess")
            }
        }
    }
}
