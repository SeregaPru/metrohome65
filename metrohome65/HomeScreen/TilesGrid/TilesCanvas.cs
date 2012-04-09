using System;
using System.Drawing;
using System.Linq;
using Fleux.Animations;
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
        private int _animationDuration = 400;

        private Graphics _controlGraphics;

        private IDrawingGraphics _drawingGraphics;

        private DoubleBuffer _buffer;

        private UIElement _bgImage;

        #endregion


        #region Properties

        public bool FreezeUpdate { get; set; }

        #endregion


        public TilesCanvas(Graphics controlGraphics)
        {
            _controlGraphics = controlGraphics;
            _bgImage = TinyIoCContainer.Current.Resolve<HomeScreenBackground>();

            Size = new Size(TileConsts.TilesPaddingLeft + TileConsts.TileSize * 4 + TileConsts.TileSpacing * 3, 100);
            SizeChanged += (s, e) => CreateBuffer();
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
            if (_buffer == null) return;

            _verticalOffset = - verticalOffset;

            // draw background
            _buffer.Graphics.Clear(MetroTheme.PhoneBackgroundBrush);
            _bgImage.Draw(_drawingGraphics);

            // draw tiles
            var rect = new Rectangle(0, - verticalOffset, Size.Width, Size.Height);
            foreach (var e in Children)
            {
                if (e.Bounds.IntersectsWith(rect))
                    e.Draw(_drawingGraphics.CreateChild(
                        new Point(e.Location.X, e.Location.Y + verticalOffset)));
            }

            // draw buffer directly to screen
           _controlGraphics.DrawImage(_buffer.Image, 0, 0);
        }

        /// <summary>
        /// Handler for update event from child.
        /// Redraw only in non-freezed mode
        /// </summary>
        /// <param name="element"></param>
        protected override void OnUpdated(UIElement element)
        {
            if (FreezeUpdate) return;
            base.Updated(this);
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
                                    Duration = _animationDuration,
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
                                    Duration = _animationDuration,
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
                           Duration = _animationDuration,
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
                Duration = _animationDuration,
                From = x - 1000 + random.Next(1000 - x - 173),
                To = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                OnAnimationStop = () => { target.EntranceAnimation = null; },
            };
        }

        #endregion
    }
}
