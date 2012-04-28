namespace Fleux.UIElements
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;

    public class StackPanel : Canvas
    {
        public StackPanel()
        {
            this.SizeChanged += (s, e) => this.Relayout();
        }

        public override void AddElement(UIElement element)
        {
            base.AddElement(element);
            element.SizeChanged += (s, e) => this.Relayout();
            this.Relayout();
        }

        public override void AddElementAt(int index, UIElement element)
        {
            base.AddElementAt(index, element);
            element.SizeChanged += (s, e) => this.Relayout();
            this.Relayout();
        }

        private void Relayout()
        {
            int y = 0;
            foreach (var i in this.Children)
            {
                i.Location = new Point(0, y);
                i.ResizeForWidth(this.Size.Width);
                y += i.Size.Height;                
            }

            Size = new Size(Size.Width, y);
        }
    }
}
