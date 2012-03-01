using System.Drawing;

namespace MetroHome65.HomeScreen
{
    public class Arrow : FlatButton
    {
        public Arrow()
            : base("MetroHome65.Images.next.ico")
        {
            this.Size = new Size(48, 48);
        }

        public void Next()
        {
            ResourceName = "MetroHome65.Images.next.ico";
            this.Size = new Size(48, 48);
        }

        public void Prev()
        {
            ResourceName = "MetroHome65.Images.back.ico";
            this.Size = new Size(48, 48);
        }

    }
}
