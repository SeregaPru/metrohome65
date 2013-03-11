namespace Fleux.Core.Scaling
{
    using System.Drawing;

    public static class SizeExtensions
    {
        public static Size ToLogic(this Size scaled)
        {
            return new Size(scaled.Width.ToLogic(), scaled.Height.ToLogic());
        }

        public static Size ToPixels(this Size logic)
        {
            return new Size(logic.Width.ToPixels(), logic.Height.ToPixels());
        }
    }
}
