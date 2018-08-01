using System;

using Genus2D;
using Genus2D.Core;

namespace TiledMapEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            StateWindow window = new StateWindow(1800, 1000, "Tiled Map Editor", OpenTK.GameWindowFlags.Default);
            window.PushState(new States.EditorState());
            window.Run(60.0);
            window.Dispose();
        }
    }
}
