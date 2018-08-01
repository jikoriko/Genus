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
    public class TilesetEditorPanel : Panel
    {
        private static TilesetEditorPanel _instance = null;

        private EditorState _editorState;

        private ListBox _tilesetList;
        private Button _addTilesetButton, _removeTilesetButton;

        private TextField _tilesetNameField, _textureNameField;

        private TilesetPropertiesPanel _propertiesPanel;
        private Button _editPassabilitiesButton;
        private Button _editPrioritiesButton;

        private Button _applyChangesButton;

        private TilesetData.Tileset _selectedTileset;

        public TilesetEditorPanel(EditorState state)
            : base((StateWindow.Instance.Width / 2) - 400, (StateWindow.Instance.Height / 2) - 300, 800, 600, BarMode.Close_Drag, state)
        {
            if (_instance != null)
                _instance.Close();
            _instance = this;
            _editorState = state;
            this.SetPanelLabel("Tileset Editor");

            _tilesetList = new ListBox(10, 10, 200, GetContentHeight() - 120, TilesetData.GetTilesetNames().ToArray(), state);
            _tilesetList.OnSelectionChange += SetTileset;

            _addTilesetButton = new Button("Add Tileset", 10, GetContentHeight() - 100, 200, 40, state);
            _addTilesetButton.OnTrigger += AddTilesetTrigger;

            _removeTilesetButton = new Button("Remove Tileset", 10, GetContentHeight() - 50, 200, 40, state);
            _removeTilesetButton.OnTrigger += RemoveTilesetTrigger;

            _tilesetNameField = new TextField(360, 20, 200, 24, state);
            _textureNameField = new TextField(360, 54, 200, 24, state);

            _propertiesPanel = new TilesetPropertiesPanel(this, _editorState);
            _editPassabilitiesButton = new Button("Passabilities", 704, 100, 80, 40, state);
            _editPassabilitiesButton.OnTrigger += SetEditPassabilities;
            _editPrioritiesButton = new Button("Priorities", 704, 150, 80, 40, state);
            _editPrioritiesButton.OnTrigger += SetEditPriorities;

            _applyChangesButton = new Button("Apply Changes", GetContentWidth() - 170, GetContentHeight() - 50, 160, 40, state);
            _applyChangesButton.OnTrigger += ApplyChangesTrigger;

            this.AddControl(_tilesetList);
            this.AddControl(_addTilesetButton);
            this.AddControl(_removeTilesetButton);

            this.AddControl(_tilesetNameField);
            this.AddControl(_textureNameField);

            this.AddControl(_propertiesPanel);
            this.AddControl(_editPassabilitiesButton);
            this.AddControl(_editPrioritiesButton);

            this.AddControl(_applyChangesButton);

            SetTileset(_tilesetList.GetSelection());
        }

        public TilesetData.Tileset GetSelectedTileset()
        {
            return _selectedTileset;
        }

        private void AddTilesetTrigger()
        {
            string name = "Tileset " + (_tilesetList.GetItems().Count + 1);
            _tilesetList.GetItems().Add(name);
            TilesetData.AddTileset(name);
        }

        private void RemoveTilesetTrigger()
        {
            int selection = _tilesetList.GetSelection();
            if (selection != -1)
            {
                _tilesetList.GetItems().RemoveAt(selection);
                TilesetData.RemoveTileset(selection);
            }
        }

        private void SetTileset(int index)
        {
            if (index > -1)
            {
                _selectedTileset = TilesetData.GetTileset(index);
                _tilesetNameField.SetText(_selectedTileset.Name);
                _textureNameField.SetText(_selectedTileset.TexturePath);
            }
            else
            {
                _selectedTileset = null;
                _tilesetNameField.SetText("");
                _textureNameField.SetText("");
            }
        }

        private void SetEditPassabilities()
        {
            _propertiesPanel.SetEditPropertyMode(TilesetPropertiesPanel.EditPropertyMode.Passabilities);
        }

        private void SetEditPriorities()
        {
            _propertiesPanel.SetEditPropertyMode(TilesetPropertiesPanel.EditPropertyMode.Priorities);
        }

        private void ApplyChangesTrigger()
        {
            int selection = _tilesetList.GetSelection();
            if (selection > -1)
            {
                _selectedTileset.Name = _tilesetNameField.GetText();
                _tilesetList.GetItems()[selection] = _tilesetNameField.GetText();
                _selectedTileset.SetTexturePath(_textureNameField.GetText());

                TilesetData.SaveData();
                SetTileset(selection);
            }
        }

        protected override void RenderContent()
        {
            base.RenderContent();

            Vector3 pos = new Vector3();
            Color4 colour = Color4.White;
            Renderer.PrintText("", ref pos, ref colour);
        }

        public override void Close()
        {
            base.Close();
            _instance = null;
            _addTilesetButton.OnTrigger -= AddTilesetTrigger;
            _applyChangesButton.OnTrigger -= ApplyChangesTrigger;
        }
    }
}
