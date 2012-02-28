using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using MetroHome65.Routines;

namespace MetroHome65.Settings.Controls
{
    public partial class Settings_color : UserControl, INotifyPropertyChanged
    {
        private int _color = Color.Black.ToArgb();

        public Settings_color()
        {
            InitializeComponent();
        }

        private bool _changing = false;

        public string Caption
        {
            get { return lblColor.Text; }
            set { lblColor.Text = value;  }
        }

        public int Value 
        { 
            get { return _color; } 
            set {
                if (_color == value) return;

                _color = value;
                Color color = Color.FromArgb(_color);
                panelColorSample.BackColor = color;

                _changing = true;
                trackRed.Value = color.R;
                trackGreen.Value = color.G;
                trackBlue.Value = color.B;
                _changing = false;

                NotifyPropertyChanged("Value");
            } 
        }

        public XmlColor ColorValue
        {
            get { return Color.FromArgb(_color); } 
            set
            {
                if (_color == value.Color.ToArgb()) return;

                Value = value.Color.ToArgb();
                NotifyPropertyChanged("ColorValue");
            }
        }

        // change widget color
        private void trackBlue_ValueChanged(object sender, EventArgs e)
        {
            if (!_changing)
            {
                ColorValue = Color.FromArgb(trackRed.Value, trackGreen.Value, trackBlue.Value);
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
