﻿namespace Fleux.UIElements.Pivot
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Animations;
    using Fleux.UIElements.Events;

    public class PivotItem
    {
        #region Gianni - Events
        public event EventHandler<NavigateRequestEventArgs> NavigateRequest;

        protected void OnNavigateRequest(Fleux.Core.FleuxPage destinationPage)
        {
            EventHandler<NavigateRequestEventArgs> localHandler = NavigateRequest;
            if (localHandler != null)
                localHandler(this, new NavigateRequestEventArgs(destinationPage));
        }
        #endregion

        public PivotItem()
        {
            this.DelayInTransitions = new List<UIElement>();
        }

        public string Title { get; set; }

        public UIElement Body { get; set; }

        public List<UIElement> DelayInTransitions { get; private set; }

        public IAnimation GetDelayedAnimation(int initialDelta)
        {
            return new AnimationGroup(
                this.DelayInTransitions.Select(i => (IAnimation)new FunctionBasedAnimation(FunctionBasedAnimation.Functions.SoftedFluid)
                {
                    From = i.Location.X + (initialDelta / 4),
                    To = i.Location.X,
                    Duration = 600,
                    OnAnimation = v => i.Location = new Point(v, i.Location.Y)
                }));
        }
    }
}
