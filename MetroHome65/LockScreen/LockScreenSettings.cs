using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MetroHome65.Routines.Settings;

namespace MetroHome65.LockScreen
{
    /// <summary>
    /// struct for serialize lockscreen settings
    /// </summary>
    [Serializable]
    [XmlType("LockScreen")]
    public class LockScreenSettings
    {
        public List<StoredParameter> Parameters;

        [XmlAttribute("LockScreenClass")]
        public String LockScreenClass;
    }
}
