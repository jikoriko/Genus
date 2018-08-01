using System;

using Genus2D;
using Genus2D.Core;

using System.Diagnostics;

namespace Genus2DTest
{
    class Program
    {

        static void Main(string[] args)
        {
            StateWindow window = new StateWindow(1000, 800, "Test", OpenTK.GameWindowFlags.Default);
            window.PushState(new TestState());
            window.Run(60.0);
            window.Dispose();
        }
    }
}
