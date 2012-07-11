// GIANNI added

namespace Fleux.UIElements
{
    using System.Drawing;
    using Core;
    using Core.GraphicsHelpers;
    using System.Reflection;

    public class ImageButton : UIElement
    {
        #region Private fields
        private bool m_pressed;
        private ImageElement m_image;
        #endregion

        #region Constructor
        public ImageButton(string resourceName)
            : this(ResourceManager.Instance.GetBitmapFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()))
        {
        }

        public ImageButton(Image image)
        {
            m_image = new ImageElement(image);
            this.Size = image.Size;
        }
        #endregion

        #region Properties
        
        public Image Image
        {
            get
            {
                return m_image.Image;
            }

            set
            {
                m_image.Image = value;
                this.Update();
            }
        }
        #endregion

        #region Methods - OVERRIDE
        public override UIElement Pressed(Point p)
        {
            m_pressed = true;
            this.Update();
            return this;
        }

        public override void Released()
        {
            m_pressed = false;
            this.Update();
            base.Released();
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (m_pressed)
            {
                m_image.Location = new Point(2, 2);
                //drawingGraphics.DrawImage(m_image, 2, 2, Size.Width, Size.Height);
            }
            else
            {
                m_image.Location = new Point(0, 0);
                //drawingGraphics.DrawImage(m_image, 0, 0, Size.Width, Size.Height);
            }
            m_image.DrawTransparent(drawingGraphics.CreateChild(m_image.Location));
        }
        #endregion
    }
}
