using System;
using System.Collections;
using System.Reflection;
using System.Linq;

using MetroHome65.Widgets;

namespace MetroHome65.Pages
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
            //!! read plugins from current assembly.
            //!! in future get from external plugins
            Assembly assembly = Assembly.GetExecutingAssembly();

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
