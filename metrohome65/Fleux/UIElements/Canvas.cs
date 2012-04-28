using Fleux.Controls;

namespace Fleux.UIElements
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Animations;
    using Core;
    using Core.GraphicsHelpers;

    public class Canvas : UIElement
    {
        public Canvas()
        {
            this.EntranceAnimation = new ForwarderAnimation(() => new AnimationGroup(this.Children.Where(e => e.EntranceAnimation != null).Select(e => e.EntranceAnimation)));
            this.ExitAnimation = new ForwarderAnimation(() => new AnimationGroup(this.Children.Where(e => e.ExitAnimation != null).Select(e => e.ExitAnimation)));
        }

        public virtual void AddElement(UIElement element)
        {
            this.Children.Add(element);
            element.Parent = this;

            //! metrohome65
            element.ParentControl = this.ParentControl;

            this.Size = new Size(Math.Max(element.Bounds.Right, this.Size.Width), Math.Max(element.Bounds.Bottom, this.Size.Height));
            element.Updated = this.OnUpdated;
        }

        public virtual void AddElementAt(int index, UIElement element)
        {
            this.Children.Insert(index, element);
            element.Parent = this;

            //! metrohome65
            element.ParentControl = this.ParentControl;

            this.Size = new Size(Math.Max(element.Bounds.Right, this.Size.Width), Math.Max(element.Bounds.Bottom, this.Size.Height));
            element.Updated = this.OnUpdated;
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            this.Children
                .Where(i => i.Bounds.IntersectsWith(drawingGraphics.VisibleRect))
                .ToList()
                .ForEach(e => e.Draw(drawingGraphics.CreateChild(e.Location, e.TransformationScaling, e.TransformationCenter)));
        }

        public void Clear()
        {
            this.Children.Clear();
        }

        //! metrohome65
        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);
            this.Children.ToList().ForEach(e => e.ParentControl = parentControl);
        }

    }
}