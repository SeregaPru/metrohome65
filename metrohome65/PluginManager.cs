using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.IO;
using MetroHome65.Interfaces;
using MetroHome65.Routines.File;

namespace MetroHome65
{
    /// <summary>
    /// Class for managing plugins assemblies:
    /// load plugins at sartup, create plugin by type etc.
    /// </summary>
    public class PluginManager : IPluginManager
    {
        private readonly Hashtable _tilePlugins = new Hashtable();
        private readonly Hashtable _lockScreenPlugins = new Hashtable();

        public PluginManager()
        {
            LoadPlugins();
        }
        
        public IEnumerable GetTileTypes()
        {
            return _tilePlugins.Values;
        }

        public IEnumerable GetLockScreenTypes()
        {
            return _lockScreenPlugins.Values;
        }

        /// <summary>
        /// reads all .dll in Plugins folder and scans for plugins with 
        /// ITile interface.
        /// Fill internal plugin map - plugin type by name.
        /// </summary>
        private void LoadPlugins()
        {
            var folderInfo = new DirectoryInfo(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase));
            FileInfo[] files = folderInfo.GetFiles();

            foreach (FileInfo file in files)
            {
                if ((! file.Name.ToLower().Contains("opennetcf")) && 
                    (file.Extension == ".dll"))
                    LoadPlugin(file.FullName);
            }
        }

        private void LoadPlugin(String pluginPath)
        {
            var assembly = Assembly.LoadFrom(pluginPath);

            foreach (var type in assembly.GetTypes())
            {
                if ((type.IsClass) && (! type.IsAbstract))
                {
                    if (type.GetInterfaces().Contains(typeof(ITile))) 
                        _tilePlugins.Add(type.FullName, type);
                    else
                    if (type.GetInterfaces().Contains(typeof(ILockScreen)))
                        _lockScreenPlugins.Add(type.FullName, type);
                }
            }
        }

        public ITile CreateTile(String tileName)
        {
            try
            {
                var tileType = (Type) _tilePlugins[tileName];
                if (tileType == null) return null;

                var tile = (ITile) Activator.CreateInstance(tileType);
                return tile;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, ex.Message);
                return null;
            }
        }

        public ILockScreen CreateLockScreen(String lockScreenName)
        {
            try
            {
                var lockScreenType = (Type)_lockScreenPlugins[lockScreenName];
                if (lockScreenType == null) return null;

                var lockScreen = (ILockScreen)Activator.CreateInstance(lockScreenType);
                return lockScreen;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex.StackTrace, ex.Message);
                return null;
            }
        }

    }

}
