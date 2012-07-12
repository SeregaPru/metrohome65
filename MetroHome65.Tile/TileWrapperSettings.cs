using System;
using System.Drawing;
using System.Collections.Generic;
using System.Xml.Serialization;
using MetroHome65.Routines.Settings;

namespace MetroHome65.Tile
{
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
