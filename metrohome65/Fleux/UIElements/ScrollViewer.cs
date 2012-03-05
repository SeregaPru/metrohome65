namespace Fleux.UIElements
{
    using System;
    using System.Drawing;
    using Animations;
    using Controls.Scrolling;
    using Core.GraphicsHelpers;
    using Core.Scaling;

    public class ScrollViewer : UIElement
    {
        private IGestureScrollingBehavior horizontalInertia;
        private IGestureScrollingBehavior verticalInertia;
        private bool lastGestureWasHorizontal = false;
        private UIElement content;
        private Bitmap clipBitmap;

        public ScrollViewer()
        {
            this.EntranceAnimation = new ForwarderAnimation(() => this.content.EntranceAnimation);
            this.ExitAnimation = new ForwarderAnimation(() => this.content.ExitAnimation);
        }

        ~ScrollViewer()
        {
            if (this.horizontalInertia != null)
            {
                this.horizontalInertia.Dispose();
            }
            if (this.verticalInertia != null)
            {
                this.verticalInertia.Dispose();
            }
        }

        public UIElement Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
                this.content.Updated = this.Update;
                this.content.Parent = this;
                this.Children.Add(this.content);

                this.content.SizeChanged += (v, e) => OnContentSizeChanged();
            }
        }

		private void OnContentSizeChanged()
        {
            if (this.verticalInertia != null)
                this.verticalInertia.Min = -Math.Max(0, this.content.Size.Height - this.Size.Height);
            if (this.horizontalInertia != null)
                this.horizontalInertia.Min = -Math.Max(0, this.content.Size.Width - this.Size.Width);
        }
        public bool DrawShadows { get; set; }

        public int HorizontalOffset { get; set; }

        public int VerticalOffset
        {
            get { return this.content.Location.Y; }
            set { this.content.Location = new Point(this.content.Location.X, value); }
        }

        public bool HorizontalScroll { get; set; }

        public bool VerticalScroll { get; set; }

        public bool ShowScrollbars { get; set; }

        public override Rectangle Bounds
        {
            get
            {
                return base.Bounds;
            }
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (this.clipBitmap == null)
            {
                this.clipBitmap = new Bitmap(this.Size.Width.ToPixels(), this.Size.Height.ToPixels());
            }
            using (var clipBitmap = drawingGraphics.GetClipBuffer(new Rectangle(0, 0, this.Size.Width, this.Size.Height), this.clipBitmap))
            {
                this.Content.Draw(clipBitmap.DrawingGr.CreateChild(new Point(this.HorizontalOffset, this.VerticalOffset), this.content.TransformationScaling, this.content.TransformationCenter));
                if (this.ShowScrollbars)
                {
                    this.DrawScrollBar(clipBitmap.DrawingGr);
                }
            }

            if (this.DrawShadows)
            {
                if (this.VerticalOffset < 0)
                {
                    drawingGraphics.DrawAlphaImage("top.png",
                                  new Rectangle(0, 0, this.Size.Width, 15));
                }
                if (this.VerticalOffset > Math.Min(0, -this.Content.Size.Height + this.Size.Height))
                {
                    drawingGraphics.DrawAlphaImage("bottom.png",
                                  new Rectangle(0, this.Size.Height - 15, this.Size.Width, 15));
                }
            }
        }

        public void DrawScrollBar(IDrawingGraphics drawingGr)
        {
            drawingGr.DrawAlphaImage("verticalscrollbar.png", new Rectangle(this.Size.Width - 5, 0, 5, this.Size.Height));
            var scrollHeight = Math.Max(this.Size.Height * this.Size.Height / this.Content.Size.Height, 20);
            var scrollBegin = this.Size.Height * -this.VerticalOffset / this.Content.Size.Height;
            drawingGr.Color(Color.White).FillRectangle(new Rectangle(this.Size.Width - 5, scrollBegin, 5, scrollHeight));
        }

        public override bool Pan(Point from, Point to, bool done, Point startPoint)
        {
            // Gianni, removed because on Click is mapped to MouseMove, with a strange behavior
#if !WindowsCE
            var directionDelta = Math.Abs(to.X - from.X) - Math.Abs(to.Y - from.Y);
            var isHorizontal = directionDelta == 0 ? this.lastGestureWasHorizontal : directionDelta > 0;
            if (this.horizontalInertia != null && this.HorizontalScroll && !isHorizontal)
            {
                this.horizontalInertia.Pan(0, 0, done);
                return false;
            }
            if (this.verticalInertia != null && this.VerticalScroll && isHorizontal)
            {
                this.verticalInertia.Pan(0, 0, done);
                return false;
            }
            if ((isHorizontal && !this.HorizontalScroll)
                || (!isHorizontal && !this.VerticalScroll))
            {
                return false;
            }

            this.TryCreateInertia();

            if (this.horizontalInertia != null)
            {
                this.horizontalInertia.Pan(from.X, to.X, done);
                to.X = from.X;
            }
            if (this.verticalInertia != null)
            {
                this.verticalInertia.Pan(from.Y, to.Y, done);
                to.Y = from.Y;
            }
#endif
            return true;
        }

        public override bool Flick(Point from, Point to, int millisecs, Point startPoint)
        {
            this.TryCreateInertia();

            // Validate if should we handle this Flick
            var directionDelta = Math.Abs(to.X - from.X) - Math.Abs(to.Y - from.Y);
            var isHorizontal = directionDelta == 0 ? this.lastGestureWasHorizontal : directionDelta > 0;
            if ((isHorizontal && !this.HorizontalScroll)
                || (!isHorizontal && !this.VerticalScroll))
            {
                return false;
            }

            if (this.horizontalInertia != null)
            {
                this.horizontalInertia.Flick(from.X, to.X, millisecs);
                to.X = from.X;
            }
            if (this.verticalInertia != null)
            {
                this.verticalInertia.Flick(from.Y, to.Y, millisecs);
                to.Y = from.Y;
            }
            return true;
        }

        public override UIElement Pressed(Point p)
        {
            if (this.horizontalInertia != null && this.HorizontalScroll)
            {
                this.horizontalInertia.Pressed();
            }

            if (this.verticalInertia != null && this.VerticalScroll)
            {
                this.verticalInertia.Pressed();
            }

            return base.Pressed(p);
        }

        private void TryCreateInertia()
        {
            if (this.Size.Height > 0 && this.content.Size.Height > 0)
            {
                if (this.HorizontalScroll && this.horizontalInertia == null)
                {
                    this.horizontalInertia = new GestureInertiaBehavior(v =>
                                                                          {
                                                                              this.HorizontalOffset = v;
                                                                              this.Update();
                                                                          })
                                            {
                                                OnAnimationStart = OnStartScroll,
                                                OnAnimationStop = OnStopScroll,
                                                Min = -Math.Max(0, this.content.Size.Width - this.Size.Width),
                                                Max = 0
                                            };
                }
                if (this.VerticalScroll && this.verticalInertia == null)
                {
                    this.verticalInertia = new GestureInertiaBehavior(v =>
                                                                        {
                                                                            this.VerticalOffset = v;
                                                                            this.Update();
                                                                        })
                                            {
                                                OnAnimationStart = OnStartScroll,
                                                OnAnimationStop = OnStopScroll,
                                                Min = -Math.Max(0, this.content.Size.Height - this.Size.Height),
                                                Max = 0
                                            };
                }
            }
        }

        // on start scroll animation
        public Action OnStartScroll { get; set; }

        // on stop scroll animation
        public Action OnStopScroll { get; set; }
    }
}
