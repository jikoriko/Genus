using System;

using Genus2D.Core;

using TiledMapEditor.UI;
using Genus2D.Graphics;

namespace TiledMapEditor.States
{
    public class EditorState : State
    {
        private static EditorState _instance = null;
        public static EditorState Instance { get { return _instance; } private set { _instance = value; } }

        private ControlPanel _controlPanel;
        private TilesetPanel _tilesetPanel;
        private MapPanel _mapPanel;

        public EditorState()
            : base()
        {
            Instance = this;

            Renderer.SetClearColour(new OpenTK.Graphics.Color4(0.25f, 0.25f, 0.25f, 1f));

            _controlPanel = new ControlPanel(this);
            _tilesetPanel = new TilesetPanel(this);
            _mapPanel = new MapPanel(this);

            this.AddControl(_controlPanel);
            this.AddControl(_tilesetPanel);
            this.AddControl(_mapPanel);
        }

        public ControlPanel GetControlPanel()
        {
            return _controlPanel;
        }

        public TilesetPanel GetTilesetPanel()
        {
            return _tilesetPanel;
        }

        public MapPanel GetMapPanel()
        {
            return _mapPanel;
        }
    }
}
