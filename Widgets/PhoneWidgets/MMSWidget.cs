using MetroHome65.Interfaces;

namespace MetroHome65.Widgets
{
    [TileInfo("MMS")] 
    public class MMSWidget : PhoneWidget
    {
        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingMmsUnread;
        }
    }
}