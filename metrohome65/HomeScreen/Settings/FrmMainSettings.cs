using System;
using System.Windows.Forms;
using MetroHome65.Settings.Controls;
using MetroHome65.Routines;

namespace MetroHome65.HomeScreen.Settings
{

    public partial class FrmMainSettings : Form
    {
        private readonly MainSettings _editSettings;
        private readonly MainSettings _mainSettings;

        public FrmMainSettings(MainSettings mainSettings)
        {
            InitializeComponent();

            _mainSettings = mainSettings;
            _editSettings = new MainSettings();
            CopySettings(_mainSettings, _editSettings);
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

            var ctrThemeColor = new Settings_color
            {
                Caption = "Theme color", ColorValue = _editSettings.ThemeColor,
            };
            PlaceControl(ctrThemeColor);
            bindingManager.Bind(_editSettings, "ThemeColor", ctrThemeColor, "ColorValue");

            var ctrTileColor = new Settings_color
            {
                Caption = "Tile color", ColorValue = _editSettings.TileColor,
            };
            PlaceControl(ctrTileColor);
            bindingManager.Bind(_editSettings, "TileColor", ctrTileColor, "ColorValue");

            var ctrFontColor = new Settings_color
            {
                Caption = "Font color", ColorValue = _editSettings.FontColor,
            };
            PlaceControl(ctrFontColor);
            bindingManager.Bind(_editSettings, "FontColor", ctrFontColor, "ColorValue");

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
            CopySettings(_editSettings, _mainSettings);
            DialogResult = DialogResult.OK;
        }

        private void MenuCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void CopySettings(MainSettings src, MainSettings dst)
        {
            dst.ThemeImage = src.ThemeImage;
            dst.ThemeColor = src.ThemeColor;
            dst.FontColor = src.FontColor;
            dst.TileColor = src.TileColor;
        }
    }
}