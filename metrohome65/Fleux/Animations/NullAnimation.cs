namespace Fleux.Animations
{
    using System;

    public class NullAnimation : IAnimation
    {
        public int Duration { get; set; }

        public void Reset()
        {
        }

        public bool Animate()
        {
            return false;
        }

        public void Cancel()
        {
        }

        public Action OnAnimationStop { get; set; }
        public Action OnAnimationStart { get; set; }

    }
}
