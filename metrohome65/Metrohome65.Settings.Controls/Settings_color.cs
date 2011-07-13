using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_color : UserControl, INotifyPropertyChanged
    {
        private int _Color = Color.Black.ToArgb();

        public Settings_color()
        {
            InitializeComponent();
        }

        private bool _changing = false;


        public int Value { 
            get { return _Color; } 
            set {
                if (_Color != value)
                {
                    _Color = value;
                    Color color = Color.FromArgb(_Color);
                    panelColorSample.BackColor = color;

                    _changing = true;
                    trackRed.Value = color.R;
                    trackGreen.Value = color.G;
                    trackBlue.Value = color.B;
                    _changing = false;

                    NotifyPropertyChanged("Value");
                }
            } 
        }


        // change widget color
        private void trackBlue_ValueChanged(object sender, EventArgs e)
        {
            if (!_changing)
            {
                Value = Color.FromArgb(trackRed.Value, trackGreen.Value, trackBlue.Value).ToArgb();
            }
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
