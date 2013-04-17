using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("MMS")] 
    public class MMSWidget : PhoneWidget
    {
        public MMSWidget()
        {
            Caption = "MMS".Localize();
        }

        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingMmsUnread;
        }
    }
}