using System;
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
            // отключаем кеширование менеджера ресурсов, ибо несет утечки и глюки
            ResourceManager.Instance.Caching = false;

            FleuxApplication.TargetDesignDpi = 192; // Default HTC HD2 Res!
            FleuxApplication.Run(new MetroHome65.HomeScreen.HomeScreen());
        }
    }
}