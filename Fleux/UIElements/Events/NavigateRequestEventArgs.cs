// GIANNI added

namespace Fleux.UIElements.Events
{
    using System;
    using System.Drawing;
    using Fleux.Core;

    public class NavigateRequestEventArgs : EventArgs
    {
        public NavigateRequestEventArgs (FleuxPage destinationPage)
	    {
            DestinationPage = destinationPage;
	    }

        public FleuxPage DestinationPage { get; private set; }
    }
}
