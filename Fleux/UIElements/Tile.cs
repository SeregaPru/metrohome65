// GIANNI added

namespace Fleux.UIElements
{
    using System.Drawing;
    using Fleux.Styles;
    using Fleux.UIElements;
    using Fleux.Core.GraphicsHelpers;

    public class Tile : UIElement
    {
        #region Private fields
        private readonly TextElement m_Text;
        private readonly ImageElement m_Icon;
        #endregion

        #region Properties
        public string Name { get { return m_Text.Text; } }
        #endregion

        #region Constructor
        public Tile(string tileName, Image icon, int x, int y, bool wide)
        {
            this.Size = new Size(wide ? 358 : 173, 173);
            this.Location = new Point(x, y);
            m_Icon = new ImageElement(icon);
            //m_Icon.Location = new Point((this.Size.Width - m_Icon.Size.Width) / 2, (this.Size.Height - m_Icon.Size.Height) / 2);
            m_Icon.Location = new Point(0, 0);
            m_Text = new TextElement(tileName) { Style = MetroTheme.TileTextStyle, Location = new Point(13, 140) };
        }
        #endregion

        #region Methods - OVERRIDE
        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            drawingGraphics.Color(MetroTheme.PhoneAccentBrush);
            drawingGraphics.FillRectangle(0, 0, this.Size.Width, this.Size.Height);
            m_Icon.Draw(drawingGraphics.CreateChild(m_Icon.Location, m_Icon.TransformationScaling, m_Icon.TransformationCenter));
            m_Text.Draw(drawingGraphics.CreateChild(m_Text.Location, m_Text.TransformationScaling, m_Text.TransformationCenter));
        }
        #endregion
    }
}
