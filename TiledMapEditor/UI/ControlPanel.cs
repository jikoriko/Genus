using System;

using OpenTK;

using Genus2D;
using Genus2D.GUI;

using TiledMapEditor.States;
using OpenTK.Graphics;
using Genus2D.Graphics;
using Genus2D.Core;
using Genus2D.Utililities;
using Genus2D.GameData;

namespace TiledMapEditor.UI
{
    public class ControlPanel : Panel
    {
        private EditorState _editorState;

        private Button _newMapButton, _saveMapButton, _LoadMapButton, _editTilesetsButton;

        private NumberControl _layerControl;

        private Button _pencilToolButton, _rectangleToolButton, _floodFillToolButton, 
            _addMapEventButton, _eventsEditorButton;

        public ControlPanel(EditorState state)
            : base(0, 0, StateWindow.Instance.Width, 114, BarMode.Empty, state)
        {
            _editorState = state;

            _newMapButton = new Button("New Map", 10, 10, 140, 40, state);
            _newMapButton.OnTrigger += NewMapTrigger;
            
            _saveMapButton = new Button("Save Map", 10, 60, 140, 40, state);
            _saveMapButton.OnTrigger += SaveMapTrigger;
            
            _LoadMapButton = new Button("Load Map", 160, 10, 140, 40, state);
            _LoadMapButton.OnTrigger += LoadMapTrigger;

            _editTilesetsButton = new Button("Edit Tilesets", 160, 60, 140, 40, state);
            _editTilesetsButton.OnTrigger += EditTilesetsTrigger;

            _layerControl = new NumberControl(570, 60, state);
            _layerControl.SetMinimum(1);
            _layerControl.SetMaximum(3);

            _pencilToolButton = new Button("", 510, 10, 40, 40, state);
            _pencilToolButton.SetButtonImage(Assets.GetTexture("GUI_Textures/PencilIcon.png"));
            _pencilToolButton.SetImageColour(Color4.White);
            _pencilToolButton.OnTrigger += PencilToolTrigger;

            _rectangleToolButton = new Button("", 560, 10, 40, 40, state);
            _rectangleToolButton.SetButtonImage(Assets.GetTexture("GUI_Textures/RectangleIcon.png"));
            _rectangleToolButton.SetImageColour(Color4.White);
            _rectangleToolButton.OnTrigger += RectangleToolTrigger;
            _rectangleToolButton.SetBackgroundColour(Color4.WhiteSmoke);

            _floodFillToolButton = new Button("", 610, 10, 40, 40, state);
            _floodFillToolButton.SetButtonImage(Assets.GetTexture("GUI_Textures/BucketIcon.png"));
            _floodFillToolButton.SetImageColour(Color4.White);
            _floodFillToolButton.OnTrigger += FloodFillToolTrigger;
            _floodFillToolButton.SetBackgroundColour(Color4.WhiteSmoke);

            _addMapEventButton = new Button("", 660, 10, 40, 40, state);
            _addMapEventButton.SetButtonImage(Assets.GetTexture("GUI_Textures/EventIcon.png"));
            _addMapEventButton.SetImageColour(Color4.White);
            _addMapEventButton.OnTrigger += AddMapEventToolTrigger;
            _addMapEventButton.SetBackgroundColour(Color4.WhiteSmoke);

            _eventsEditorButton = new Button("Events", 800, 10, 80, 40, state);
            _eventsEditorButton.OnTrigger += EventsTrigger;

            this.AddControl(_newMapButton);
            this.AddControl(_saveMapButton);
            this.AddControl(_LoadMapButton);
            this.AddControl(_editTilesetsButton);
            this.AddControl(_layerControl);
            this.AddControl(_pencilToolButton);
            this.AddControl(_rectangleToolButton);
            this.AddControl(_floodFillToolButton);
            this.AddControl(_addMapEventButton);

            this.AddControl(_eventsEditorButton);
        }

        private void NewMapTrigger()
        {
            NewMapPanel panel = new NewMapPanel(_state);
            _state.AddControl(panel);
        }

        private void SaveMapTrigger()
        {
            MapData mapData = ((EditorState)_state).GetMapPanel().GetMapData();
            if (mapData != null)
                MapInfo.SaveMap(mapData);
        }

        private void LoadMapTrigger()
        {
            LoadMapPanel loadPanel = new LoadMapPanel(_state);
            _state.AddControl(loadPanel);
        }

        private void EditTilesetsTrigger()
        {
            TilesetEditorPanel panel = new TilesetEditorPanel(_editorState);
            _state.AddControl(panel);
        }

        public int GetLayer()
        {
            return _layerControl.GetIndex() - 1;
        }

        private void PencilToolTrigger()
        {
            _editorState.GetMapPanel().SetMapTool(MapPanel.MapTool.Pencil);

            _pencilToolButton.SetBackgroundColour(Color4.RoyalBlue);
            _rectangleToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _floodFillToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _addMapEventButton.SetBackgroundColour(Color4.WhiteSmoke);
        }

        private void RectangleToolTrigger()
        {
            _editorState.GetMapPanel().SetMapTool(MapPanel.MapTool.Rectangle);

            _pencilToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _rectangleToolButton.SetBackgroundColour(Color4.RoyalBlue);
            _floodFillToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _addMapEventButton.SetBackgroundColour(Color4.WhiteSmoke);
        }

        private void FloodFillToolTrigger()
        {
            _editorState.GetMapPanel().SetMapTool(MapPanel.MapTool.FloodFill);

            _pencilToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _rectangleToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _floodFillToolButton.SetBackgroundColour(Color4.RoyalBlue);
            _addMapEventButton.SetBackgroundColour(Color4.WhiteSmoke);
        }

        private void AddMapEventToolTrigger()
        {
            _editorState.GetMapPanel().SetMapTool(MapPanel.MapTool.AddMapEvent);

            _pencilToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _rectangleToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _floodFillToolButton.SetBackgroundColour(Color4.WhiteSmoke);
            _addMapEventButton.SetBackgroundColour(Color4.RoyalBlue);

        }

        private void EventsTrigger()
        {
            MapEventsPanel eventsPanel = new MapEventsPanel(_editorState);
            _state.AddControl(eventsPanel);
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = new Vector3(504, 74, 0);
            Color4 colour = Color4.White;

            Renderer.PrintText("Layer:", ref pos, ref colour);
        }
    }
}
