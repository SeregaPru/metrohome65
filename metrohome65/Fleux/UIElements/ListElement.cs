namespace Fleux.UIElements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    public class ListElement : ScrollViewer
    {
        protected BindingList<object> sourceItems;
        protected Canvas contentCanvas = new Canvas();
        protected int currentFirstItem = 0;

        // first fill list items in thread
        protected bool ThreadedFill = true;

        public ListElement()
        {
            this.VerticalScroll = true;
        }

        public virtual BindingList<object> SourceItems
        {
            get
            {
                return this.sourceItems;
            }

            set
            {
                if (ThreadedFill)
                    new System.Threading.Thread(() => FillBindingList(value)).Start();
                else
                    FillBindingList(value);
            }
        }

        private void FillBindingList(BindingList<object> value)
        {
            this.sourceItems = value;
            int y = 0;
            foreach (var o in this.sourceItems)
            {
                var uiE = this.DataTemplateSelector(o)(o);
                uiE.Location = new Point(0, y);
                this.contentCanvas.AddElement(uiE);
                y += uiE.Size.Height;
            }
            this.contentCanvas.Size = new Size(this.Size.Width, y);
            this.Content = this.contentCanvas;
        }

        public Func<object, Func<object, UIElement>> DataTemplateSelector { get; set; }
    }
}
