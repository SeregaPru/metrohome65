using MetroHome65.HomeScreen.TilesGrid;
using MetroHome65.Routines.UIControls;

namespace MetroHome65.Controls
{
    public class Arrow : ThemedImageButton
    {
        private const string NextImageName = "next";
        private const string PrevImageName = "back";

        public Arrow()
            : base(NextImageName)
        {
        }

        public void Next()
        {
            ResourceName = GetImageRes(NextImageName);
        }

        public void Prev()
        {
            ResourceName = GetImageRes(PrevImageName);
        }

    }
}
