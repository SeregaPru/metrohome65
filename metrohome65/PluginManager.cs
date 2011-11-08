using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using MetroHome65.Widgets;

namespace MetroHome65.HomeScreen
{
    /// <summary>
    /// Class for managing plugins assemblies:
    /// load plugins at sartup, create plugin by type etc.
    /// </summary>
    class PluginManager
    {
        static private PluginManager _instance = null;

        static public PluginManager GetInstance()
        {
            if (_instance == null)
                _instance = new PluginManager();
            return _instance;
        }

        public PluginManager()
        {
            LoadPlugins();
        }
        
        //!! todo потом сделать приватным а выставить итератор
        public Hashtable _plugins = new Hashtable();

        /// <summary>
        /// reads all .dll in Plugins folder and scans for plugins with 
        /// IWidget interface.
        /// Fill internal plugin map - plugin type by name.
        /// </summary>
        private void LoadPlugins()
        {
            DirectoryInfo FolderInfo = new DirectoryInfo(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase));
            FileInfo[] files = FolderInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                if ((! file.Name.ToLower().Contains("opennetcf")) && 
                    (file.Extension == ".dll"))
                    LoadPlugin(file.FullName);
            }
        }

        private void LoadPlugin(String PluginPath)
        {
            Assembly assembly = Assembly.LoadFrom(PluginPath);

            foreach (Type type in assembly.GetTypes())
            {
                if ((type.IsClass) && 
                    (type.GetInterfaces().Contains(typeof(IWidget))) &&
                    (! type.IsAbstract))
                {
                    _plugins.Add(type.FullName, type);
                }
            }
        }

        public IWidget CreateWidget(String WidgetName)
        {
            Type WidgetType = (Type)_plugins[WidgetName];
            if (WidgetType == null) return null;

            IWidget Widget = (IWidget)Activator.CreateInstance(WidgetType);
            return Widget;
        }

    }
}
