using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MetroHome65.HomeScreen
{
    [XmlType("param")]
    public class StoredParameter
    {
        [XmlAttribute]
        public String Name;

        [XmlAttribute]
        public String Value;

        public StoredParameter() { }

        public StoredParameter(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
    }


    /// <summary>
    /// struct for serialize Tile settings
    /// </summary>
    [Serializable]
    [XmlType("WidgetWrapper")]
    public class TileWrapperSettings
    {
        public List<StoredParameter> Parameters;
        public Point Location;
        public Size Size;

        [XmlAttribute("WidgetClass")]
        public String TileClassName;
    }
}
