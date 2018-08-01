using System;

using OpenTK;
using OpenTK.Graphics;

using Genus2D;
using Genus2D.GUI;
using Genus2D.Core;
using Genus2D.Graphics;
using Genus2D.GameData;


using TiledMapEditor.States;

namespace TiledMapEditor.UI
{
    public class NewMapPanel : Panel
    {
        private static NewMapPanel _instance = null;

        private TextField _nameField;
        private TextField _widthField, _heightField;
        private NumberControl _tilesetControl;
        private Button _createMapButton;
        
        public NewMapPanel(State state)
            : base((StateWindow.Instance.Width / 2) - 100, (StateWindow.Instance.Height / 2) - 150, 200, 300, BarMode.Close_Drag, state)
        {
            if (_instance != null)
                _instance.Close();
            _instance = this;

            this.SetPanelLabel("New Map");

            _nameField = new TextField(GetContentWidth() - 90, 10, 80, 40, state);

            _widthField = new TextField(GetContentWidth() - 90, 60, 80, 40, state);
            _widthField.SetText("10");
            
            _heightField = new TextField(GetContentWidth() - 90, 110, 80, 40, state);
            _heightField.SetText("10");

            _tilesetControl = new NumberControl(102, 160, state);
            _tilesetControl.SetMinimum(1);
            _tilesetControl.SetMaximum(TilesetData.TilesetCount());

            _createMapButton = new Button("Create Map", 10, GetContentHeight() - 50, GetContentWidth() - 20, 40, state);
            _createMapButton.OnTrigger += CreateMap;

            this.AddControl(_nameField);
            this.AddControl(_widthField);
            this.AddControl(_heightField);
            this.AddControl(_tilesetControl);
            this.AddControl(_createMapButton);
        }

        private void CreateMap()
        {
            try
            {
                int tileset = _tilesetControl.GetIndex() - 1;
                if (TilesetData.GetTileset(tileset) == null)
                {
                    this.Close();
                    return;
                }
                else if (!TilesetData.GetTileset(tileset).TextureLoaded())
                {
                    this.Close();
                    return;
                }

                string name = _nameField.GetText();
                name = name.Replace(" ", "");
                if (name == "") return;
                int width = int.Parse(_widthField.GetText());
                int height = int.Parse(_heightField.GetText());

                if (!MapInfo.AddMapInfo(name, width, height)) return;

                width = Math.Max(width, 10);
                height = Math.Max(height, 10);

                MapData mapData = new MapData(name, width, height, tileset);
                MapInfo.SaveMap(mapData);

                EditorState.Instance.GetMapPanel().SetMapData(mapData);
                EditorState.Instance.GetMapPanel().SetScrollDimensions(width * 32, height * 32);
                EditorState.Instance.GetTilesetPanel().SetTileset(tileset);
                this.Close();
            }
            catch
            {

            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            int baseX = GetContentWidth() - 90;
            string text = "Name:";
            Vector3 pos = new Vector3(baseX - Renderer.GetFont().GetTextWidth(text), 24, 0);
            Color4 colour = Color4.White;

            Renderer.PrintText(text, ref pos, ref colour);

            text = "Width:";
            pos.X = baseX - Renderer.GetFont().GetTextWidth(text);
            pos.Y += 50;
            Renderer.PrintText(text, ref pos, ref colour);

            text = "Height:";
            pos.X = baseX - Renderer.GetFont().GetTextWidth(text);
            pos.Y += 50;
            Renderer.PrintText(text, ref pos, ref colour);

            text = "Tileset:";
            pos.X = baseX - Renderer.GetFont().GetTextWidth(text);
            pos.Y += 50;
            Renderer.PrintText(text, ref pos, ref colour);
        }

        public override void Close()
        {
            base.Close();
            _instance = null;
            _createMapButton.OnTrigger -= CreateMap;
        }
    }
}
