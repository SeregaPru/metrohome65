using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using Fleux.Core.GraphicsHelpers;

namespace Fleux.Styles
{
    public class TextStyle : Style, INotifyPropertyChanged, System.Xml.Serialization.IXmlSerializable
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


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }



        #region IXmlSerializable Members

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();

            try
            {
                _fontSize = Convert.ToInt32(reader.ReadString());
            }
            catch
            {
                _fontSize = 8;
            }

            try
            {
                _fontFamily = reader.ReadString();
            }
            catch
            {
                _fontFamily = MetroTheme.PhoneFontFamilyNormal;
            }

            try
            {
                _foreground = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
            }
            catch
            {
                _foreground = MetroTheme.PhoneForegroundBrush;
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteString(_fontSize.ToString(CultureInfo.InvariantCulture));
            writer.WriteString(_fontFamily.ToString(CultureInfo.InvariantCulture));
            writer.WriteString(_foreground.ToArgb().ToString(CultureInfo.InvariantCulture));
        }

        #endregion // IXmlSerializable Members

    }
}
