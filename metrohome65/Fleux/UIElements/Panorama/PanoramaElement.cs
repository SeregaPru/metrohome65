namespace Fleux.UIElements.Panorama
{
    using System;
    using System.Drawing;
    using System.Linq;
    using Animations;
    using Core.GraphicsHelpers;
    using Styles;

    public class PanoramaElement : Canvas
    {
        protected IAnimation animation;
        protected int position = 0;
        protected int finePosition;
        protected bool isPanning;
        protected int sectionSpace = 425;
        protected int sectionCount;
        protected int currentSectionIndex;

        public PanoramaElement()
        {
            this.Background = new Canvas();
            this.Title = new Canvas();
            this.Sections = new Canvas { Location = new Point(0, 130), Size = new Size(1800, 600) };
            this.AddElement(this.Background);
            this.AddElement(this.Title);
            this.AddElement(this.Sections);

            this.SetAnimations();
        }

        public Canvas Background { get; set; }

        public Canvas Title { get; set; }

        public Canvas Sections { get; set; }

        public int FinePosition
        {
            get
            {
                return this.finePosition;
            }

            set
            {
                this.finePosition = value;
                this.CalculateFactors();
            }
        }

        public int CurrentSectionIndex
        {
            get
            {
                return this.currentSectionIndex;
            }

            set
            {
                if (value <= 0 && value > -this.sectionCount)
                {
                    this.currentSectionIndex = value;
                }
                this.StoryBoardPlay(
                    new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
                    {
                        From = this.finePosition,
                        To = -this.currentSectionIndex * this.sectionSpace,
                        Duration = 400,
                        OnAnimation = v => { this.FinePosition = v; this.Update(); }
                    });
            }
        }

        public void SetAnimations()
        {
            this.EntranceAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
            {
                From = -this.Sections.Size.Width,
                To = this.FinePosition,
                OnAnimation = v => this.FinePosition = v
            };
            this.ExitAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.CubicReverse)
            {
                From = this.FinePosition,
                To = -this.Sections.Size.Width,
                OnAnimation = v => this.FinePosition = v
            };
        }

        public override bool Flick(Point from, Point to, int millisecs, Point startPoint)
        {
            if (this.animation != null)
            {
                this.animation.Cancel();
            }
            if (!base.Flick(from, to, millisecs, startPoint) && (Math.Abs(to.X - from.X) > Math.Abs(to.Y - from.Y)))
            {
                this.CurrentSectionIndex += Math.Sign(to.X - from.X);
            }
            else if (this.isPanning)
            {
                this.isPanning = false;
                this.CurrentSectionIndex = this.CurrentSectionIndex;
            }

            return true;
        }

        public override bool Pan(Point from, Point to, bool done, Point startPoint)
        {
            if (this.animation != null)
            {
                this.animation.Cancel();
            }
            if (!base.Pan(from, to, done, startPoint))
            {
                this.isPanning = !done;

                // Validate if should we handle this Pan
                if (Math.Abs(to.X - from.X) > Math.Abs(to.Y - from.Y))
                {
                    this.FinePosition -= to.X - from.X;
                    this.Update();
                }
                if (done)
                {
                    this.CurrentSectionIndex = this.CurrentSectionIndex;
                }
            }
            return true;
        }

        public void AddSection(PanoramaSection newSection)
        {
            this.AddSection(newSection, false);
        }

        public void AddSection(PanoramaSection newSection, bool wider)
        {
            newSection.Size = new Size(380 * (wider ? 2 : 1), this.Size.Height - this.Sections.Location.Y);
            newSection.Location = new Point(this.sectionSpace * this.sectionCount, 0);
            this.sectionCount += wider ? 2 : 1;
            this.Sections.AddElement(newSection);
            this.SetAnimations();
        }

        protected void CalculateFactors()
        {
            var titlef = (double)(this.Title.Size.Width - this.Size.Width) / (double)(this.Sections.Size.Width - this.Size.Width);
            var backgroundf = (double)(this.Background.Size.Width - this.Size.Width) / (double)(this.sectionSpace * (this.sectionCount - 1));
            this.Background.Location = new Point((int)(-this.finePosition * backgroundf), 0);
            this.Title.Location = new Point((int)(-this.finePosition * titlef), this.Title.Location.Y);
            this.Sections.Location = new Point((int)(-this.finePosition * 1), this.Sections.Location.Y);
        }

        private void StoryBoardPlay(IAnimation animation)
        {
            if (this.animation != null)
            {
                this.animation.Cancel();
            }
            this.animation = animation;
            StoryBoard.BeginPlay(this.animation);
        }
    }
}
