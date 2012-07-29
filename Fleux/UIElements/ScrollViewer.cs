using System.Drawing.Imaging;
using Fleux.Controls;
using Fleux.Styles;

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
        private IGestureScrollingBehavior _horizontalInertia;
        private IGestureScrollingBehavior _verticalInertia;
        private bool _lastGestureWasHorizontal = false;
        private UIElement _content;
        private Bitmap _clipBitmap;
        //!!private IImageWrapper _imageScrollBar;

        //! Fork: fleuxdesktop2, Change Set 8b81eb940370
        //!!private bool panInProgress = false;
        //!!private int panV = 0;
        //!!private int panH = 0; 

        public ScrollViewer()
        {
            this.EntranceAnimation = new ForwarderAnimation(() => this._content.EntranceAnimation);
            this.ExitAnimation = new ForwarderAnimation(() => this._content.ExitAnimation);
        }

        ~ScrollViewer()
        {
            if (this._horizontalInertia != null)
                this._horizontalInertia.Dispose();

            if (this._verticalInertia != null)
                this._verticalInertia.Dispose();
            
            if (_clipBitmap != null)
                _clipBitmap.Dispose();
            
            //!!if (this._imageScrollBar != null)
            //!!    this._imageScrollBar.Dispose();
        }

        public UIElement Content
        {
            get
            {
                return this._content;
            }

            set
            {
                //! Fork: fleuxdesktop2, Change Set bede1dc701a9
                // allow content recreation
                if (this._content != null)
                {
                    this._content.ParentControl = null; //! MetroHome65
                    this.Children.Remove(this._content);
                }

                this._content = value;
                this._content.Updated = this.OnUpdated;
                this._content.Parent = this;
                this.Children.Add(this._content);

                //! Fork: fleuxdesktop2, Change Set e692d2097a32
                this._horizontalInertia = null;
                this._verticalInertia = null; 

                //! MetroHome65
                this._content.ParentControl = this.ParentControl;
                this._content.SizeChanged += (v, e) => OnContentSizeChanged();
            }
        }

        //! MetroHome65
        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);
            if (this._content != null)
                this._content.ParentControl = parentControl;
        }

        private void OnContentSizeChanged()
        {
            if (this._verticalInertia != null)
                this._verticalInertia.Min = -Math.Max(0, this._content.Size.Height - this.Size.Height);
            if (this._horizontalInertia != null)
                this._horizontalInertia.Min = -Math.Max(0, this._content.Size.Width - this.Size.Width);
        }

        public bool DrawShadows { get; set; }

        //! Fork: fleuxdesktop2, Change Set 8b81eb940370
        public int HorizontalOffset
        {
            get { return this._content.Location.X; }
            set {
                this._content.Location = new Point(value, this._content.Location.Y);
                //!!if (!panInProgress){
                //!!    panH = value;
                //!!}
            }
        }

        //! Fork: fleuxdesktop2, Change Set 8b81eb940370
        public int VerticalOffset
        {
            get { return this._content.Location.Y; }
            set {
                this._content.Location = new Point(this._content.Location.X, value);
                //!!if (!panInProgress){
                //!!    panV = value;
                //!!}
            }         
        }

        public bool HorizontalScroll { get; set; }

        public bool VerticalScroll { get; set; }

        public bool ShowScrollbars { get; set; }

        //! Fork: fleuxdesktop2, Change Set 8b81eb940370
        /**
          If true, vertical 'overscroll' when panning and flicking will be disabled
        */
        public bool TrimVerticalPanning = false;

        //! Fork: fleuxdesktop2, Change Set 8b81eb940370
        /**
          If true, horizontal 'overscroll' when panning and flicking will be disabled
        */
        public bool TrimHorizontalPanning = false;
        

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            if (this._content == null) return;
                
            if (this._clipBitmap == null)
            {
                this._clipBitmap = new Bitmap(this.Size.Width.ToPixels(), this.Size.Height.ToPixels(), PixelFormat.Format16bppRgb565);
            }

            using (var clipBuffer = drawingGraphics.GetClipBuffer(
                new Rectangle(0, 0, this.Size.Width, this.Size.Height), this._clipBitmap))
            {
                this.Content.Draw(
                    clipBuffer.DrawingGr.CreateChild(new Point(this.HorizontalOffset, this.VerticalOffset),
                                                     this._content.TransformationScaling,
                                                     this._content.TransformationCenter));
                if (this.ShowScrollbars)
                {
                    this.DoDrawScrollBar(clipBuffer.DrawingGr);
                }
            }

            if (this.DrawShadows)
            {
                DoDrawShadows(drawingGraphics);
            }
        }

        protected void DoDrawShadows(IDrawingGraphics drawingGraphics)
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

        protected void DoDrawScrollBar(IDrawingGraphics drawingGraphics)
        {
            if (this.Content.Size.Height == 0) return;

            //if (_imageScrollBar == null)
            //    _imageScrollBar = ResourceManager.Instance.GetIImageFromEmbeddedResource(
            //        "verticalscrollbar.png", Assembly.GetExecutingAssembly());
            //drawingGraphics.DrawAlphaImage(_imageScrollBar, new Rectangle(this.Size.Width - 5, 0, 5, this.Size.Height));
            drawingGraphics.Color(Color.Silver).
                FillRectangle(new Rectangle(this.Size.Width - 5, 0, 5, this.Size.Height));

            var scrollHeight = Math.Max(this.Size.Height * this.Size.Height / this.Content.Size.Height, 20);
            var scrollBegin = this.Size.Height * -this.VerticalOffset / this.Content.Size.Height;
            drawingGraphics.Color(MetroTheme.PhoneForegroundBrush).
                FillRectangle(new Rectangle(this.Size.Width - 5, scrollBegin, 5, scrollHeight));
        }

        public override bool Pan(Point from, Point to, bool done, Point startPoint)
        {
            // Gianni, removed because on Click is mapped to MouseMove, with a strange behavior
#if !WindowsCE
            var directionDelta = Math.Abs(to.X - from.X) - Math.Abs(to.Y - from.Y);
            var isHorizontal = directionDelta == 0 ? this._lastGestureWasHorizontal : directionDelta > 0;

            //! Fork: fleuxdesktop2, Change Set 8b81eb940370
            //!!panInProgress = !done; 

            if (this._horizontalInertia != null && this.HorizontalScroll && !isHorizontal)
            {
                this._horizontalInertia.Pan(0, 0, done);
                return false; //!?? Fork: fleuxdesktop2, Change Set 8b81eb940370
            }
            if (this._verticalInertia != null && this.VerticalScroll && isHorizontal)
            {
                this._verticalInertia.Pan(0, 0, done);
                return false; //!?? Fork: fleuxdesktop2, Change Set 8b81eb940370
            }
            if ((isHorizontal && !this.HorizontalScroll)
                || (!isHorizontal && !this.VerticalScroll))
            {
                return false;
            }

            this._lastGestureWasHorizontal = isHorizontal;

            this.TryCreateInertia();

            if (this._horizontalInertia != null)
            {
                this._horizontalInertia.Pan(from.X, to.X, done);
                to.X = from.X;
            }
            if (this._verticalInertia != null)
            {
                this._verticalInertia.Pan(from.Y, to.Y, done);
                to.Y = from.Y;
            }
#endif
            return true;
        }

        public override bool Flick(Point from, Point to, int millisecs, Point startPoint)
        {
            this.TryCreateInertia();

            //! Fork: fleuxdesktop2, Change Set 8b81eb940370
            //!!panInProgress = false;

            // Validate if should we handle this Flick
            var directionDelta = Math.Abs(to.X - from.X) - Math.Abs(to.Y - from.Y);
            var isHorizontal = directionDelta == 0 ? this._lastGestureWasHorizontal : directionDelta > 0;
            if ((isHorizontal && !this.HorizontalScroll)
                || (!isHorizontal && !this.VerticalScroll))
            {
                return false;
            }

            this._lastGestureWasHorizontal = isHorizontal;

            if (this._horizontalInertia != null)
            {
                this._horizontalInertia.Flick(from.X, to.X, millisecs);
                to.X = from.X;
            }
            if (this._verticalInertia != null)
            {
                this._verticalInertia.Flick(from.Y, to.Y, millisecs);
                to.Y = from.Y;
            }
            return true;
        }

        public override UIElement Pressed(Point p)
        {
            if (this._horizontalInertia != null && this.HorizontalScroll)
            {
                this._horizontalInertia.Pressed();
            }

            if (this._verticalInertia != null && this.VerticalScroll)
            {
                this._verticalInertia.Pressed();
            }

            return base.Pressed(p);
        }

        private void TryCreateInertia()
        {
            if (this.Size.Height > 0 && this._content.Size.Height > 0)
            {
                if (this.HorizontalScroll && this._horizontalInertia == null)
                {
                    this._horizontalInertia = new GestureInertiaBehavior(v =>
                                                                          {
                                                                              this.HorizontalOffset = v;
                                                                              this.Update();
                                                                          })
                                            {
                                                OnAnimationStart = OnStartScroll,
                                                OnAnimationStop = OnStopScroll,
                                                Min = -Math.Max(0, this._content.Size.Width - this.Size.Width),
                                                Max = 0,
                                                //! Fork: fleuxdesktop2, Change Set 8b81eb940370
                                                TrimPanning = TrimHorizontalPanning, 
                                            };
                }
                if (this.VerticalScroll && this._verticalInertia == null)
                {
                    this._verticalInertia = new GestureInertiaBehavior(v =>
                                                                        {
                                                                            this.VerticalOffset = v;
                                                                            this.Update();
                                                                        })
                                            {
                                                OnAnimationStart = OnStartScroll,
                                                OnAnimationStop = OnStopScroll,
                                                Min = -Math.Max(0, this._content.Size.Height - this.Size.Height),
                                                Max = 0,
                                                //! Fork: fleuxdesktop2, Change Set 8b81eb940370
                                                TrimPanning = TrimVerticalPanning,
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
