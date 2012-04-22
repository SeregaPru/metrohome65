using Fleux.Controls;

namespace Fleux.UIElements.Pivot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Animations;
    using Controls.Gestures;
    using Core;
    using Core.GraphicsHelpers;
    using Styles;

    public class Pivot : Canvas
    {
        private readonly int headerPadding = 20;
        private List<PivotItem> pivotItems = new List<PivotItem>();
        private int headersWidth;
        private int offsetForPanning;
        private int offsetForHeaders;
        private int offsetForBody;
        private Canvas headers;
        private PivotItem currentItem;
        private bool currentFromLeft;
        private Canvas body;

        public Pivot(string title)
            : base()
        {
            // 1. Title Section
            base.AddElement(new TextElement(title)
            {
                Style = MetroTheme.PhoneTextPageTitle1Style,
                Location = new Point(24 - 3, 9)  // -3 is a correction for Segoe fonts
            });

            this.headers = new Canvas
                {
                    Location = new Point(21, 30)
                };

            this.body = new Canvas
            {
                Location = new Point(21, 150),
            };
            base.AddElement(this.headers);
            base.AddElement(this.body);
        }

        public PivotItem CurrentItem
        {
            get
            {
                return this.currentItem;
            }

            set
            {
                this.offsetForPanning = 0;
                var previousHeader = this.currentItem.Title;
                this.currentItem = value;
                var index = this.pivotItems.LastIndexOf(value);
                var beforeItems = this.pivotItems.GetRange(0, index);
                this.pivotItems.RemoveRange(0, index);
                this.pivotItems.AddRange(beforeItems);
                this.RefreshHeaders();
                this.AnimatePivotItemTransition(this.currentFromLeft, previousHeader);
            }
        }

        public new void AddElement(UIElement element)
        {
            // Hides AddElement method from Canvas
        }

        public void AddPivotItem(PivotItem item)
        {
            this.pivotItems.Add(item);
            this.body.Size = new Size(this.Size.Width - 21, this.Size.Height - 150);
            item.Body.Size = this.body.Size;
            item.Body.Location = new Point(0, 0);

            //! metrohome65
            item.Body.Parent = this;
            item.Body.ParentControl = this.ParentControl;

            this.RefreshHeaders();
            if (this.pivotItems.Count == 1)
            {
                this.currentItem = item;
            }
        }

        public override bool Pan(Point from, Point to, bool done, Point startPoint)
        {
            // Gianni, removed because on Click is mapped to MouseMove, with a strange behavior
#if !WindowsCE
            this.offsetForHeaders = 0;
            this.offsetForBody = 0;
            if (done && this.offsetForPanning != 0)
            {
                this.AnimateCurrentReposition();
            }
            else if (GesturesEngine.IsHorizontal(from, to))
            {
                this.offsetForPanning += (to.X - from.X) / 2;
                this.Update();
                return true;
            }

            return base.Pan(from, to, done, startPoint);
#else
            return true;
#endif
        }

        public override bool Flick(Point from, Point to, int millisecs, Point startPoint)
        {
            if (GesturesEngine.IsHorizontal(from, to))
            {
                if (this.pivotItems.Count > 1)
                {
                    if (to.X - from.X < 0)
                    {
                        this.CurrentItem = this.pivotItems[this.pivotItems.IndexOf(this.currentItem) + 1];
                    }
                    else
                    {
                        this.currentFromLeft = true;
                        this.CurrentItem = this.pivotItems.Last();
                    }
                }
                else
                {
                    this.AnimateCurrentReposition();
                }
            }
            else if (this.offsetForPanning != 0)
            {
                this.AnimateCurrentReposition();
            }

            return base.Flick(from, to, millisecs, startPoint);
        }

        public override void Draw(IDrawingGraphics drawingGraphics)
        {
            this.headers.Location = new Point(this.offsetForPanning + this.offsetForHeaders + 21, 30);
            this.body.Clear();
            this.CurrentItem.Body.Location = new Point(0, 0);
            this.CurrentItem.Body.Size = this.body.Size;
            this.body.Location = new Point(21 + this.offsetForBody + (this.offsetForPanning * 2), 150);
            this.body.AddElement(this.CurrentItem.Body);

            base.Draw(drawingGraphics);

            this.DrawHeaders(drawingGraphics.CreateChild(new Point(21 - this.headersWidth + this.offsetForHeaders + this.offsetForPanning, 30)));
        }

        private void AnimateCurrentReposition()
        {
            StoryBoard.BeginPlay(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 200,
                From = this.offsetForPanning,
                To = 0,
                OnAnimation = v => { this.offsetForPanning = v; this.Update(); }
            });
        }

        private void AnimatePivotItemTransition(bool fromLeft, string previousHeader)
        {
            this.currentFromLeft = false;
            StoryBoard.Play(new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                Duration = 300,
                From = fromLeft ? -this.Size.Width : this.CalculateHeaderWidth(previousHeader), // this.Size.Width,
                To = 0,
                OnAnimation = v => this.offsetForHeaders = v
            },
            new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluidExtended)
            {
                Duration = 600,
                From = fromLeft ? -this.Size.Width : this.Size.Width,
                To = 0,
                OnAnimation = v => this.offsetForBody = v
            },
            this.CurrentItem.GetDelayedAnimation(fromLeft ? -this.Size.Width : this.Size.Width),
            new CommitStoryboardAnimation
            {
                Duration = 600,
                CommitAction = this.Update
            });
        }

        private void DrawHeaders(IDrawingGraphics gr)
        {
            var style = MetroTheme.PhoneTextPageTitle2Style;

            foreach (var h in this.pivotItems)
            {
                style.Foreground = MetroTheme.PhoneSubtleBrush;
                gr.Style(style).DrawText(h.Title).MoveRelX(this.headerPadding);
            }
        }

        private void RefreshHeaders()
        {
            int x = 0;
            this.headers.Clear();
            foreach (var i in this.pivotItems)
            {
                var h = this.CreateHeader(i, this.CurrentItem);
                h.Location = new Point(x, 0);
                this.headers.AddElement(h);
                x += h.Size.Width + this.headerPadding;
            }

            this.headersWidth = x;
            // GIANNI added, to avoid headers repetition on wide screen
            if (this.headersWidth > this.Size.Width)
            {
                foreach (var i in this.pivotItems)
                {
                    var h = this.CreateHeader(i, null);
                    h.Location = new Point(x, 0);
                    this.headers.AddElement(h);
                    x += h.Size.Width + this.headerPadding;
                }
            }

            this.headers.Size = new Size(this.headersWidth, 80);
        }

        private TextElement CreateHeader(PivotItem i, PivotItem selected)
        {
            var style = MetroTheme.PhoneTextPageTitle2Style;
            style.Foreground = i == selected ? MetroTheme.PhoneForegroundBrush : MetroTheme.PhoneSubtleBrush;
            return new TextElement(i.Title)
            {
                Style = style,
                Size = new Size(this.CalculateHeaderWidth(i.Title), 80),
                TapHandler = p =>
                    {
                        if (this.currentItem != i)
                        {
                            this.CurrentItem = i;
                        }
                        return true;
                    }
            };
        }

        private int CalculateHeaderWidth(string p)
        {
            return FleuxApplication.DummyDrawingGraphics.MoveTo(0, 0)
                .Style(MetroTheme.PhoneTextPageTitle2Style)
                .DrawText(p)
                .Right;
        }


        //! metrohome65
        protected override void SetParentControl(FleuxControl parentControl)
        {
            base.SetParentControl(parentControl);
            this.pivotItems.ToList().ForEach(e => e.Body.ParentControl = parentControl);
        }

    }
}