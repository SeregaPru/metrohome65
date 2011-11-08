using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65
{
    public partial class FrmMainSettings : Form
    {
        private string _BgImagePath = "";

        public FrmMainSettings()
        {
            InitializeComponent();

            ControlBackground.Caption = "Background";
        }

        public string BgImagePath {
            get { return _BgImagePath; }
            set
            {
                if (value != _BgImagePath)
                {
                    value = _BgImagePath;
                    ControlBackground.Value = value;
                }
            }
        }

        private void menuApply_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}