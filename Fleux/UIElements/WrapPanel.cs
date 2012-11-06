namespace Fleux.UIElements
{
    using System.Drawing;

    public class WrapPanel : Canvas
    {
        public Color Foreground { get; set; }
        public int Rows { get; set; }
        public int LeftMargin { get; set; }
        public int TopMargin { get; set; }
        public int RightMargin { get; set; }
        public int BottomMargin { get; set; }
        public Image Background { get; set; }

        public WrapPanel()
        {
            this.SizeChanged += (s, e) => this.Relayout();
        }

        public override void AddElement(UIElement element)
        {
            base.AddElement(element);
            this.Relayout();
        }

        public override void AddElementAt(int index, UIElement element)
        {
            base.AddElementAt(index, element);
            this.Relayout();
        }

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            if (Background != null)
            {
                int backgroundSize = 0;
                while (backgroundSize < this.Size.Height)
                {
                    drawingGraphics.DrawImage(Background, 0, backgroundSize);
                    backgroundSize += Background.Size.Height;
                }
            }
            base.Draw(drawingGraphics);
        }

        private void Relayout()
        {
            int x = LeftMargin;
            int y = TopMargin;
            Rows = 0;
            int itemHeight = 0;
            int panelHeight = 0;
            foreach (var i in this.Children)
            {
                if (TopMargin + i.Size.Height + BottomMargin > itemHeight)
                    itemHeight = TopMargin + i.Size.Height + BottomMargin;

                if (panelHeight == 0)
                {
                    panelHeight = itemHeight;
                    Rows = 1;
                }

                i.Location = new Point(x, y);
                if (x + LeftMargin + i.Size.Width + RightMargin > this.Size.Width)
                {
                    x = LeftMargin;
                    y += itemHeight;
                    Rows++;
                    panelHeight += itemHeight;
                    itemHeight = 0;
                }
                else
                {
                    x += LeftMargin + i.Size.Width + RightMargin;
                }
            }
            this.Size = new Size(this.Size.Width, panelHeight);
        }
    }
}
