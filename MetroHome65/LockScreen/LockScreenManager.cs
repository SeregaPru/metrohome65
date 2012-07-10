using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Interfaces;

namespace MetroHome65.LockScreen
{
    public class LockScreenManager: Canvas
    {
        private UIElement _lockScreen;

        public LockScreenManager()
        {
            CreateLockScreen();
            this.SizeChanged += (sender, args) => OnSizeChanged();
        }

        private void CreateLockScreen()
        {
            var pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            _lockScreen = pluginManager.CreateLockScreen("MetroHome65.SimpleLock.SimpleLock") as UIElement;
            _lockScreen.Location = new Point(0, 0);

            Clear();
            AddElement(_lockScreen);
        }

        private void OnSizeChanged()
        {
            _lockScreen.Size = this.Size;
        }

    }
}
