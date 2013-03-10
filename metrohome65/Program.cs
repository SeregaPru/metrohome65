using System;
using Fleux.Core;
using MetroHome65.Routines;

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
            new PrepareEnvironment().Prepare();

            // отключаем кеширование менеджера ресурсов, ибо несет утечки и глюки
            ResourceManager.Instance.Caching = false;

            Extensions.InitLocalization();

            FleuxApplication.TargetDesignDpi = 192; // Default HTC HD2 Res!
            FleuxApplication.Run(new MetroHome65.HomeScreen.HomeScreen());
        }
    }
}