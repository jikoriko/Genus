using System;

using Genus2D;
using Genus2D.GUI;
using Genus2D.Core;
using Genus2D.GameData;

using TiledMapEditor.States;

namespace TiledMapEditor.UI
{
    public class LoadMapPanel : Panel
    {
        private DropDownBox _mapSelectionBox;
        private Button _loadButton;

        public LoadMapPanel(State state)
            : base((StateWindow.Instance.Width / 2) - 150, (StateWindow.Instance.Height / 2) - 150, 300, 300, BarMode.Close_Drag, state)
        {
            this.SetPanelLabel("Load Map");

            _mapSelectionBox = new DropDownBox(10, 10, GetContentWidth() - 20, MapInfo.GetMapInfoStrings().ToArray(), state);
            _mapSelectionBox.SetMaxItemsVisible(6);
            _loadButton = new Button("Load", 10, GetContentHeight() - 50, GetContentWidth() - 20, 40, state);
            _loadButton.OnTrigger += LoadMapTrigger;

            this.AddControl(_mapSelectionBox);
            this.AddControl(_loadButton);
        }

        private void LoadMapTrigger()
        {
            int selection = _mapSelectionBox.GetSelection();
            MapInfo.LoadMap(selection);
            MapData data = MapInfo.LoadMap(selection);

            EditorState.Instance.GetMapPanel().SetMapData(data);
            if (data != null)
            {
                EditorState.Instance.GetMapPanel().SetScrollDimensions(data.GetWidth() * 32, data.GetHeight() * 32);
                int tileset = data.GetTilesetID();
                EditorState.Instance.GetTilesetPanel().SetTileset(tileset);
            }

            this.Close();
        }

        public override void Close()
        {
            base.Close();
            _loadButton.OnTrigger -= LoadMapTrigger;
        }
    }
}
