using MetroHome65.Interfaces;

namespace MetroHome65.Widgets
{
    [TileInfo("SMS")]
    public class SMSWidget : PhoneWidget
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingSmsUnread;
        }
    }
}