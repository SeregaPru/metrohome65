using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MetroHome65
{
    public partial class Settings_color : UserControl
    {
        private Color _Color = Color.Black;

        public Settings_color()
        {
            InitializeComponent();
        }

        public Color Value { 
            get {
                return _Color;
            } 
            set {
                _Color = value;
                trackRed.Value = _Color.R;
                trackGreen.Value = _Color.G;
                trackBlue.Value = _Color.B;
            } 
        }

        // change widget color
        private void trackBlue_ValueChanged(object sender, EventArgs e)
        {
            _Color = Color.FromArgb(trackRed.Value, trackGreen.Value, trackBlue.Value);
            panelColorSample.BackColor = _Color;
        }

    }
}
