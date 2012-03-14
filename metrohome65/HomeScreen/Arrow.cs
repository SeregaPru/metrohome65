using MetroHome65.HomeScreen.TilesGrid;

namespace MetroHome65.HomeScreen
{
    public class Arrow : ThemedImageButton
    {
        private const string NextImage = "next";
        private const string PrevImage = "back";

        public Arrow()
            : base(NextImage)
        {
        }

        public void Next()
        {
            ResourceName = GetImageRes(NextImage);
        }

        public void Prev()
        {
            ResourceName = GetImageRes(PrevImage);
        }

    }
}
