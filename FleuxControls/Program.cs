using System;
using Fleux.Core;

namespace FleuxControls
{
    static class Program
    {
        [MTAThread]
        static void Main()
        {
            FleuxApplication.TargetDesignDpi = 192; // Default HTC HD2 Res!
            FleuxApplication.Run(new SmsList());
        }
    }
}