using System;
using System.Windows.Forms;
using MobilePractices.OpenFileDialogEx;

namespace MetroHome65.Pages
{
    public partial class Settings_file : UserControl
    {
        private String _ExeFile = "";

        public String Caption { set { lblCaption.Text = value; } }

        public String Value { 
            get { return _ExeFile; } 
            set {
                if (_ExeFile != value)
                {
                    _ExeFile = value;
                    ValueChanged(_ExeFile);
                }
            } 
        }


        public Settings_file()
        {
            InitializeComponent();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            OpenFileDialogEx dialog = new OpenFileDialogEx();
            dialog.Filter = "*.exe;*.lnk";

            if (dialog.ShowDialog() == DialogResult.OK)
                Value = dialog.FileName;
        }


        public delegate void ValueChangedHandler(String Value);

        public event ValueChangedHandler OnValueChanged;

        public void ValueChanged(String Value)
        {
            if (OnValueChanged != null)
                OnValueChanged(Value);
        }


    }
}
