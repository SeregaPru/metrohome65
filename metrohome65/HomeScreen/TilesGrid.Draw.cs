using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Fleux.Controls;
using Fleux.UIElements;
using Fleux.Core;
using Fleux.Animations;
using Fleux.UIElements.Panorama;
using WindowsPhone7Sample.Elements;

namespace MetroHome65.HomeScreen
{

    public partial class TilesGrid : ScrollViewer
    {

        private bool _launching = false;
        private WidgetWrapper _launchedTile = null;

        private UIElement SetEntranceAnimation(WidgetWrapper target, int targetPos )
        {
            var random = new Random();
            var x = targetPos;
            target.EntranceAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.BounceEntranceSin)
            {
                From = x - 1000 + random.Next(1000 - x - 173),
                To = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y)
            };
            return target;
        }

        private void SetExitAnimation(WidgetWrapper target)
        {
            var random = new Random();
            var x = target.Location.X;
            
            SetEntranceAnimation(target, x);

            target.ExitAnimation = new FunctionBasedAnimation(FunctionBasedAnimation.Functions.BounceExitSin)
            {
                To = -target.Size.Width - random.Next(1000),
                From = x,
                OnAnimation = v => target.Location = new Point(v, target.Location.Y),
                EaseFunction = v => Math.Pow(v, 15),
            };

            // click handler - run exit animation and then tile's OnClick method
            target.TapHandler = p => { WidgetClickAt(p, target); return true; };
        }

    }
}
