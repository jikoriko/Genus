using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public partial class EditorForm : Form
    {

        public static EditorForm Instance { private set; get; }

        public TilesetSelectionPanel tilesetSelectionPanel;
        public MapPanel mapPanel;
        public TilesetDataPanel tilesetDataPanel;
        public SpriteViewerPanel spriteViewerPanel;
        public IconSheetPanel iconSheetPanel;

        private List<ComboBox> _autoTileSelections;
        private List<NumericUpDown> _autoTimers;

        public enum MapTool
        {
            Pencil, Rectangle, FloodFill, Event, SpawnPoint, None
        }

        public enum TilesetProperties
        {
            Passabilities, Passabilities8Dir, Priorities, TerrainTags, BushFlags, CounterFlags, None
        }

        public EditorForm()
        {
            Instance = this;
            InitializeComponent();

            tilesetSelectionPanel = new TilesetSelectionPanel();
            tilesetSelectionPanel.Size = splitContainer2.Panel1.Size;

            mapPanel = new MapPanel();
            mapPanel.Size = splitContainer2.Panel2.Size;

            tilesetDataPanel = new TilesetDataPanel();
            tilesetDataPanel.Size = panel2.Size;

            spriteViewerPanel = new SpriteViewerPanel();
            spriteViewerPanel.Size = SpriteViewerParent.Size;

            iconSheetPanel = new IconSheetPanel();
            iconSheetPanel.Size = panel1.Size;

            splitContainer2.Panel1.Controls.Add(tilesetSelectionPanel);
            splitContainer2.Panel2.Controls.Add(mapPanel);
            panel2.Controls.Add(tilesetDataPanel);
            panel1.Controls.Add(iconSheetPanel);
            SpriteViewerParent.Controls.Add(spriteViewerPanel);

            PencilButton.Checked = true;
            PassabilitiesButton.Checked = true;

            _autoTileSelections = new List<ComboBox>();
            _autoTileSelections.Add(AutoTileSelection1);
            _autoTileSelections.Add(AutoTileSelection2);
            _autoTileSelections.Add(AutoTileSelection3);
            _autoTileSelections.Add(AutoTileSelection4);
            _autoTileSelections.Add(AutoTileSelection5);
            _autoTileSelections.Add(AutoTileSelection6);
            _autoTileSelections.Add(AutoTileSelection7);

            _autoTimers = new List<NumericUpDown>();
            _autoTimers.Add(AutoTimer1);
            _autoTimers.Add(AutoTimer2);
            _autoTimers.Add(AutoTimer3);
            _autoTimers.Add(AutoTimer4);
            _autoTimers.Add(AutoTimer5);
            _autoTimers.Add(AutoTimer6);
            _autoTimers.Add(AutoTimer7);

            tabControl1.SelectedIndexChanged += ChangeTabIndex;
            InitializeDataPanels();
        }

        private void ChangeTabIndex(object sender, EventArgs e)
        {
            int index = tabControl1.SelectedIndex;
            switch (index)
            {
                case 2:
                    PopulateEventsList();
                    break;
            }
        }

        private void InitializeDataPanels()
        {
            PopulateTilesetsList();
            PopulateTilesetSelections();
            PopulateAutoTileSelections();
            PopulateEventsList();
            PopulateSpritesList();
            PopulateSpriteSelections();
            PopulateItemList();
            PopulateItemIconSelections();
            PopulateSystemVariables();
        }

        #region Map Functions

        public MapTool GetMapTool()
        {
            if (PencilButton.Checked) return MapTool.Pencil;
            else if (RectangleButton.Checked) return MapTool.Rectangle;
            else if (FloodFillButton.Checked) return MapTool.FloodFill;
            else if (EventButton.Checked) return MapTool.Event;
            else if (SpawnButton.Checked) return MapTool.SpawnPoint;
            return MapTool.None;
        }

        public int GetMapLayer()
        {
            return (int)LayerControl.Value - 1;
        }

        public void SetMapData(Genus2D.GameData.MapData mapData, int mapID)
        {
            mapPanel.SetMapData(mapData, mapID);
            mapPanel.Refresh();

        }

        private void PopulateMapTilesetSelection()
        {
            int selection = MapTilesetSelection.SelectedIndex;
            MapTilesetSelection.Items.Clear();
            int tilesetCount = Genus2D.GameData.TilesetData.TilesetCount();
            mapPanel.ClearTilesetImages();
            for (int i = 0; i < tilesetCount; i++)
            {
                Genus2D.GameData.TilesetData.Tileset tileset = Genus2D.GameData.TilesetData.GetTileset(i);
                MapTilesetSelection.Items.Add(tileset.Name);
                mapPanel.AddTilesetImage(tileset.ImagePath);
            }
            
            if (selection == -1 && tilesetCount > 0)
                selection = 0;
            if (selection < tilesetCount && selection != -1)
            {
                MapTilesetSelection.SelectedIndex = selection;
                SelectMapTileset(selection);
            }
            else
            {
                SelectMapTileset(-1);
            }

        }

        private void MapTilesetSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = MapTilesetSelection.SelectedIndex;
            SelectMapTileset(selection);
        }

        private void SelectMapTileset(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.TilesetData.Tileset tileset = Genus2D.GameData.TilesetData.GetTileset(index);
                tilesetSelectionPanel.SetTileset(tileset);
            }
            else
            {
                tilesetSelectionPanel.SetTileset(null);
            }
        }

        public int GetSelectedMapTileset()
        {
            return MapTilesetSelection.SelectedIndex;
        }

        private void NewMapButton_Click(object sender, EventArgs e)
        {
            NewMapForm form = new NewMapForm(this);
            form.ShowDialog(this);
        }

        private void LoadMapButton_Click(object sender, EventArgs e)
        {
            if (Genus2D.GameData.MapInfo.NumberMaps() == 0)
            {
                MessageBox.Show("No maps exist.");
                return;
            }

            LoadMapForm form = new LoadMapForm(this);
            form.ShowDialog(this);

        }

        private void SaveMapButton_Click(object sender, EventArgs e)
        {
            if (mapPanel.GetMapData() != null)
            {
                Genus2D.GameData.MapInfo.SaveMap(mapPanel.GetMapData());
            }
        }

        private void ClearLayerButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.MapData mapData = mapPanel.GetMapData();
            if (mapData != null)
            {
                int layer = GetMapLayer();
                for (int x = 0; x < mapData.GetWidth(); x++)
                {
                    for (int y = 0; y < mapData.GetHeight(); y++)
                    {
                        mapData.SetTile(layer, x, y, 0, -1);
                    }
                }
                mapPanel.Refresh();
            }
        }

        private void EditMapButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.MapData mapData = mapPanel.GetMapData();
            if (mapData != null)
            {
                EditMapForm form = new EditMapForm(this, mapData);
                form.ShowDialog(this);
            }
        }

        #endregion

        #region Tileset Functions

        private void PopulateTilesetsList()
        {
            List<string> tilesets = Genus2D.GameData.TilesetData.GetTilesetNames();
            int index = TilesetsList.SelectedIndex;
            TilesetsList.Items.Clear();
            TilesetsList.Items.AddRange(tilesets.ToArray());
            if (index < TilesetsList.Items.Count)
                TilesetsList.SelectedIndex = index;
            PopulateMapTilesetSelection();
        }

        private void PopulateTilesetSelections()
        {
            if (Directory.Exists("Assets/Textures/Tilesets"))
            {
                int selection = TilesetSelectionBox.SelectedIndex;
                TilesetSelectionBox.Items.Clear();
                string[] files = Directory.GetFiles("Assets/Textures/Tilesets", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    TilesetSelectionBox.Items.Add(Path.GetFileName(files[i]));
                }
                TilesetSelectionBox.SelectedIndex = selection;
            }
        }

        private void TilesetsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTileset();
        }

        private void ChangeTileset()
        {
            Genus2D.GameData.TilesetData.Tileset tileset = GetSelectedTileset();
            if (tileset != null)
            {
                TilesetNameBox.Text = tileset.Name;
                for (int i = 0; i < TilesetSelectionBox.Items.Count; i++)
                {
                    if (TilesetSelectionBox.Items[i].ToString() == tileset.ImagePath)
                    {
                        TilesetSelectionBox.SelectedIndex = i;
                        break;
                    }
                    if (i == TilesetSelectionBox.Items.Count - 1)
                        TilesetSelectionBox.SelectedIndex = -1;
                }

                for (int i = 0; i < _autoTileSelections.Count; i++)
                {
                    _autoTileSelections[i].SelectedIndex = 0;
                    _autoTimers[i].Value = (decimal)tileset.GetAutoTileTimer(i);
                    for (int j = 0; j < _autoTileSelections[j].Items.Count; j++)
                    {
                        if (_autoTileSelections[i].Items[j].ToString() == tileset.GetAutoTile(i))
                        {
                            _autoTileSelections[i].SelectedIndex = j;
                            break;
                        }
                    }
                }

                tilesetDataPanel.SetTileset(tileset);


            }
            else
            {
                TilesetNameBox.Text = "";
                TilesetSelectionBox.SelectedIndex = -1;
                for (int i = 0; i < _autoTileSelections.Count; i++)
                {
                    _autoTileSelections[i].SelectedIndex = 0;
                }
                tilesetDataPanel.SetTileset(null);
            }
        }

        private void ImportTilesetButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sourcePath = dialog.FileName;
                Image image = Image.FromFile(sourcePath);
                if (image.Width / 32 != 8 || image.Height % 32 != 0)
                {
                    MessageBox.Show("Tilesets must be 8 tiles wide with a tile dimension of 32x32 pixels." + '\n' +
                        "Tileset height = Num Y tiles x 32.");
                }
                else
                {
                    if (!Directory.Exists("Assets/Textures/Tilesets"))
                        Directory.CreateDirectory("Assets/Textures/Tilesets");
                    string targetPath = "Assets/Textures/Tilesets/" + Path.GetFileName(sourcePath);
                    File.Copy(sourcePath, targetPath, true);
                    PopulateTilesetSelections();
                }
                image.Dispose();
            }
        }

        private void PopulateAutoTileSelections()
        {

            if (Directory.Exists("Assets/Textures/AutoTiles"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/AutoTiles", "*.png");
                for (int i = 0; i < _autoTileSelections.Count; i++)
                {
                    int selection = _autoTileSelections[i].SelectedIndex;
                    _autoTileSelections[i].Items.Clear();

                    _autoTileSelections[i].Items.Add("None");

                    for (int j = 0; j < files.Length; j++)
                    {
                        string filename = Path.GetFileName(files[j]);
                        _autoTileSelections[i].Items.Add(filename);
                        if (!mapPanel.AutoTileImages.ContainsKey(filename))
                        {
                            mapPanel.AutoTileImages.Add(filename, Image.FromFile("Assets/Textures/AutoTiles/" + filename));
                        }
                    }

                    if (selection < _autoTileSelections[i].Items.Count && selection != -1)
                        _autoTileSelections[i].SelectedIndex = selection;
                    else
                        _autoTileSelections[i].SelectedIndex = 0;
                }
            }

        }

        private void ImportAutoTileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sourcePath = dialog.FileName;
                Image image = Image.FromFile(sourcePath);
                if (image.Width % 96 != 0 || image.Height != 128)
                {
                    MessageBox.Show("Autotiles must be 96 pixels wide * num animation frames." + '\n' +
                        "Autotile height = 128.");
                }
                else
                {
                    if (!Directory.Exists("Assets/Textures/AutoTiles"))
                        Directory.CreateDirectory("Assets/Textures/AutoTiles");
                    string targetPath = "Assets/Textures/AutoTiles/" + Path.GetFileName(sourcePath);
                    File.Copy(sourcePath, targetPath, true);
                    PopulateAutoTileSelections();
                }
            }
        }

        private void ApplyTilesetDataChange()
        {
            Genus2D.GameData.TilesetData.Tileset tileset = GetSelectedTileset();
            if (tileset != null)
            {
                tileset.Name = TilesetNameBox.Text;
                tileset.SetImagePath(TilesetSelectionBox.Text);

                for (int i = 0; i < _autoTileSelections.Count; i++)
                {
                    int selection = _autoTileSelections[i].SelectedIndex;
                    if (selection > 0)
                        tileset.SetAutoTile(i, _autoTileSelections[i].Items[selection].ToString());
                    else
                        tileset.SetAutoTile(i, "");
                    tileset.SetAutoTileTimer(i, (float)_autoTimers[i].Value);
                }

                PopulateTilesetsList();
            }
            Genus2D.GameData.TilesetData.SaveData();
        }

        public Genus2D.GameData.TilesetData.Tileset GetSelectedTileset()
        {
            if (TilesetsList.SelectedIndex != -1)
                return Genus2D.GameData.TilesetData.GetTileset(TilesetsList.SelectedIndex);
            return null;
        }

        public TilesetProperties CurrentTilesetProperty()
        {
            if (PassabilitiesButton.Checked) return TilesetProperties.Passabilities;
            else if (PassabilitesButton2.Checked) return TilesetProperties.Passabilities8Dir;
            else if (PrioritiesButton.Checked) return TilesetProperties.Priorities;
            else if (TerrainTagButton.Checked) return TilesetProperties.TerrainTags;
            else if (BushFlagsButton.Checked) return TilesetProperties.BushFlags;
            else if (CounterFlagsButton.Checked) return TilesetProperties.CounterFlags;
            return TilesetProperties.None;
        }

        private void ApplyTilesetButton_Click(object sender, EventArgs e)
        {
            ApplyTilesetDataChange();
        }

        private void UndoTilesetButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.TilesetData.ReloadData();
            PopulateTilesetsList();
        }

        private void PassabilitiesButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void PassabilitesButton2_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void PrioritiesButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void TerrainTagButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void BushFlagsButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void CounterFlagsButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetDataPanel.Refresh();
        }

        private void AddTilesetButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.TilesetData.AddTileset("Tileset " + (TilesetsList.Items.Count + 1).ToString("000"));
            this.PopulateTilesetsList();
            TilesetsList.SetSelected(TilesetsList.Items.Count - 1, true);
        }

        private void RemoveTilesetButton_Click(object sender, EventArgs e)
        {
            if (TilesetsList.SelectedIndex != -1)
            {
                Genus2D.GameData.TilesetData.RemoveTileset(TilesetsList.SelectedIndex);
                this.PopulateTilesetsList();
                TilesetsList.SelectedIndex = -1;
                ChangeTileset();
            }
        }

        #endregion

        #region Event Functions

        private void PopulateEventsList()
        {
            int selection = EventsList.SelectedIndex;
            EventsList.Items.Clear();
            List<string> events = Genus2D.GameData.EventData.GetEventsDataNames();
            EventsList.Items.AddRange(events.ToArray());
            if (selection < events.Count)
                EventsList.SelectedIndex = selection;
        }

        private void PopulateEventCommandsList()
        {
            int selection = EventsList.SelectedIndex;
            int commandListIndex = EventCommandsList.SelectedIndex;
            EventCommandsList.Items.Clear();
            if (selection != -1)
            {
                Genus2D.GameData.EventData eventData = Genus2D.GameData.EventData.GetEventData(selection);
                List<string> commandStrings = eventData.GetEventCommandStrings();
                int conditionDepth = 0;
                for (int i = 0; i < commandStrings.Count; i++)
                {
                    if ((eventData.EventCommands[i].Type == Genus2D.GameData.EventCommand.CommandType.ConditionalBranchElse || 
                        eventData.EventCommands[i].Type == Genus2D.GameData.EventCommand.CommandType.ConditionalBranchEnd) && conditionDepth > 0)
                        conditionDepth--;

                    string name = "";
                        for (int j = 0; j < conditionDepth; j++)
                        name += "        ";
                    name += commandStrings[i];
                    EventCommandsList.Items.Add(name);

                    if (eventData.EventCommands[i].Type == Genus2D.GameData.EventCommand.CommandType.ConditionalBranchStart || 
                        eventData.EventCommands[i].Type == Genus2D.GameData.EventCommand.CommandType.ConditionalBranchElse)
                        conditionDepth++;
                }
                if (commandListIndex < EventCommandsList.Items.Count)
                    EventCommandsList.SelectedIndex = commandListIndex;
            }
        }

        private void PopulateEventCommandData()
        {
            int selection = EventCommandsList.SelectedIndex;
            EventCommandDataPanel.Controls.Clear();
            if (selection != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[selection];

                UserControl control = null;

                switch (command.Type)
                {
                    case Genus2D.GameData.EventCommand.CommandType.WaitTimer:
                        control = new CommandDataPresets.WaitTimerPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportPlayer:
                        control = new CommandDataPresets.TeleportPlayerPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MovePlayer:
                        control = new CommandDataPresets.MovePlayerPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangePlayerDirection:
                        control = new CommandDataPresets.ChangePlayerDirectionPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportMapEvent:
                        control = new CommandDataPresets.TeleportMapEventPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MoveMapEvent:
                        control = new CommandDataPresets.MoveMapEventPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangeMapEventDirection:
                        control = new CommandDataPresets.ChangeMapEventDirection(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowMessage:
                        control = new CommandDataPresets.ShowMessagePreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowOptions:
                        control = new CommandDataPresets.ShowOptionsPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangeSystemVariable:
                        control = new CommandDataPresets.ChangeSystemVariablePreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ConditionalBranchStart:
                        control = new CommandDataPresets.ConditionalBranchPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.AddInventoryItem:
                        control = new CommandDataPresets.ChangeInventoryItemPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.RemoveInventoryItem:
                        control = new CommandDataPresets.ChangeInventoryItemPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangePlayerSprite:
                        control = new CommandDataPresets.ChangePlayerSpritePreset(command);
                        break;
                }

                if (control != null)
                {
                    EventCommandDataPanel.Controls.Add(control);
                }

            }
        }

        private void ApplyEventCommandData()
        {
            int selection = EventCommandsList.SelectedIndex;
            if (selection != -1)
            {
                if (EventCommandDataPanel.Controls.Count == 0)
                    return;
                CommandDataPresets.CommandDataInterface commandDataInterface = (CommandDataPresets.CommandDataInterface)EventCommandDataPanel.Controls[0];
                commandDataInterface.ApplyData();
            }
        }

        private void EventsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeEventListSelection();
        }

        private void ChangeEventListSelection()
        {
            PopulateEventCommandsList();
            PopulateEventCommandData();
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                EventNameBox.Text = data.Name;
            }
            else
            {
                EventNameBox.Text = "";
            }
        }

        private void AddEventButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.EventData data = new Genus2D.GameData.EventData("Event " + (Genus2D.GameData.EventData.EventsDataCount() + 1).ToString("000"));
            Genus2D.GameData.EventData.AddEventData(data);
            PopulateEventsList();
            EventsList.SelectedIndex = EventsList.Items.Count - 1;
        }

        private void RemoveEventButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData.RemoveEventData(EventsList.SelectedIndex);
                PopulateEventsList();
                ChangeEventListSelection();
            }
        }

        private void EventCommandsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateEventCommandData();
        }

        private void AddEventCommandButton_Click(object sender, EventArgs e)
        {
            AddEventCommandForm form = new AddEventCommandForm();
            form.ShowDialog(this);
        }

        public void AddEventCommand(Genus2D.GameData.EventCommand.CommandType type)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                data.AddEventCommand(type);
                PopulateEventCommandsList();
                EventCommandsList.SelectedIndex = EventCommandsList.Items.Count - 1;
            }
        }

        private void CopyEventCommandButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex == -1)
                return;
            if (EventCommandsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[EventCommandsList.SelectedIndex];
                Genus2D.GameData.EventCommand newCommand = new Genus2D.GameData.EventCommand(command.Type);
                for (int i = 0; i < command.NumParameters(); i++)
                {
                    object parameter = command.GetParameter(i);
                    newCommand.SetParameter(i, parameter);
                }
                data.AddEventCommand(newCommand);
                PopulateEventCommandsList();
                EventCommandsList.SelectedIndex = EventCommandsList.Items.Count - 1;
            }
        }

        private void RemoveEventCommandButton_Click(object sender, EventArgs e)
        {
            if (EventCommandsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                data.RemoveEventCommand(EventCommandsList.SelectedIndex);
                PopulateEventCommandsList();
                PopulateEventCommandData();
            }
        }

        private void MoveCommandUpButton_Click(object sender, EventArgs e)
        {
            if (EventCommandsList.SelectedIndex != -1)
            {
                int selection = EventCommandsList.SelectedIndex;
                if (selection > 0)
                {
                    Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                    Genus2D.GameData.EventCommand command = data.EventCommands[selection];
                    data.EventCommands.Insert(selection - 1, command);
                    data.EventCommands.RemoveAt(selection + 1);
                    EventCommandsList.SelectedIndex--;
                    PopulateEventCommandsList();
                }
            }
        }

        private void MoveCommandDownButton_Click(object sender, EventArgs e)
        {
            if (EventCommandsList.SelectedIndex != -1)
            {
                int selection = EventCommandsList.SelectedIndex;
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                if (selection < data.EventCommands.Count - 1)
                {
                    Genus2D.GameData.EventCommand command = data.EventCommands[selection];
                    data.EventCommands.Insert(selection + 2, command);
                    data.EventCommands.RemoveAt(selection);
                    EventCommandsList.SelectedIndex++;
                    PopulateEventCommandsList();
                }
            }
        }

        private void ApplyEventChangesButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                data.Name = EventNameBox.Text;

                ApplyEventCommandData();
                PopulateEventsList();
            }
            Genus2D.GameData.EventData.SaveEventsData();
        }

        private void UndoEventChangesButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.EventData.ReloadData();
            PopulateEventsList();
        }

        #endregion

        #region Sprite Functions

        private void AddSpriteButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SpriteData.AddSpriteData("Sprite " + (SpritesList.Items.Count + 1).ToString("000"));
            PopulateSpritesList();
            SpritesList.SelectedIndex = SpritesList.Items.Count - 1;
        }

        private void RemoveSpriteButton_Click(object sender, EventArgs e)
        {
            int selection = SpritesList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.SpriteData.RemoveSprite(selection);
                PopulateSpritesList();
            }
        }

        private void ApplySpriteButton_Click(object sender, EventArgs e)
        {
            int selection = SpritesList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.SpriteData sprite = Genus2D.GameData.SpriteData.GetSpriteData(selection);
                sprite.Name = SpriteNameBox.Text;
                sprite.ImagePath = SpriteSelectionBox.Text;
                if (SpriteSelectionBox.Text != "")
                {
                    spriteViewerPanel.SetSprite(Image.FromFile("Assets/Textures/Sprites/" + sprite.ImagePath));
                }
                else
                {
                    spriteViewerPanel.SetSprite(null);
                }
                sprite.VerticalAnchorPoint.X = (int)VerticalSpriteAnchorX.Value;
                sprite.VerticalAnchorPoint.Y = (int)VerticalSpriteAnchorY.Value;
                sprite.VerticalBounds.X = (int)VerticalSpriteBoundsWidth.Value;
                sprite.VerticalBounds.Y = (int)VerticalSpriteBoundsHeight.Value;
                sprite.HorizontalAnchorPoint.X = (int)HorizontalSpriteAnchorX.Value;
                sprite.HorizontalAnchorPoint.Y = (int)HorizontalSpriteAnchorY.Value;
                sprite.HorizontalBounds.X = (int)HorizontalSpriteBoundsWidth.Value;
                sprite.HorizontalBounds.Y = (int)HorizontalSpriteBoundsHeight.Value;
            }
            else
            {
                spriteViewerPanel.SetSprite(null);
            }
            Genus2D.GameData.SpriteData.SaveData();
        }

        private void UndoSpriteButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SpriteData.ReloadData();
            PopulateSpritesList();
        }

        public Point GetSpriteVerticalAnchor()
        {
            return new Point((int)VerticalSpriteAnchorX.Value, (int)VerticalSpriteAnchorY.Value);
        }

        public Point GetSpriteVerticalBounds()
        {
            return new Point((int)VerticalSpriteBoundsWidth.Value, (int)VerticalSpriteBoundsHeight.Value);
        }

        public Point GetSpriteHorizontalAnchor()
        {
            return new Point((int)HorizontalSpriteAnchorX.Value, (int)HorizontalSpriteAnchorY.Value);
        }

        public Point GetSpriteHorizontalBounds()
        {
            return new Point((int)HorizontalSpriteBoundsWidth.Value, (int)HorizontalSpriteBoundsHeight.Value);
        }

        private void PopulateSpritesList()
        {
            int selection = SpritesList.SelectedIndex;

            SpritesList.Items.Clear();

            List<string> spriteNames = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpritesList.Items.AddRange(spriteNames.ToArray());

            if (selection < SpritesList.Items.Count)
                SpritesList.SelectedIndex = selection;
            else
                SelectSprite(-1);
        }

        private void PopulateSpriteSelections()
        {
            int selection = SpriteSelectionBox.SelectedIndex;
            SpriteSelectionBox.Items.Clear();
            if (Directory.Exists("Assets/Textures/Sprites"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Sprites", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                SpriteSelectionBox.Items.AddRange(files);
            }
            SpriteSelectionBox.SelectedIndex = selection;
        }

        private void SelectSprite(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.SpriteData sprite = Genus2D.GameData.SpriteData.GetSpriteData(index);
                SpriteNameBox.Text = sprite.Name;
                SpriteSelectionBox.Text = sprite.ImagePath;
                VerticalSpriteAnchorX.Value = (int)sprite.VerticalAnchorPoint.X;
                VerticalSpriteAnchorY.Value = (int)sprite.VerticalAnchorPoint.Y;
                VerticalSpriteBoundsWidth.Value = (int)sprite.VerticalBounds.X;
                VerticalSpriteBoundsHeight.Value = (int)sprite.VerticalBounds.Y;
                HorizontalSpriteAnchorX.Value = (int)sprite.HorizontalAnchorPoint.X;
                HorizontalSpriteAnchorY.Value = (int)sprite.HorizontalAnchorPoint.Y;
                HorizontalSpriteBoundsWidth.Value = (int)sprite.HorizontalBounds.X;
                HorizontalSpriteBoundsHeight.Value = (int)sprite.HorizontalBounds.Y;
                if (SpriteSelectionBox.Text != "")
                {
                    spriteViewerPanel.SetSprite(Image.FromFile("Assets/Textures/Sprites/" + sprite.ImagePath));
                }
                else
                {
                    spriteViewerPanel.SetSprite(null);
                }
            }
            else
            {
                SpriteNameBox.Text = "";
                SpriteSelectionBox.Text = "";
                VerticalSpriteAnchorX.Value = 0;
                VerticalSpriteAnchorY.Value = 0;
                VerticalSpriteBoundsWidth.Value = 2;
                VerticalSpriteBoundsHeight.Value = 2;
                HorizontalSpriteAnchorX.Value = 0;
                HorizontalSpriteAnchorY.Value = 0;
                HorizontalSpriteBoundsWidth.Value = 2;
                HorizontalSpriteBoundsHeight.Value = 2;
            }
        }

        private void SpritesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = SpritesList.SelectedIndex;
            SelectSprite(selection);
        }

        private void VerticalSpriteAnchorX_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                HorizontalSpriteAnchorX.Value = VerticalSpriteAnchorX.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void VerticalSpriteAnchorY_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                HorizontalSpriteAnchorY.Value = VerticalSpriteAnchorY.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void VerticalSpriteBoundsWidth_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                HorizontalSpriteBoundsWidth.Value = VerticalSpriteBoundsWidth.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void VerticalSpriteBoundsHeight_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                HorizontalSpriteBoundsHeight.Value = VerticalSpriteBoundsHeight.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void HorizontalSpriteAnchorX_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                VerticalSpriteAnchorX.Value = HorizontalSpriteAnchorX.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void HorizontalSpriteAnchorY_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                VerticalSpriteAnchorY.Value = HorizontalSpriteAnchorY.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void HorizontalSpriteBoundsWidth_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                VerticalSpriteBoundsWidth.Value = HorizontalSpriteBoundsWidth.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void HorizontalSpriteBoundsHeight_ValueChanged(object sender, EventArgs e)
        {
            if (LockSpriteBoundsCheck.Checked)
            {
                VerticalSpriteBoundsHeight.Value = HorizontalSpriteBoundsHeight.Value;
            }
            spriteViewerPanel.Refresh();
        }

        private void ImportSpriteButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sourcePath = dialog.FileName;
                Image image = Image.FromFile(sourcePath);
                if (image.Width < 32 * 4 || image.Height < 32 * 4)
                {
                    MessageBox.Show("Sprites must be a minimum of 32 pixels wide/tall * 4 frames.");
                }
                else
                {
                    if (!Directory.Exists("Assets/Textures/Sprites"))
                        Directory.CreateDirectory("Assets/Textures/Sprites");
                    string targetPath = "Assets/Textures/Sprites/" + Path.GetFileName(sourcePath);
                    File.Copy(sourcePath, targetPath, true);
                    PopulateSpriteSelections();
                }
            }
        }

        #endregion

        #region Item Functions

        private void PopulateItemList()
        {
            int selection = ItemListBox.SelectedIndex;
            ItemListBox.Items.Clear();
            for (int i = 0; i < Genus2D.GameData.ItemData.GetItemDataCount(); i++)
            {
                ItemListBox.Items.Add(Genus2D.GameData.ItemData.GetItemData(i).Name);
            }
            if (selection < ItemListBox.Items.Count)
                ItemListBox.SelectedIndex = selection;
            else
                SelectItem(-1);

        }

        private void PopulateItemIconSelections()
        {
            ItemIconSelection.Items.Clear();
            if (Directory.Exists("Assets/Textures/Icons"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Icons", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                ItemIconSelection.Items.AddRange(files);
            }
        }

        private void AddItemButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ItemData data = new Genus2D.GameData.ItemData("Item " + (ItemListBox.Items.Count + 1).ToString("000"));
            Genus2D.GameData.ItemData.AddItemData(data);
            PopulateItemList();
            ItemListBox.SelectedIndex = ItemListBox.Items.Count - 1;

        }

        private void RemoveItemButton_Click(object sender, EventArgs e)
        {
            int selection = ItemListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ItemData.RemoveItemData(selection);
                PopulateItemList();
            }
        }

        private void ItemListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = ItemListBox.SelectedIndex;
            SelectItem(selection);
        }

        private void SelectItem(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.ItemData data = Genus2D.GameData.ItemData.GetItemData(index);
                ItemNameBox.Text = data.Name;
                ItemIconSelection.Text = data.IconSheetImage;
                ItemTypeSelection.SelectedIndex = (int)data.GetItemType();
                ItemMaxStack.Value = data.GetMaxStack();
                iconSheetPanel.SetItemData(data);
            }
            else
            {
                ItemNameBox.Text = "";
                ItemIconSelection.SelectedIndex = -1;
                ItemTypeSelection.SelectedIndex = 0;
                ItemMaxStack.Value = 1;
                iconSheetPanel.SetItemData(null);
            }
            PopulateItemStatPanel();
        }

        private void PopulateItemStatPanel()
        {
            int selection = ItemListBox.SelectedIndex;
            ItemStatsPanel.Controls.Clear();
            if (selection != -1)
            {
                Genus2D.GameData.ItemData data = Genus2D.GameData.ItemData.GetItemData(selection);
                switch (data.GetItemType())
                {
                    case Genus2D.GameData.ItemData.ItemType.Tool:

                        ComboBox toolSelection = new ComboBox();
                        toolSelection.Location = new Point(10, 10);

                        for (int i = 0; i < 3; i++)
                        {
                            Genus2D.GameData.ToolType toolType = (Genus2D.GameData.ToolType)i;
                            toolSelection.Items.Add(toolType.ToString());
                        }
                        toolSelection.SelectedIndex = (int)data.GetItemStat("ToolType").Item2;

                        ItemStatsPanel.Controls.Add(toolSelection);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Consumable:

                        NumericUpDown hpControl = new NumericUpDown();
                        hpControl.Location = new Point(10, 10);
                        hpControl.Value = (int)data.GetItemStat("HP").Item2;

                        NumericUpDown staminaControl = new NumericUpDown();
                        staminaControl.Location = new Point(10, 20 + hpControl.Size.Height);
                        staminaControl.Value = (int)data.GetItemStat("Stamina").Item2;

                        ItemStatsPanel.Controls.Add(hpControl);
                        ItemStatsPanel.Controls.Add(staminaControl);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Material:

                        NumericUpDown matIdControl = new NumericUpDown();
                        matIdControl.Location = new Point(10, 10);
                        matIdControl.Value = (int)data.GetItemStat("MaterialID").Item2;

                        ItemStatsPanel.Controls.Add(matIdControl);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Equipment:

                        ComboBox equipmentSlotSelection = new ComboBox();
                        equipmentSlotSelection.Location = new Point(10, 10);

                        for (int i = 0; i < 9; i++)
                        {
                            Genus2D.GameData.EquipmentSlot slot = (Genus2D.GameData.EquipmentSlot)i;
                            equipmentSlotSelection.Items.Add(slot.ToString());
                        }
                        equipmentSlotSelection.SelectedIndex = (int)data.GetItemStat("EquipmentSlot").Item2;

                        int y = 20 + equipmentSlotSelection.Size.Height;
                        NumericUpDown atkStrengthControl = new NumericUpDown();
                        atkStrengthControl.Location = new Point(10, y);
                        atkStrengthControl.Value = (int)data.GetItemStat("AttackStrength").Item2;

                        y += atkStrengthControl.Size.Height + 10;
                        NumericUpDown defenceControl = new NumericUpDown();
                        defenceControl.Location = new Point(10, y);
                        defenceControl.Value = (int)data.GetItemStat("Defence").Item2;

                        ItemStatsPanel.Controls.Add(equipmentSlotSelection);
                        ItemStatsPanel.Controls.Add(atkStrengthControl);
                        ItemStatsPanel.Controls.Add(defenceControl);

                        break;
                }
            }
        }

        private void ImportIconButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sourcePath = dialog.FileName;
                Image image = Image.FromFile(sourcePath);
                if (image.Width / 32 != 8 || image.Height / 32 != 8)
                {
                    MessageBox.Show("Icon sheets must be 8 tiles wide and tall, with a tile dimension of 32x32 pixels.");
                }
                else
                {
                    if (!Directory.Exists("Assets/Textures/Icons"))
                        Directory.CreateDirectory("Assets/Textures/Icons");
                    string targetPath = "Assets/Textures/Icons/" + Path.GetFileName(sourcePath);
                    File.Copy(sourcePath, targetPath, true);
                    PopulateItemIconSelections();
                }
            }
        }

        private void ApplyItemButton_Click(object sender, EventArgs e)
        {
            int selection = ItemListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ItemData data = Genus2D.GameData.ItemData.GetItemData(selection);
                data.Name = ItemNameBox.Text;
                if (ItemIconSelection.SelectedIndex != -1)
                    data.IconSheetImage = (string)ItemIconSelection.Items[ItemIconSelection.SelectedIndex];
                int prevItemType = (int)data.GetItemType();
                data.SetItemType((Genus2D.GameData.ItemData.ItemType)ItemTypeSelection.SelectedIndex);
                data.SetMaxStack((int)ItemMaxStack.Value);

                if (prevItemType == ItemTypeSelection.SelectedIndex)
                {
                    switch (data.GetItemType())
                    {
                        case Genus2D.GameData.ItemData.ItemType.Tool:

                            ComboBox toolSelection = (ComboBox)ItemStatsPanel.Controls[0];
                            data.SetItemStat("ToolType", toolSelection.SelectedIndex);

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Consumable:

                            NumericUpDown hpControl = (NumericUpDown)ItemStatsPanel.Controls[0];
                            NumericUpDown staminaControl = (NumericUpDown)ItemStatsPanel.Controls[1];
                            data.SetItemStat("HP", (int)hpControl.Value);
                            data.SetItemStat("Stamina", (int)staminaControl.Value);

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Material:

                            NumericUpDown matIdControl = (NumericUpDown)ItemStatsPanel.Controls[0];
                            data.SetItemStat("MaterialID", (int)matIdControl.Value);

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Equipment:

                            ComboBox equipmentSlotSelection = (ComboBox)ItemStatsPanel.Controls[0];
                            NumericUpDown atkStrengthControl = (NumericUpDown)ItemStatsPanel.Controls[1];
                            NumericUpDown defenceControl = (NumericUpDown)ItemStatsPanel.Controls[2];
                            data.SetItemStat("EquipmentSlot", equipmentSlotSelection.SelectedIndex);
                            data.SetItemStat("AttackStrength", (int)atkStrengthControl.Value);
                            data.SetItemStat("Defence", (int)defenceControl.Value);

                            break;
                    }
                }
                else
                {
                    PopulateItemStatPanel();
                }

                PopulateItemList();
            }
            Genus2D.GameData.ItemData.SaveItemData();
        }

        private void UndoItemButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ItemData.ReloadData();
            PopulateItemList();
        }

        #endregion

        #region System Variables

        private void PopulateSystemVariables()
        {
            int selection = SystemVariablesList.SelectedIndex;
            SystemVariablesList.Items.Clear();
            for (int i = 0; i < Genus2D.GameData.SystemVariable.SystemVariablesCount(); i++)
            {
                SystemVariablesList.Items.Add(Genus2D.GameData.SystemVariable.GetSystemVariable(i).Name);
            }
            if (selection < SystemVariablesList.Items.Count)
                SystemVariablesList.SelectedIndex = selection;
            else
                SelectSystemVariable(-1);
        }

        private void SystemVariablesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = SystemVariablesList.SelectedIndex;
            SelectSystemVariable(selection);
        }

        private void SelectSystemVariable(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.SystemVariable variable = Genus2D.GameData.SystemVariable.GetSystemVariable(index);
                VariableNameBox.Text = variable.Name;
                VariableTypeBox.SelectedIndex = (int)variable.Type;
                VariableValueBox.Text = variable.Value.ToString();
            }
            else
            {
                VariableNameBox.Text = "";
                VariableTypeBox.SelectedIndex = 0;
                VariableValueBox.Text = "";
            }
        }

        private void AddVariableButton_Click(object sender, EventArgs e)
        {
            string name = "Variable " + (SystemVariablesList.Items.Count + 1).ToString("000");
            Genus2D.GameData.SystemVariable variable = new Genus2D.GameData.SystemVariable(name);
            Genus2D.GameData.SystemVariable.AddSystemVariable(variable);
            PopulateSystemVariables();
            SystemVariablesList.SelectedIndex = SystemVariablesList.Items.Count - 1;
        }

        private void RemoveVariableButton_Click(object sender, EventArgs e)
        {
            int selection = SystemVariablesList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.SystemVariable.RemoveSystemVariable(selection);
                PopulateSystemVariables();
            }
        }

        private void UndoVariablesButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SystemVariable.ReloadData();
            PopulateSystemVariables();
        }

        private void ApplyVariablesButton_Click(object sender, EventArgs e)
        {
            int selection = SystemVariablesList.SelectedIndex;
            if (selection != -1)
            {
                string name = VariableNameBox.Text;
                Genus2D.GameData.VariableType type = (Genus2D.GameData.VariableType)VariableTypeBox.SelectedIndex;

                Genus2D.GameData.SystemVariable variable = Genus2D.GameData.SystemVariable.GetSystemVariable(selection);
                variable.Name = name;
                variable.SetVariableType(type);

                string valueString = VariableValueBox.Text;
                if (!variable.SetValue(valueString))
                    MessageBox.Show("Error parsing value string to selected variable type.");

                PopulateSystemVariables();

            }
            Genus2D.GameData.SystemVariable.SaveData();
        }

        #endregion

    }
}
