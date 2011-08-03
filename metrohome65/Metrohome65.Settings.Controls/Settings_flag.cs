using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_flag : UserControl, INotifyPropertyChanged
    {
        public String Caption { set { cbFlag.Text = value; } }

        public Boolean Value {
            get { return cbFlag.Checked; } 
            set {
                if (cbFlag.Checked != value)
                {
                    cbFlag.Checked = value;
                    NotifyPropertyChanged("Value");
                }
            } 
        }


        public Settings_flag()
        {
            InitializeComponent();
        }

        private void cbFlag_CheckStateChanged(object sender, EventArgs e)
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
