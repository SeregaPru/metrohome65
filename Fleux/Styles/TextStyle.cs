using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using Fleux.Core.GraphicsHelpers;

namespace Fleux.Styles
{
    public class TextStyle : Style, INotifyPropertyChanged
    {
        private int _fontSize;
        private string _fontFamily;
        private Color _foreground;
        private ThicknessStyle _thickness;
        private bool _bold;

        public TextStyle(string fontFamily, int fontSize, Color foreGround, ThicknessStyle thickness)
        {
            this.FontSize = fontSize;
            this.FontFamily = fontFamily;
            this.Foreground = foreGround;
            this.Thickness = thickness;
        }

        public TextStyle(string fontFamily, int fontSize, Color foreGround)
            : this(fontFamily, fontSize, foreGround, null)
        {
        }

        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize == value) return;
                _fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily == value) return;
                _fontFamily = value;
                OnPropertyChanged("FontFamily");
            }
        }

        public Color Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground == value) return;
                _foreground = value;
                OnPropertyChanged("Foreground");
            }
        }

        public ThicknessStyle Thickness
        {
            get { return _thickness; }
            set
            {
                if (_thickness == value) return;
                _thickness = value;
                OnPropertyChanged("Thickness");
            }
        }

        public bool Bold
        {
            get { return _bold; }
            set
            {
                if (_bold == value) return;
                _bold = value;
                OnPropertyChanged("Bold");
            }
        }


        public void ApplyTo(IDrawingGraphics gr)
        {
            gr.FontName(this.FontFamily);
            gr.FontSize(this.FontSize);
            gr.Color(this.Foreground);
            gr.Bold(this.Bold);
        }


        public override string ToString()
        {
            return String.Format("{0};{1};{2}", 
                _fontFamily, _fontSize, _foreground.ToArgb().ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// special static method for parsing tile settings
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TextStyle FromString(string value)
        {
            try
            {
                string[] fontParams = value.Split(';');
                var fontName = (fontParams.Length >= 1) ? fontParams[0] : MetroTheme.PhoneFontFamilyNormal;
                var fontSize = (fontParams.Length >= 2) ? Convert.ToInt32(fontParams[1]) : 8;
                var fontColor = (fontParams.Length >= 3) ? Color.FromArgb(Convert.ToInt32(fontParams[2])) : Color.White;
                return new TextStyle(fontName, fontSize, fontColor);
            }
            catch(Exception)
            {
                return new TextStyle(MetroTheme.PhoneFontFamilyNormal, 8, Color.White);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
