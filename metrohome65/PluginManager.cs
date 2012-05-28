using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;
using MetroHome65.Interfaces;

namespace MetroHome65.HomeScreen
{
    
    /// <summary>
    /// Class for managing plugins assemblies:
    /// load plugins at sartup, create plugin by type etc.
    /// </summary>
    public class PluginManager : IPluginManager
    {
        public PluginManager()
        {
            LoadPlugins();
        }
        
        private Hashtable _plugins = new Hashtable();

        public IEnumerable GetTileTypes()
        {
            return _plugins.Values;
        }

        /// <summary>
        /// reads all .dll in Plugins folder and scans for plugins with 
        /// ITile interface.
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
                    (type.GetInterfaces().Contains(typeof(ITile))) &&
                    (! type.IsAbstract))
                {
                    _plugins.Add(type.FullName, type);
                }
            }
        }

        public ITile CreateTile(String tileName)
        {
            var widgetType = (Type)_plugins[tileName];
            if (widgetType == null) return null;

            ITile tile = (ITile)Activator.CreateInstance(widgetType);
            return tile;
        }

    }

}
