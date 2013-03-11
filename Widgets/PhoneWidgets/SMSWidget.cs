using MetroHome65.Interfaces;

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

	private const string _registryPath = "\\Software\MTK\SMS\Unread";

        protected override int GetMissedCount()
        {
	    if (_unreadType != 1)
            	return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
	    else
	    {
		return (int)Registry.LocalMachine.OpenSubKey(_registryPath, false);
	    }           
        }

        public override ICollection<UIElement> EditControls(FleuxControlPage settingsPage)
        {
            var controls = base.EditControls(settingsPage);
            var bindingManager = new BindingManager();
            
            controls.AddElement(new Separator());

	    // sms count method selection
            var ctrUnreadType = new SelectSettingsControl
            {
                Caption = "Unread Count".Localize(),
                Items = new List<object> { "Standart".Localize(), "MTK".Localize() },
            };
            controls.AddElement(ctrUnreadType);
            bindingManager.Bind(this, "UndearType", ctrUnreadType, "SelectedIndex", true);

            return controls;
        }

    }
}