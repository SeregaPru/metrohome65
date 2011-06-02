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
        public String Caption { set { lblCaption.Text = value; } }

        public String Value {
            get { return textValue.Text; } 
            set { 
                textValue.Text = value; 
            } 
        }


        public Settings_string()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            Value = "";
        }


        public delegate void ValueChangedHandler(String Value);

        public event ValueChangedHandler OnValueChanged;

        private void textValue_TextChanged(object sender, EventArgs e)
        {
            ValueChanged(Value);
        }

        public void ValueChanged(String Value)
        {
            if (OnValueChanged != null)
                OnValueChanged(Value);
        }

    }
}
