using System;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    public partial class Settings_file : UserControl
    {
        private String _ExeFile = "";

        public String Caption { set { lblCaption.Text = value; } }

        public String Value { get { return _ExeFile; } set { _ExeFile = value; } }


        public Settings_file()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                _ExeFile = openFileDialog1.FileName;
        }
    }
}
