using System;
using System.Windows.Forms;
using MetroHome65.Settings.Controls;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.Settings
{

    public partial class FrmMainSettings : Form
    {
        private readonly MainSettings _editSettings;

        public FrmMainSettings()
        {
            InitializeComponent();

            _editSettings = new MainSettings();
            CreateControls();
        }

        public void CreateControls()
        {
            this.SuspendLayout();

            var bindingManager = new BindingManager();

            var ctrThemeImage = new Settings_image
            {
                Caption = "Theme background", Value = _editSettings.ThemeImage,
            };
            PlaceControl(ctrThemeImage);
            bindingManager.Bind(_editSettings, "ThemeImage", ctrThemeImage, "Value");

            var ctrThemeType = new Settings_flag
            {
                Caption = "Dark theme", Value = _editSettings.ThemeIsDark,
            };
            PlaceControl(ctrThemeType);
            bindingManager.Bind(_editSettings, "ThemeIsDark", ctrThemeType, "Value");

            var ctrTileColor = new Settings_color
            {
                Caption = "Tile color", ColorValue = _editSettings.TileColor,
            };
            PlaceControl(ctrTileColor);
            bindingManager.Bind(_editSettings, "TileColor", ctrTileColor, "ColorValue");

            this.ResumeLayout(false);
        }

        private void PlaceControl(Control control)
        {
            if (control == null) return;

            var controlTop = 0;
            foreach (Control ctrl in Controls)
                controlTop = Math.Max(controlTop, ctrl.Bottom);
                
            control.Parent = this;
            control.Top = controlTop;

            Controls.Add(control);
        }

        private void MenuApplyClick(object sender, EventArgs e)
        {
            _editSettings.ApplyTheme();

            // write new settings to file
            (new MainSettingsProvider()).WriteSettings(_editSettings);

            DialogResult = DialogResult.OK;
        }

        private void MenuCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
 
    }
}