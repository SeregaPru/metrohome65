using MetroHome65.HomeScreen.TilesGrid;

namespace MetroHome65.HomeScreen
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
