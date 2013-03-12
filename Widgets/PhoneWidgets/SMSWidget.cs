using System;
using Fleux.Controls;
using Fleux.UIElements;
using MetroHome65.Interfaces;
using MetroHome65.Routines;
using MetroHome65.Routines.UIControls;
using Metrohome65.Settings.Controls;
using Microsoft.Win32;
using System.Collections.Generic;

namespace MetroHome65.Widgets
{
    [TileInfo("SMS")]
    public class SMSWidget : PhoneWidget
    {
	    // How to get unread sms count.
	    // for MTK chipset there is special registry item.
	    int _unreadType = 0;

        [TileParameter]
        public int UnreadType
        {
            get { return _unreadType; }
            set {
                if (_unreadType != value)
                {
                    _unreadType = value;
                    NotifyPropertyChanged("UnreadType");
                }
            }
        }

	    private const string _registryPath = @"\Software\MTK\SMS";

        protected override int GetMissedCount()
        {
	        if (_unreadType != 1)
                return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
	        else
	        {
	            try
	            {
                    return (int)Registry.LocalMachine.OpenSubKey(_registryPath, false).GetValue("Unread");
	            }
	            catch (Exception)
	            {
	                return 0;
	            }
	        }           
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();
            
	        // sms count method selection
            var ctrUnreadType = new SelectSettingsControl
            {
                Caption = "Unread Count".Localize(),
                Items = new List<object> { "Standart".Localize(), "MTK".Localize() },
            };
            controls.Add(ctrUnreadType);
            bindingManager.Bind(this, "UnreadType", ctrUnreadType, "SelectedIndex", true);

            controls.Add(new Separator());

            return controls;
        }

    }
}