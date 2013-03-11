using System;
using System.Collections.Generic;
using System.Globalization;
using MetroHome65.Routines.File;


namespace MetroHome65.Routines
{
    public static class Extensions
    {
        public static string Localize(this string text)
        {
            return Localizer.Instance.GetText(text);
        }

        public static void InitLocalization()
        {
            Localizer.Instance.Initialize();
        }
    }


    internal class Localizer
    {
        // create singleton
        private static readonly Localizer _instance = new Localizer();

        /// <summary>
        /// translation dictionary
        /// key - english string
        /// value - translated string
        /// </summary>
        private readonly Dictionary<string, string> _strings = new Dictionary<string, string>();

        /// <summary>
        /// Get instance of singleton.
        /// </summary>
        internal static Localizer Instance
        {
            get { return _instance; }
        }

        static Localizer()
        { }

        /// <summary>
        /// Read from file translated strings for current language
        /// </summary>
        internal void Initialize()
        {
            var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var fileName = FileRoutines.CoreDir + @"\metrohome65.lang." + lang;

            if (!System.IO.File.Exists(fileName)) return;

            using (var reader = new System.IO.StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line)) continue;

                    if (line.StartsWith("#")) continue;

                    var vals = line.Trim().Split('|');
                    if ((vals.Length == 2) && !String.IsNullOrEmpty(vals[1]))
                        _strings.Add(vals[0], vals[1]);
                }
            }
        }

        internal String GetText(string name)
        {
            return _strings.ContainsKey(name) ? _strings[name] : name;
        }
    }
}
