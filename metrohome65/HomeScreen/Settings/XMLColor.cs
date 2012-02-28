using System;
using System.Drawing;

namespace MetroHome65.HomeScreen.Settings
{
    public class XmlColor : System.Xml.Serialization.IXmlSerializable
    {
        #region Private Properties

        private Color _color;

        #endregion

        #region Public Properties

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        #endregion

        #region Constructor

        public XmlColor() : this(Color.Black)
        { }

        public XmlColor(Color color)
        {
            _color = color;
        }

        public static implicit operator Color(XmlColor color)
        {
            return color._color;
        }
        public static implicit operator XmlColor(Color color)
        {
            return new XmlColor(color);
        }

        #endregion

        #region IXmlSerializable Members

        void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_color.ToArgb().ToString());
        }

        void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            try
            {
                _color = Color.FromArgb(Convert.ToInt32(reader.ReadString()));
            }
            catch
            {
                _color = Color.Black;
            }
            reader.ReadEndElement();
        }

        System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
        {
            return null;
        }

        #endregion // IXmlSerializable Members
    }
}
