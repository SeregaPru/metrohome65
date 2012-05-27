// GIANNI added

namespace Fleux.UIElements.Events
{
    using System;

    public class ButtonTapEventArgs : EventArgs
    {
        public ButtonTapEventArgs(int buttonID)
	    {
            ButtonID = buttonID;
	    }

        public int ButtonID{ get; private set; }
    }
}
