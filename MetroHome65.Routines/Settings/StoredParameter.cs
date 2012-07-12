using System;
using System.Xml.Serialization;

namespace MetroHome65.Routines.Settings
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
}