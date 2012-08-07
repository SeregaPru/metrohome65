using System.Drawing;
using System.Reflection;
using Fleux.Core;
using Fleux.UIElements;
using Fleux.Core.GraphicsHelpers;
using Fleux.Core.NativeHelpers;


namespace MetroHome65.WPLock.Controls
{
    class TransparentButton : UIElement
    {
        #region Private fields
        private bool m_pressed;
        private IImageWrapper image;
        #endregion

        #region Constructor

        public TransparentButton(string resourceName)
            : this(ResourceManager.Instance.GetIImageFromEmbeddedResource(resourceName, Assembly.GetCallingAssembly()))
        {
        }

        
        public TransparentButton(IImageWrapper image) 
        {
            this.image = image;
            this.Size = image.Size;
        }
        #endregion
        
        #region Properties
        public IImageWrapper Image
        {
            get { return this.image; }
            set { this.image = value; }
        }
        #endregion

        #region Methods - OVERRIDE
        public override UIElement Pressed(Point p)
        {
            this.m_pressed = true;
            this.Update();
            return this;
        }

        public override void Released()
        {
            this.m_pressed = false;
            this.Update();
            base.Released();
        }
        
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (m_pressed)
            {
                drawingGraphics.DrawAlphaImage(this.image, 2, 2, Size.Width - 4, Size.Height - 4);
            }
            else
            {
                drawingGraphics.DrawAlphaImage(this.image, 0, 0, Size.Width, Size.Height);
            }
        }
        #endregion
    
    }
}
