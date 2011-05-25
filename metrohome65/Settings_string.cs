using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Pages
{
    public partial class Settings_string : UserControl
    {
        private String _Value = "";

        public String Caption { set { lblCaption.Text = value; } }

        public String Value { get { return _Value; } set { _Value = value; } }


        public Settings_string()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            textValue.Text = "";
        }
    }
}
