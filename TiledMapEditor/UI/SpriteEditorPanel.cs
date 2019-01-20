using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.GUI;

namespace TiledMapEditor.UI
{
    public class SpriteEditorPanel : Panel
    {
        public SpriteEditorPanel(State state) 
            : base(((int)Renderer.GetResoultion().X / 2) - 200, ((int)Renderer.GetResoultion().Y / 2) - 200, 800, 800, BarMode.Close_Drag, state)
        {

        }
    }
}
