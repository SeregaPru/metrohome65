using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Fleux.UIElements;
using Fleux.Core;
using System.Drawing;
using Fleux.Animations;

namespace MetroHome65.HomeScreen
{
    public class Arrow : TransparentImageElement
    {
        public Arrow()
            : base(ResourceManager.Instance.GetIImageFromEmbeddedResource("next.png"))
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
