using Fleux.Core;
using System.Drawing;

namespace MetroHome65.HomeScreen
{
    public class Arrow : FlatButton
    {
        public Arrow() : base("next.png")
        {
            this.Size = new Size(48, 48);
        }

        public void Next()
        {
            Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("next.png");
        }

        public void Prev()
        {
            Image = ResourceManager.Instance.GetIImageFromEmbeddedResource("back.png");
        }

    }
}
