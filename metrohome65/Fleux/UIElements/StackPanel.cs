namespace Fleux.UIElements
{
    using System.Drawing;

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

        /// <summary>
        /// flag that relayout is now performing. to prevent cyclyng
        /// </summary>
        private bool _processingRelayout;

        private void Relayout()
        {
            if (_processingRelayout) return;
            _processingRelayout = true;

            int y = 0;
            foreach (var i in this.Children)
            {
                i.Location = new Point(0, y);
                i.ResizeForWidth(this.Size.Width);
                y += i.Size.Height;                
            }

            Size = new Size(Size.Width, y);

            _processingRelayout = false;
        }
    }
}
