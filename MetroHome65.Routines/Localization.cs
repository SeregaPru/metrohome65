using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;


namespace MetroHome65.Routines
{
    public static class Extensions
    {
        public static string Localize(this string text)
        {
            return Localizer.Instance.GetText(text);
        }
    }


    internal class Localizer
    {
        /// <summary>
        /// two-letter language ident
        /// </summary>
        private string _lang;

        /// <summary>
        /// translation dictionary
        /// key - english string
        /// value - translated string
        /// </summary>
        private readonly Dictionary<string, string> _strings = new Dictionary<string, string>();

        // create singleton
        private static readonly Localizer _instance = new Localizer();

        /// <summary>
        /// Get instance of singleton.
        /// </summary>
        internal static Localizer Instance
        {
            get { return _instance; }
        }

        static Localizer()
        { }

        private Localizer()
        {
            _lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            ReadLangStrings();
        }

        /// <summary>
        /// Read from file translated strings for current language
        /// </summary>
        private void ReadLangStrings()
        {
            var fileName = "metrohome65.lang." + _lang;

            if (!System.IO.File.Exists(fileName)) return;

            using (var reader = new System.IO.StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#")) return;

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
