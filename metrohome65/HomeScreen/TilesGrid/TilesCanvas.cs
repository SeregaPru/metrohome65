using System;
using System.Drawing;
using System.Linq;
using Fleux.Animations;
using Fleux.Controls;
using Fleux.Core.GraphicsHelpers;
using Fleux.Styles;
using Fleux.UIElements;
using MetroHome65.HomeScreen.Tile;
using MetroHome65.Routines;
using TinyIoC;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class TilesCanvas : Canvas  
    {
        #region Fields

        private int _verticalOffset;

        // duration for entrance or exit animation
        private const int AnimationDuration = 400;

        // buffer and graphics for direct to screen draw
        private Graphics _controlGraphics;
        private IDrawingGraphics _drawingGraphics;
        private DoubleBuffer _buffer;

        private readonly UIElement _bgImage;

        // flag that we redraw now
        private bool _updating;

        #endregion


        #region Properties

        public bool FreezeUpdate { get; set; }

        #endregion


        public TilesCanvas()
        {
            _bgImage = TinyIoCContainer.Current.Resolve<HomeScreenBackground>();

            Size = new Size(TileConsts.TilesPaddingLeft + TileConsts.TileSize * 4 + TileConsts.TileSpacing * 3, 100);
            SizeChanged += (s, e) => CreateBuffer();
        }

        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);

            if ((parentControl != null) && (_controlGraphics == null))
                _controlGraphics = parentControl.CreateGraphics();
        }

        private void CreateBuffer()
        {
            // create new buffer with new size
            _buffer = new DoubleBuffer(new Size(Size.Width, ScreenConsts.ScreenHeight));

            _drawingGraphics = DrawingGraphics.FromGraphicsAndRect(
                _buffer.Graphics, _buffer.Image,
                new Rectangle(0, 0, _buffer.Image.Width, _buffer.Image.Height));
        }

        public void DeleteElement(UIElement element)
        {
            element.Parent = null;
            element.Updated = null;
            Children.Remove(element);
        }


        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            _verticalOffset = drawingGraphics.VisibleRect.Top;

            base.Draw(drawingGraphics);
        }

        public void DirectDraw(int verticalOffset)
        {
            _verticalOffset = -verticalOffset;

            if (_buffer == null) return;

            if (_updating) return;
            _updating = true;

            try
            {
                // draw background
                _buffer.Graphics.Clear(MetroTheme.PhoneBackgroundBrush);
                _bgImage.Draw(_drawingGraphics);

                // draw tiles
                var rect = new Rectangle(0, - verticalOffset, Size.Width, Size.Height);
                foreach (var e in ChildrenEnumerable)
                {
                    if (e.Bounds.IntersectsWith(rect))
                        e.Draw(_drawingGraphics.CreateChild(
                            new Point(e.Location.X, e.Location.Y + verticalOffset)));
                }

                // draw buffer directly to screen
               _controlGraphics.DrawImage(_buffer.Image, 0, 0);

            }
            catch (Exception) { }

            _updating = false;
        }

        public void DirectDrawElement(int verticalOffset, UIElement element)
        {
            if (_buffer == null) return;

            var rect = new Rectangle(0, -verticalOffset, Size.Width, Size.Height);
            if (!element.Bounds.IntersectsWith(rect)) return;

            if (_updating) return;
            _updating = true;

            var clipRect = new Rectangle(
                element.Location.X, element.Location.Y + verticalOffset, element.Size.Width, element.Size.Height);
            
            try
            {
                // draw background
                _buffer.Graphics.FillRectangle(new SolidBrush(MetroTheme.PhoneBackgroundBrush), clipRect);
                var oldclip = _buffer.Graphics.Clip;
                _buffer.Graphics.Clip = new Region(clipRect);
                _bgImage.Draw(_drawingGraphics);
                _buffer.Graphics.Clip = oldclip;

                // draw tiles
                element.Draw(_drawingGraphics.CreateChild(
                    new Point(element.Location.X, element.Location.Y + verticalOffset)));

                // draw buffer directly to screen
                _controlGraphics.DrawImage(_buffer.Image, element.Location.X, element.Location.Y + verticalOffset, 
                    clipRect, GraphicsUnit.Pixel);

            }
            catch (Exception) { }

            //_buffer.Graphics.Clip = oldclip;
            _updating = false;
        }

        /// <summary>
        /// Handler for update event from child.
        /// Redraw only in non-freezed mode
        /// </summary>
        /// <param name="element"></param>
        protected override void OnUpdated(UIElement element)
        {
            if (FreezeUpdate) return;

            if (element == this)
              base.Updated(this);
            else
                DirectDrawElement(- _verticalOffset, element);
            
        }


        #region Animation

        //!! todo - потом переписать не на анимацию каждой плитки, а на централизовано управляемую анимацию
        //!! todo - и не использовать анимацию контрола, а делать все внутри

        public void AnimateExit()
        {
            FreezeUpdate = false;

            var scrollRect = new Rectangle(0, _verticalOffset, Bounds.Width, Bounds.Height);
            var sb = new StoryBoard();

            foreach (var curTile in ChildrenEnumerable)
            {
                var tileWrapper = curTile as TileWrapper;

                if (curTile.Bounds.IntersectsWith(scrollRect))
                {
                    curTile.ExitAnimation = GetExitAnimation(tileWrapper);
                    curTile.EntranceAnimation = GetEntranceAnimation(tileWrapper);

                    sb.AddAnimation(curTile.ExitAnimation);
                }
                else
                {
                    curTile.EntranceAnimation = null;
                    curTile.ExitAnimation = null;
                }
            }

            // stub animation for redraw
            sb.AddAnimation(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                                {
                                    From = 0,
                                    To = 100,
                                    Duration = AnimationDuration,
                                    OnAnimation = v => DirectDraw(- _verticalOffset),
                                });
            sb.AnimateSync();
        }

        public void AnimateEntrance()
        {
            var sb = new StoryBoard();
            foreach (var element in Children.Where(e => e.EntranceAnimation != null))
            {
                sb.AddAnimation(element.EntranceAnimation);
            }

            // stub animation for redraw
            sb.AddAnimation(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                                {
                                    From = 0,
                                    To = 100,
                                    Duration = AnimationDuration,
                                    OnAnimation = v => DirectDraw(- _verticalOffset),
                                });
            sb.AnimateSync();

            // not always animation completes good. after animation correct tiles positions.
            foreach (var element in Children)
                element.Location = (element as TileWrapper).GetScreenRect().Location;
            DirectDraw(- _verticalOffset);

            FreezeUpdate = true;
        }

        private IAnimation GetExitAnimation(TileWrapper target)
        {
            var random = new Random();
            return new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
                       {
                           Duration = AnimationDuration,
                           To = -target.Size.Width - random.Next(1000),
                           From = target.Location.X,
                           OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                           OnAnimationStop = () => { target.ExitAnimation = null; },
            };
        }

        private IAnimation GetEntranceAnimation(TileWrapper target)
        {
            var random = new Random();
            var x = target.GetScreenRect().Left; // here use GetScreenRect instead Bounds because real coords are wrong
            return new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = AnimationDuration,
                From = x - 1000 + random.Next(1000 - x - 173),
                To = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                OnAnimationStop = () => { target.EntranceAnimation = null; },
            };
        }

        #endregion
    }
}
