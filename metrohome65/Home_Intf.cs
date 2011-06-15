using System;
using System.Drawing;

namespace MetroHome65
{
    interface IHost
    {
        /// <summary>
        /// Background area color
        /// </summary>
        Color BackgroundColor { get; }

        void ChangePage(bool Next);

    }
}
