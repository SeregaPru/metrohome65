using Fleux.UIElements;
using Metrohome65.Settings.Controls;

namespace MetroHome65.LockScreen
{
    public class LockScreenSettings: StackPanel
    {

        public LockScreenSettings()
        {
            var stackPanel = new StackPanel();

            // lock screen bg image
            var ctrLockScreenImage = new ImageSettingsControl
                                         {
                                             Caption = "Lock screen background",
                                             //!!Value = Settings.LockScreenImage,
                                         };
            stackPanel.AddElement(ctrLockScreenImage);
            //!!BindingManager.Bind(Settings, "LockScreenImage", ctrLockScreenImage, "Value");
        }
    }
}
