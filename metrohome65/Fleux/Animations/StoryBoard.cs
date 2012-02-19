﻿namespace Fleux.Animations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public class StoryBoard
    {
        private readonly List<IAnimation> animations = new List<IAnimation>();
        private static StoryBoard sb;
        private Thread animationThread;
        private bool stopAnimation;

        public static void Play(IAnimation animation)
        {
            var sb = new StoryBoard();
            sb.AddAnimation(animation);
            sb.AnimateSync();
        }

        public static void Play(params IAnimation[] animations)
        {
            var sb = new StoryBoard();
            sb.AddAnimations(animations);
            sb.AnimateSync();
        }

        public static void BeginPlay(params IAnimation[] animations)
        {
            bool processed = false;

            if (sb != null)
            {
                lock (sb)
                {
                    if (sb != null)
                    {
                        sb.AddAnimations(animations);
                        processed = true;
                    }
                }
            }
            if (!processed)
            {
                sb = new StoryBoard();
                sb.AddAnimations(animations);
                sb.BeginAnimate(() => sb = null);
            }
        }

        public void AddAnimation(IAnimation a)
        {
            this.animations.Add(a);
            if (this.animationThread != null)
            {
                a.Reset();
            }
        }

        public void BeginAnimate(Action onAnimateCompleted)
        {
            if (this.animationThread == null)
            {
                this.animationThread = new Thread(() =>
                {
                    this.AnimateSync();
                    onAnimateCompleted();
                    this.animationThread = null;
                });
                this.animationThread.Start();
            }
        }

        public void BeginAnimate()
        {
            this.BeginAnimate(() => { });
        }

        public void AnimateSync()
        {
            this.animations.ForEach(a => a.Reset());

            this.animations.ForEach(a => a.OnStart());
            System.Windows.Forms.Application.DoEvents();

            var keepAnimating = true;
            while (!this.stopAnimation && keepAnimating)
            {
                lock (this)
                {
                    keepAnimating = this.animations.Aggregate(false, (current, animation) => (animation.Animate() || current));
                }
                System.Windows.Forms.Application.DoEvents();
            }

            this.animations.ForEach(a => a.OnFinish());
            System.Windows.Forms.Application.DoEvents();

            this.stopAnimation = false;
        }

        public void CancelAsyncAnimate()
        {
            if (this.animationThread != null)
            {
                this.stopAnimation = true;
                this.animationThread.Join(1000);
                if (this.stopAnimation)
                {
                    // If the animation thread takes longer
                    // than 1 sec to stop, then we kill it
                    this.animationThread.Abort();
                    this.stopAnimation = false;
                }
                this.animationThread = null;
            }
        }

        public void AddAnimations(params IAnimation[] newAnimations)
        {
            lock (this)
            {
                foreach (var animation in newAnimations)
                {
                    this.AddAnimation(animation);
                }
            }
        }

        public void Clear()
        {
            this.animations.Clear();
        }
    }
}
