using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

namespace MetroHome65.LockScreen
{
    public class LockScreenSettings: StackPanel
    {
        private readonly ComboBox _lockScreenTypeCombo;

        private readonly List<Type> _lockScreenTypes = new List<Type>();

        private readonly IPluginManager _pluginManager;



        public LockScreenSettings()
        {
            _pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            var stackPanel = new StackPanel();

            // selection for lockscreen plugin
            stackPanel.AddElement(
                new TextElement("LockScreen plugin") { Size = new Size(SettingsConsts.MaxWidth, 50), }
                );

            _lockScreenTypeCombo = new ComboBox { Size = new Size(SettingsConsts.MaxWidth, 50), };
            _lockScreenTypeCombo.SelectedIndexChanged += (s, e) => ChangeLockScreenType();
            FillLockScreenTypes();
            stackPanel.AddElement(_lockScreenTypeCombo);

            stackPanel.AddElement(new Separator());

            // lock screen bg image
            var ctrLockScreenImage = new ImageSettingsControl
                                         {
                                             Caption = "Lock screen background",
                                             //!!Value = Settings.LockScreenImage,
                                         };
            stackPanel.AddElement(ctrLockScreenImage);
            //!!BindingManager.Bind(Settings, "LockScreenImage", ctrLockScreenImage, "Value");
        }

        private void FillLockScreenTypes()
        {
            _lockScreenTypes.Clear();
            var items = new List<object>();

            foreach (Type plugin in _pluginManager.GetLockScreenTypes())
            {
                // get human readable lockscreen name for display in list
                var lockScreenName = "";
                var attributes = plugin.GetCustomAttributes(typeof(LockScreenInfoAttribute), true);
                if (attributes.Length > 0)
                    lockScreenName = ((LockScreenInfoAttribute)attributes[0]).Caption;
                if (lockScreenName == "")
                    lockScreenName = plugin.Name;

                items.Add(lockScreenName);
                _lockScreenTypes.Add(plugin);
            }

            _lockScreenTypeCombo.Items = items;
        }

        private void ChangeLockScreenType()
        {
            //
        }

    }
}
