using System;
using System.Windows.Forms;
using Fleux.Core;

namespace MetroHome65
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            FleuxApplication.TargetDesignDpi = 192; // Default HTC HD2 Res!
            FleuxApplication.Run(new MetroHome65.HomeScreen.HomeScreen());
        }
    }
}