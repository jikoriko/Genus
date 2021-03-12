using System;

using Genus2D.Core;

namespace RpgGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Genus2D.GameData.Preloader.PreLoadData();
            StateWindow window = new StateWindow(1200, 800, "RPG Game", OpenTK.GameWindowFlags.FixedWindow);
            window.PushState(new States.LoginState());
            window.Run(60.0);
            window.Dispose();
        }
        
    }
}
