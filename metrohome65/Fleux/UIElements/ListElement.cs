namespace Fleux.UIElements
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Core.GraphicsHelpers;

    public class ListElement : ScrollViewer
    {
        protected BindingList<object> sourceItems;
        protected Canvas contentCanvas = new Canvas();
        protected int currentFirstItem = 0;

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
                new System.Threading.Thread(() =>
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
                    }).Start();
            }
        }

        public Func<object, Func<object, UIElement>> DataTemplateSelector { get; set; }
    }
}
