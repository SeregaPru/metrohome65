using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;

namespace MetroHome65.LockScreen
{
    public class LockScreenSettings: StackPanel
    {
        private readonly CustomSettingsPage<MainSettings> _page;
        private readonly ComboBox _lockScreenTypeCombo;

        private readonly IPluginManager _pluginManager;

        private List<Type> _lockScreenTypes = new List<Type>();



        public LockScreenSettings(CustomSettingsPage<MainSettings> page)
        {
            _page = page;
            _pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            // selection for lockscreen plugin
            this.AddElement(
                new TextElement("Lock screen plugin") { Size = new Size(SettingsConsts.MaxWidth, 50), }
                );

            _lockScreenTypeCombo = new ComboBox { Size = new Size(SettingsConsts.MaxWidth, 50), };
            FillLockScreenTypes();
            _lockScreenTypeCombo.SelectedIndexChanged += (s, e) => ChangeLockScreenType(); // urgent! after filling
            this.AddElement(_lockScreenTypeCombo);

            this.AddElement(new Separator());

            // add current lockscreen settings
            ChangeLockScreenType();
        }

        private void FillLockScreenTypes()
        {
            _lockScreenTypeCombo.Items.Clear();
            _lockScreenTypes.Clear();

            var selectedIndex = -1;
            foreach (Type plugin in _pluginManager.GetLockScreenTypes())
            {
                // get human readable lockscreen name for display in list
                var lockScreenName = "";
                var attributes = plugin.GetCustomAttributes(typeof(LockScreenInfoAttribute), true);
                if (attributes.Length > 0)
                    lockScreenName = ((LockScreenInfoAttribute)attributes[0]).Caption;
                if (lockScreenName == "")
                    lockScreenName = plugin.Name;

                _lockScreenTypeCombo.Items.Add(lockScreenName);
                _lockScreenTypes.Add(plugin);

                if (_page.Settings.LockScreenClass == plugin.Name)
                    selectedIndex = _lockScreenTypeCombo.Items.Count - 1;
            }

            if (_lockScreenTypeCombo.Items.Count > 1)
                _lockScreenTypeCombo.SelectedIndex = selectedIndex;
            else
                _lockScreenTypeCombo.SelectedIndex = 2; // hack
        }

        private void ChangeLockScreenType()
        {
            if (_lockScreenTypeCombo.SelectedIndex == -1) return;

            // get selected plugin type
            var type = _lockScreenTypes[_lockScreenTypeCombo.SelectedIndex];

            Type settingsType = null;
            var props = type.GetProperties();
            foreach (var srcPropInfo in props)
            {
                // set only properties that are marked as lockscreen setings
                var attributes = srcPropInfo.GetCustomAttributes(typeof(LockScreenSettingsAttribute), true);
                if (attributes.Length > 0)
                {
                    settingsType = srcPropInfo.PropertyType;
                    break;
                }
            }

            // create empty settings object for selected lockscreen type
            if (settingsType == null) return;
            var settings = Activator.CreateInstance(settingsType);

            _page.Settings.LockScreenSettings = settings as ILockScreenSettings;
            CreateSettingsControls();
        }

        private void CreateSettingsControls()
        {
            var lockScreenSettings = _page.Settings.LockScreenSettings as ILockScreenSettings;
            if (lockScreenSettings == null) return;

            var controls = lockScreenSettings.EditControls(_page, _page.BindingManager);
            foreach (var uiElement in controls)
            {
                this.AddElement(uiElement);

                this.AddElement(new Separator());
            }

        }
    }
}
