using System;
using System.Drawing;
using System.Linq;
using Fleux.Animations;
using Fleux.UIElements;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class TilesCanvas : 
        FreezedCanvas  
        //BufferedCanvas
    {

        public void DeleteElement(UIElement element)
        {
            element.Parent = null;
            element.Updated = null;
            Children.Remove(element);
        }

        private int _verticalOffset;
        private int _animationDuration = 500;

        public override void Draw(Fleux.Core.GraphicsHelpers.IDrawingGraphics drawingGraphics)
        {
            _verticalOffset = drawingGraphics.VisibleRect.Top;
            base.Draw(drawingGraphics);
        }

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
                    SetExitAnimation(tileWrapper);
                    SetEntranceAnimation(tileWrapper);

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
                                    OnAnimation = v => Update(),
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
                                    OnAnimation = v => Update(),
                                });
            sb.AnimateSync();

            FreezeUpdate = true;
        }

        private void SetExitAnimation(TileWrapper target)
        {
            var random = new Random();
            target.ExitAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = _animationDuration,
                To = -target.Size.Width - random.Next(1000),
                From = target.Bounds.Left,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
            };
        }

        private void SetEntranceAnimation(TileWrapper target)
        {
            var random = new Random();
            var x = target.GetScreenRect().Left; // here use GetScreenRect instead Bounds because real coords are wrong
            target.EntranceAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.Linear)
            {
                Duration = _animationDuration,
                From = x - 1000 + random.Next(1000 - x - 173),
                To = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
            };
        }

    }
}
