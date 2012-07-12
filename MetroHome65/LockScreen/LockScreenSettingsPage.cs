using System;
using System.Collections.Generic;
using System.Drawing;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Settings;
using MetroHome65.Interfaces;
using MetroHome65.Interfaces.Events;
using MetroHome65.Routines.Settings;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;
using TinyIoC;
using TinyMessenger;

namespace MetroHome65.LockScreen
{
    public class LockScreenSettingsPage: StackPanel
    {
        private readonly CustomSettingsPage<MainSettings> _page;
        private readonly ComboBox _lockScreenTypeCombo;
        private readonly StackPanel _settingsPanel;

        private readonly List<Type> _lockScreenTypes = new List<Type>();
        private ILockScreenSettings _lockScreenSettings;


        public LockScreenSettingsPage(CustomSettingsPage<MainSettings> page)
        {
            _page = page;
            _page.OnApplySettings += (sender, settings) => OnApplySettings(settings);

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
            _settingsPanel = new StackPanel();
            this.AddElement(_settingsPanel);
            ChangeLockScreenType();
        }

        /// <summary>
        /// Fill combobox with lockscreen types from loaded plugins
        /// </summary>
        private void FillLockScreenTypes()
        {
            _lockScreenTypeCombo.Items.Clear();
            _lockScreenTypes.Clear();

            var _pluginManager = TinyIoC.TinyIoCContainer.Current.Resolve<IPluginManager>();

            var selectedIndex = -1;
            foreach (Type plugin in _pluginManager.GetLockScreenTypes())
            {
                // get human readable lockscreen name for display in list
                var lockScreenName = "";
                var attributes = plugin.GetCustomAttributes(typeof(LockScreenInfoAttribute), true);
                if (attributes.Length > 0)
                    lockScreenName = ((LockScreenInfoAttribute)attributes[0]).Caption;
                if (String.IsNullOrEmpty(lockScreenName))
                    lockScreenName = plugin.Name;

                _lockScreenTypeCombo.Items.Add(lockScreenName);
                _lockScreenTypes.Add(plugin);

                if (String.Equals(_page.Settings.LockScreenSettings.LockScreenClass, plugin.FullName))
                    selectedIndex = _lockScreenTypeCombo.Items.Count - 1;
            }

            if (_lockScreenTypeCombo.Items.Count > 1)
                _lockScreenTypeCombo.SelectedIndex = selectedIndex;
            else
                _lockScreenTypeCombo.SelectedIndex = 2; // hack
        }

        /// <summary>
        /// Handler for change of lockscreen type.
        /// Rebuild settings page with settings for new selected lockscreen type.
        /// </summary>
        private void ChangeLockScreenType()
        {
            if (_lockScreenTypeCombo.SelectedIndex == -1) return;

            // get selected plugin type 
            var type = _lockScreenTypes[_lockScreenTypeCombo.SelectedIndex];
            _page.Settings.LockScreenSettings.LockScreenClass = type.FullName;

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
            _lockScreenSettings = settings as ILockScreenSettings;
            if (_lockScreenSettings == null) return;

            // fill new settings properties with according properties from old settings
            StoredSettingsHelper.StoredSettingsToObject(_page.Settings.LockScreenSettings.Parameters,
                _lockScreenSettings, typeof(LockScreenParameterAttribute));

            CreateSettingsControls();
        }

        /// <summary>
        /// create controls for selected lockscreen settings
        /// </summary>
        private void CreateSettingsControls()
        {
            _settingsPanel.Clear();

            var controls = _lockScreenSettings.EditControls(_page, _page.BindingManager);
            foreach (var uiElement in controls)
            {
                _settingsPanel.AddElement(uiElement);
                _settingsPanel.AddElement(new Separator());
            }
        }

        /// <summary>
        /// notify about change lockscreen settings
        /// </summary>
        private void OnApplySettings(MainSettings settings)
        {
            // store lockscreen type and settings to MainSettings
            var mainSettings = TinyIoCContainer.Current.Resolve<MainSettings>();
            mainSettings.LockScreenSettings.LockScreenClass = settings.LockScreenSettings.LockScreenClass;
            StoredSettingsHelper.StoredSettingsFromObject(ref mainSettings.LockScreenSettings.Parameters,
                _lockScreenSettings, typeof(LockScreenParameterAttribute));

            // notify about lockscreen settings change
            var messenger = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
            messenger.Publish(new SettingsChangedMessage("LockScreenClass", settings.LockScreenSettings.LockScreenClass));
            messenger.Publish(new SettingsChangedMessage("LockScreenSettings", _lockScreenSettings));
        }

    }
}
