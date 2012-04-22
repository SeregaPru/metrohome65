using System;
using System.ComponentModel;
using System.Drawing;
using Fleux.UIElements;

namespace Metrohome65.Settings.Controls
{
    public sealed class FlagSettingsControl : Canvas, INotifyPropertyChanged
    {
        private readonly CheckBox _cbFlag;

        public String Caption { set { _cbFlag.Text = value; } }

        public Boolean Value {
            get { return _cbFlag.Checked; } 
            set {
                if (_cbFlag.Checked != value)
                {
                    _cbFlag.Checked = value;
                    NotifyPropertyChanged("Value");
                }
            } 
        }


        public FlagSettingsControl()
        {
            Size = new Size(450, 85);

            _cbFlag = new CheckBox("<flag parameter>")
                         {
                             Location = new Point(8, 20),
                             AutoSizeMode = CheckBox.AutoSizeModeOptions.WrapText,
                         };
            _cbFlag.CheckStateChanged += CbFlagCheckStateChanged;
            AddElement(_cbFlag);
        }

        private void CbFlagCheckStateChanged(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Value");
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion


    }
}
