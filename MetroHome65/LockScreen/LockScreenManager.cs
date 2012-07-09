using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.LockScreen
{
    public class LockScreenManager
    {
        static public UIElement CreateLockScreen()
        {
            var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            return pluginManager.CreateLockScreen("MetroHome65.SimpleLock.SimpleLock") as UIElement;
        }
    }
}
