using System;
using System.IO;
using System.Xml.Serialization;

namespace MetroHome65.Routines.File
{
    public static class XmlHelper<T>
    {
        public static T Read(String fileName)
        {
            if (! System.IO.File.Exists(fileName)) return default(T);

            var settings = default(T);
            try
            {
                TextReader reader = new StreamReader(fileName);
                var serializer = new XmlSerializer(typeof(T));
                settings = (T)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Read XML error");
            }

            return settings;
        }

        public static void Write(T settings, String fileName)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                TextWriter writer = new StreamWriter(fileName, false);
                serializer.Serialize(writer, settings);
                writer.Close();
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.StackTrace, "Write XML error");
            }
        }
    }
}