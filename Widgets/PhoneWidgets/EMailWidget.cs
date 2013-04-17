using MetroHome65.Interfaces;
using MetroHome65.Routines;

namespace MetroHome65.Widgets
{
    [TileInfo("E-mail")]
    public class EMailWidget : PhoneWidget
    {
        public EMailWidget()
        {
            Caption = "Mail".Localize();
        }

        protected override int GetMissedCount()
        {
            return Microsoft.WindowsMobile.Status.SystemState.MessagingTotalEmailUnread;
        }
    }
}