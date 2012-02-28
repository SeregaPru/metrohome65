using System;
using System.Windows.Forms;
using MetroHome65.Settings.Controls;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen
{

    public partial class FrmMainSettings : Form
    {
        private MainSettings _settings;

        public FrmMainSettings(MainSettings mainSettings)
        {
            _settings = mainSettings;

            InitializeComponent();

            CreateControls();
        }
             
        public void CreateControls()
        {
            this.SuspendLayout();

            var bindingManager = new BindingManager();

            var ctrThemeColor = new Settings_color
            {
                Caption = "Theme color", Value = _settings.ThemeColorValue,
            };
            PlaceControl(ctrThemeColor);
            bindingManager.Bind(_settings, "ThemeColorValue", ctrThemeColor, "Value");

            var ctrThemeImage = new Settings_image
            {
                Caption = "Theme background", Value = _settings.ThemeImage,
            };
            PlaceControl(ctrThemeImage);
            bindingManager.Bind(_settings, "ThemeImage", ctrThemeImage, "Value");

            var ctrTileColor = new Settings_color
            {
                Caption = "Tile color", Value = _settings.TileColorValue,
            };
            PlaceControl(ctrTileColor);
            bindingManager.Bind(_settings, "TileColorValue", ctrTileColor, "Value");

            var ctrFontColor = new Settings_color
            {
                Caption = "List font color",
                Value = _settings.ListFontColorValue,
            };
            PlaceControl(ctrFontColor);
            bindingManager.Bind(_settings, "ListFontColorValue", ctrFontColor, "Value");

            this.ResumeLayout(false);
        }

        private void PlaceControl(Control control)
        {
            if (control == null) return;

            int controlTop = 0;
            foreach (Control ctrl in this.Controls)
                controlTop = Math.Max(controlTop, ctrl.Bottom);
                
            control.Parent = this;
            control.Top = controlTop;

            Controls.Add(control);
        }

        private void mnuApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}