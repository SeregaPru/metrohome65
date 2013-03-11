using MetroHome65.Interfaces;

namespace MetroHome65.Widgets
{
    [TileInfo("E-mail")]
    public class EMailWidget : PhoneWidget
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingTotalEmailUnread;
        }
    }
}