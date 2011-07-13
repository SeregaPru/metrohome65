using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_string : UserControl, INotifyPropertyChanged
    {
        public String Caption { set { lblCaption.Text = value; } }

        public String Value {
            get { return textValue.Text; } 
            set {
                if (textValue.Text != value)
                {
                    textValue.Text = value;
                    NotifyPropertyChanged("Value");
                }
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

        private void textValue_TextChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Value");
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion

    }
}
