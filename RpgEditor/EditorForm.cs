using Genus2D.GameData;
using OpenTK.Graphics;
using RpgEditor.ItemDataPresets;
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
        public IconSheetPanel itemIconSheetPanel;
        public IconSheetPanel projectileIconSheetPanel;
        public ProjectileViewerPanel projectileViewerPanel;

        private List<ComboBox> _autoTileSelections;
        private List<NumericUpDown> _autoTimers;

        public enum MapTool
        {
            Pencil, Rectangle, FloodFill, Event, SpawnPoint, None
        }

        public enum TilesetProperties
        {
            Passabilities, Passabilities8Dir, Priorities, TerrainTags, BushFlags, CounterFlags, ReflectionFlags, BridgeFlags, None
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

            itemIconSheetPanel = new IconSheetPanel();
            itemIconSheetPanel.Size = panel1.Size;

            projectileIconSheetPanel = new IconSheetPanel();
            projectileIconSheetPanel.Size = panel3.Size;

            projectileViewerPanel = new ProjectileViewerPanel(this);
            projectileViewerPanel.Size = panel4.Size;

            splitContainer2.Panel1.Controls.Add(tilesetSelectionPanel);
            splitContainer2.Panel2.Controls.Add(mapPanel);
            panel2.Controls.Add(tilesetDataPanel);
            panel1.Controls.Add(itemIconSheetPanel);
            panel3.Controls.Add(projectileIconSheetPanel);
            panel4.Controls.Add(projectileViewerPanel);
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
            PopulateIconSelections();
            PopulateProjectilesList();
            PopulateCraftablesList();
            PopulateEmittersList();
            PopulateParticleTextureSelections();
            PopulateParticleShapeSelections();
            PopulateWorkbenchList();
            PopulateClassesList();
            PopulateDropTablesList();
            PopulateEnemyList();
            PopulateQuestList();
            PopulateShopList();
            PopulateSystemVariables();
            PopulateSytemData();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (ParticlePreviewWindow.PreviewInstance != null)
                ParticlePreviewWindow.PreviewInstance.Close();
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

        private void FloodFillButton_CheckedChanged(object sender, EventArgs e)
        {
            tilesetSelectionPanel.UpdateMapTool();
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
                    _autoTimers[i].Value = (decimal)Math.Max(tileset.GetAutoTileTimer(i), 0.1);
                    for (int j = 0; j < _autoTileSelections[i].Items.Count; j++)
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
            else if (ReflectionFlagsButton.Checked) return TilesetProperties.ReflectionFlags;
            else if (BridgeFlagsButton.Checked) return TilesetProperties.BridgeFlags;
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

        private void Properties_CheckedChanged(object sender, EventArgs e)
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
                    case Genus2D.GameData.EventCommand.CommandType.ChangeMapEvent:
                        control = new CommandDataPresets.ChangeMapEventPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.AddGold:
                        control = new CommandDataPresets.ChangeGoldPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.RemoveGold:
                        control = new CommandDataPresets.ChangeGoldPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.SpawnEnemy:
                        control = new CommandDataPresets.EnemySpawnPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ProgressQuest:
                        control = new CommandDataPresets.ProgressQuestPreset(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowShop:
                        control = new CommandDataPresets.ShowShopPreset(command);
                        break;
                    case EventCommand.CommandType.ShowWorkbench:
                        control = new CommandDataPresets.ShowWorkbenchPreset(command);
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
                PopulateSpritesList();
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
            EnemySpriteSelection.Items.Clear();
            EnemySpriteSelection.Items.Add("None");

            List<string> spriteNames = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpritesList.Items.AddRange(spriteNames.ToArray());
            EnemySpriteSelection.Items.AddRange(spriteNames.ToArray());

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

            PopulateCraftingItemsList();
        }

        private void PopulateIconSelections()
        {
            ItemIconSelection.Items.Clear();
            ProjectileIconSelection.Items.Clear();
            if (Directory.Exists("Assets/Textures/Icons"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Icons", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                ItemIconSelection.Items.AddRange(files);
                ProjectileIconSelection.Items.AddRange(files);
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
                ItemSellableSelection.SelectedIndex = data.Sellable ? 1 : 0;
                ItemSellPriceSelection.Value = data.SellPrice;
                ItemTypeSelection.SelectedIndex = (int)data.GetItemType();
                ItemMaxStack.Value = data.GetMaxStack();
                itemIconSheetPanel.SetItemData(data);
            }
            else
            {
                ItemNameBox.Text = "";
                ItemIconSelection.SelectedIndex = -1;
                ItemSellableSelection.SelectedIndex = 0;
                ItemSellPriceSelection.Value = 0;
                ItemTypeSelection.SelectedIndex = 0;
                ItemMaxStack.Value = 1;
                itemIconSheetPanel.SetItemData(null);
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

                        ToolPreset toolPreset = new ToolPreset();
                        toolPreset.SetToolType((Genus2D.GameData.ToolType)data.GetItemStat("ToolType"));
                        ItemStatsPanel.Controls.Add(toolPreset);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Consumable:

                        ConsumablePreset consumablePreset = new ConsumablePreset();
                        consumablePreset.SetHpHeal((int)data.GetItemStat("HpHeal"));
                        consumablePreset.SetMpHeal((int)data.GetItemStat("MpHeal"));
                        consumablePreset.SetStaminaHeal((int)data.GetItemStat("StaminaHeal"));
                        ItemStatsPanel.Controls.Add(consumablePreset);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Material:

                        MatarialPreset materialPreset = new MatarialPreset();
                        materialPreset.SetMaterialID((int)data.GetItemStat("MaterialID"));
                        ItemStatsPanel.Controls.Add(materialPreset);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Equipment:

                        EquipmentPreset equipmentPreset = new EquipmentPreset();
                        equipmentPreset.SetEquipmentSlot((Genus2D.GameData.EquipmentSlot)data.GetItemStat("EquipmentSlot"));
                        equipmentPreset.SetAttackStyle((Genus2D.GameData.AttackStyle)data.GetItemStat("AttackStyle"));
                        equipmentPreset.SetVitalityBonus((int)data.GetItemStat("VitalityBonus"));
                        equipmentPreset.SetInteligenceBonus((int)data.GetItemStat("InteligenceBonus"));
                        equipmentPreset.SetStrengthBonus((int)data.GetItemStat("StrengthBonus"));
                        equipmentPreset.SetAgilityBonus((int)data.GetItemStat("AgilityBonus"));
                        equipmentPreset.SetMeleeDefenceBonus((int)data.GetItemStat("MeleeDefenceBonus"));
                        equipmentPreset.SetRangeDefenceBonus((int)data.GetItemStat("RangeDefenceBonus"));
                        equipmentPreset.SetMagicDefenceBonus((int)data.GetItemStat("MagicDefenceBonus"));
                        equipmentPreset.SetProjectileID((int)data.GetItemStat("ProjectileID"));
                        equipmentPreset.SetMpDrain((int)data.GetItemStat("MP"));
                        ItemStatsPanel.Controls.Add(equipmentPreset);

                        break;
                    case Genus2D.GameData.ItemData.ItemType.Ammo:

                        AmmoPreset ammoPreset = new AmmoPreset();
                        ammoPreset.SetStrengthBonus((int)data.GetItemStat("StrengthBonus"));
                        ammoPreset.SetProjectileID((int)data.GetItemStat("ProjectileID"));
                        ItemStatsPanel.Controls.Add(ammoPreset);

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
                    PopulateIconSelections();
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

                Genus2D.GameData.ItemData.ItemType itemType = (Genus2D.GameData.ItemData.ItemType)ItemTypeSelection.SelectedIndex;
                if (itemType != Genus2D.GameData.ItemData.ItemType.Quest)
                {
                    data.Sellable = ItemSellableSelection.SelectedIndex == 0 ? false : true;
                    data.SellPrice = (int)ItemSellPriceSelection.Value;
                }
                else
                {
                    data.Sellable = false;
                    data.SellPrice = 0;
                }

                data.SetItemType(itemType);
                data.SetMaxStack((int)ItemMaxStack.Value);

                if (prevItemType == ItemTypeSelection.SelectedIndex)
                {
                    switch (data.GetItemType())
                    {
                        case Genus2D.GameData.ItemData.ItemType.Tool:

                            ToolPreset toolPreset = (ToolPreset)ItemStatsPanel.Controls[0];
                            data.SetItemStat("ToolType", toolPreset.GetToolType());

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Consumable:

                            ConsumablePreset consumablePreset = (ConsumablePreset)ItemStatsPanel.Controls[0];
                            data.SetItemStat("HpHeal", consumablePreset.GetHpHeal());
                            data.SetItemStat("MpHeal", consumablePreset.GetMpHeal());
                            data.SetItemStat("StaminaHeal", consumablePreset.GetStaminaHeal());

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Material:

                            MatarialPreset materialPreset = (MatarialPreset)ItemStatsPanel.Controls[0];
                            data.SetItemStat("MaterialID", materialPreset.GetMaterialID());

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Equipment:

                            EquipmentPreset equipmentPreset = (EquipmentPreset)ItemStatsPanel.Controls[0];
                            data.SetItemStat("EquipmentSlot", equipmentPreset.GetEquipmentSlot());
                            data.SetItemStat("AttackStyle", equipmentPreset.GetAttackStyle());
                            data.SetItemStat("VitalityBonus", equipmentPreset.GetVitalityBonus());
                            data.SetItemStat("InteligenceBonus", equipmentPreset.GetInteligenceBonus());
                            data.SetItemStat("StrengthBonus", equipmentPreset.GetStrengthBonus());
                            data.SetItemStat("AgilityBonus", equipmentPreset.GetAgilityBonus());
                            data.SetItemStat("MeleeDefenceBonus", equipmentPreset.GetMeleeDefenceBonus());
                            data.SetItemStat("RangeDefenceBonus", equipmentPreset.GetRangeDefenceBonus());
                            data.SetItemStat("MagicDefenceBonus", equipmentPreset.GetMagicDefenceBonus());
                            data.SetItemStat("ProjectileID", equipmentPreset.GetProjectileID());
                            data.SetItemStat("MP", equipmentPreset.GetMpDrain());

                            break;
                        case Genus2D.GameData.ItemData.ItemType.Ammo:

                            AmmoPreset ammoPreset = (AmmoPreset)ItemStatsPanel.Controls[0];
                            data.SetItemStat("StrengthBonus", ammoPreset.GetStrengthBonus());
                            data.SetItemStat("ProjectileID", ammoPreset.GetProjectileID());

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

        #region Projectile Functions

        private void PopulateProjectilesList()
        {
            int selection = ProjectileListBox.SelectedIndex;
            ProjectileListBox.Items.Clear();
            EnemyProjectileSelection.Items.Clear();
            EnemyProjectileSelection.Items.Add("None");
            for (int i = 0; i < Genus2D.GameData.ProjectileData.GetProjectileDataCount(); i++)
            {
                ProjectileListBox.Items.Add(Genus2D.GameData.ProjectileData.GetProjectileData(i).Name);
                EnemyProjectileSelection.Items.Add(Genus2D.GameData.ProjectileData.GetProjectileData(i).Name);
            }
            if (selection < ProjectileListBox.Items.Count)
                ProjectileListBox.SelectedIndex = selection;
            else
                SelectProjectile();

            ItemStatsPanel.Refresh();
        }

        private void ProjectileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectProjectile();
        }

        private void SelectProjectile()
        {
            int selection = ProjectileListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ProjectileData data = Genus2D.GameData.ProjectileData.GetProjectileData(selection);
                ProjectileNameBox.Text = data.Name;
                ProjectileIconSelection.Text = data.IconSheetImage;
                ProjectileSpeed.Value = (decimal)data.Speed;
                ProjectileLifespan.Value = (decimal)data.Lifespan;
                ProjectileAnchorX.Value = data.AnchorX;
                ProjectileAnchorY.Value = data.AnchorY;
                ProjectileBoundsWidth.Value = data.BoundsWidth;
                ProjectileBoundsHeight.Value = data.BoundsHeight;
                projectileIconSheetPanel.SetProjectileData(data);
                projectileViewerPanel.SetProjectileData(data);
            }
            else
            {
                ProjectileNameBox.Text = "";
                ProjectileIconSelection.SelectedIndex = -1;
                ProjectileSpeed.Value = (decimal)0.1f;
                ProjectileLifespan.Value = (decimal)0.1f;
                ProjectileAnchorX.Value = 0;
                ProjectileAnchorY.Value = 0;
                ProjectileBoundsWidth.Value = 2;
                ProjectileBoundsHeight.Value = 2;
                projectileIconSheetPanel.SetProjectileData(null);
                projectileViewerPanel.SetProjectileData(null);
            }
        }

        private void AddProjectileButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ProjectileData data = new Genus2D.GameData.ProjectileData("Projectile " + (ProjectileListBox.Items.Count + 1).ToString("000"));
            Genus2D.GameData.ProjectileData.AddProjectileData(data);
            PopulateProjectilesList();
            ProjectileListBox.SelectedIndex = ProjectileListBox.Items.Count - 1;
        }

        private void RemoveProjectileButton_Click(object sender, EventArgs e)
        {
            int selection = ProjectileListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ProjectileData.RemoveProjectileData(selection);
                PopulateProjectilesList();
            }
        }

        private void UndoProjectileButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ProjectileData.ReloadData();
            PopulateProjectilesList();
        }

        private void ApplyProjectileButton_Click(object sender, EventArgs e)
        {
            int selection = ProjectileListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ProjectileData data = Genus2D.GameData.ProjectileData.GetProjectileData(selection);
                data.Name = ProjectileNameBox.Text;
                data.IconSheetImage = ProjectileIconSelection.Text;
                data.Speed = (float)ProjectileSpeed.Value;
                data.Lifespan = (float)ProjectileLifespan.Value;
                data.AnchorX = (int)ProjectileAnchorX.Value;
                data.AnchorY = (int)ProjectileAnchorY.Value;
                data.BoundsWidth = (int)ProjectileBoundsWidth.Value;
                data.BoundsHeight = (int)ProjectileBoundsHeight.Value;

                projectileIconSheetPanel.SetProjectileData(data);
                projectileViewerPanel.SetProjectileData(data);
            }
            Genus2D.GameData.ProjectileData.SaveItemData();
        }

        public Point GetProjectilelAnchor()
        {
            return new Point((int)ProjectileAnchorX.Value, (int)ProjectileAnchorY.Value);
        }

        public Point GetProjectileBounds()
        {
            return new Point((int)ProjectileBoundsWidth.Value, (int)ProjectileBoundsHeight.Value);
        }

        private void ProjectileAnchorX_ValueChanged(object sender, EventArgs e)
        {
            projectileViewerPanel.Refresh();
        }

        private void ProjectileAnchorY_ValueChanged(object sender, EventArgs e)
        {
            projectileViewerPanel.Refresh();
        }

        private void ProjectileBoundsWidth_ValueChanged(object sender, EventArgs e)
        {
            projectileViewerPanel.Refresh();
        }

        private void ProjectileBoundsHeight_ValueChanged(object sender, EventArgs e)
        {
            projectileViewerPanel.Refresh();
        }

        #endregion

        #region Craftables Data

        private void PopulateCraftablesList()
        {
            int selection = CraftablesListBox.SelectedIndex;
            CraftablesListBox.Items.Clear();
            for (int i = 0; i < Genus2D.GameData.CraftableData.GetCraftableDataCount(); i++)
            {
                CraftablesListBox.Items.Add(Genus2D.GameData.CraftableData.GetCraftableData(i).Name);
            }
            if (selection < CraftablesListBox.Items.Count)
                CraftablesListBox.SelectedIndex = selection;
            else
                SelectCraftable(-1);

        }

        private void SelectCraftable(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.CraftableData data = Genus2D.GameData.CraftableData.GetCraftableData(index);
                CraftableNameBox.Text = data.Name;
                CraftableItemSelection.SelectedIndex = data.CraftedItemID;
                CraftableAmountSelection.Value = data.CraftedItemCount;
                CraftableWorkbenchSelection.SelectedIndex = data.WorkbenchID;
            }
            else
            {
                CraftableNameBox.Text = "";
                CraftableItemSelection.SelectedIndex = -1;
                CraftableAmountSelection.Value = 1;
                CraftableWorkbenchSelection.SelectedIndex = -1;

            }

            PopulateCraftableMaterialList();
        }

        private void PopulateCraftingItemsList()
        {
            int selection = CraftableItemSelection.SelectedIndex;

            CraftableItemSelection.Items.Clear();
            List<string> items = ItemData.GetItemNames();
            CraftableItemSelection.Items.AddRange(items.ToArray());

            if (selection < items.Count)
                CraftableItemSelection.SelectedIndex = selection;
            else
                CraftableItemSelection.SelectedIndex = -1;
        }

        private void PopulateWorkbenchList()
        {
            int selection = WorkBenchesListBox.SelectedIndex;
            int craftableSelection = CraftableWorkbenchSelection.SelectedIndex;

            CraftableWorkbenchSelection.Items.Clear();
            WorkBenchesListBox.Items.Clear();
            for (int i = 0; i < CraftableData.GetWorkbenchDataCount(); i++)
            {
                string name = CraftableData.GetWorkbench(i);
                CraftableWorkbenchSelection.Items.Add(name);
                WorkBenchesListBox.Items.Add(name);
            }

            if (selection < CraftableData.GetWorkbenchDataCount())
            {
                WorkBenchesListBox.SelectedIndex = selection;
            }

            if (craftableSelection < CraftableData.GetWorkbenchDataCount())
            {
                CraftableWorkbenchSelection.SelectedIndex = craftableSelection;
            }
        }

        private void PopulateCraftableMaterialList()
        {
            int craftableSelection = CraftablesListBox.SelectedIndex;
            CraftingMaterialListBox.Items.Clear();

            if (craftableSelection > -1)
            {
                CraftableData data = CraftableData.GetCraftableData(craftableSelection);
                for (int i = 0; i < data.Materials.Count; i++)
                {
                    ItemData itemData = ItemData.GetItemData(data.Materials[i].Item1);
                    string label = "Item: " + (itemData == null ? "None " : itemData.Name);
                    label +=  " {" + data.Materials[i].Item1 + ", " + data.Materials[i].Item2 + "}";
                    CraftingMaterialListBox.Items.Add(label);
                }
            }

        }

        private void CraftablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectCraftable(CraftablesListBox.SelectedIndex);
        }

        private void AddCraftableButton_Click(object sender, EventArgs e)
        {
            CraftableData data = new CraftableData("Craftable " + (CraftableData.GetCraftableDataCount() + 1));
            CraftableData.AddCraftableData(data);
            PopulateCraftablesList();
        }

        private void RemoveCraftableButton_Click(object sender, EventArgs e)
        {
            CraftableData.RemoveCraftableData(CraftablesListBox.SelectedIndex);
            PopulateCraftablesList();
        }

        private void AddWorkbenchButton_Click(object sender, EventArgs e)
        {
            CraftableData.AddWorkbench(WorkbenchNameBox.Text);
            PopulateWorkbenchList();
        }

        private void RemoveWorkbenchButton_Click(object sender, EventArgs e)
        {
            CraftableData.RemoveWorkbench(WorkBenchesListBox.SelectedIndex);
            PopulateWorkbenchList();
        }

        private void WorkBenchesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkBenchesListBox.SelectedIndex > -1)
            {
                WorkbenchNameBox.Text = (string)WorkBenchesListBox.Items[WorkBenchesListBox.SelectedIndex];
            }
            else
            {
                WorkbenchNameBox.Text = "";
            }
        }

        private void UndoCraftablesButton_Click(object sender, EventArgs e)
        {
            CraftableData.ReloadData();
            PopulateCraftablesList();
            PopulateWorkbenchList();
        }

        private void ApplyCraftablesButton_Click(object sender, EventArgs e)
        {
            int selection = CraftablesListBox.SelectedIndex;
            if (selection > -1)
            {
                Genus2D.GameData.CraftableData data = Genus2D.GameData.CraftableData.GetCraftableData(selection);
                data.Name = CraftableNameBox.Text;
                data.CraftedItemID = CraftableItemSelection.SelectedIndex;
                data.CraftedItemCount = (int)CraftableAmountSelection.Value;
                data.WorkbenchID = CraftableWorkbenchSelection.SelectedIndex;
            }

            CraftableData.SaveCraftablesData();
            CraftableData.SaveWorkbenchesData();
        }

        private void AddCraftingMaterialButton_Click(object sender, EventArgs e)
        {
            int selection = CraftablesListBox.SelectedIndex;
            if (selection > -1)
            {
                Genus2D.GameData.CraftableData data = Genus2D.GameData.CraftableData.GetCraftableData(selection);
                data.Materials.Add(new Tuple<int, int>(-1, 1));
                PopulateCraftableMaterialList();
            }
        }

        private void EditCraftingMaterialButton_Click(object sender, EventArgs e)
        {
            int craftableSelection = CraftablesListBox.SelectedIndex;
            int selection = CraftingMaterialListBox.SelectedIndex;

            if (selection > -1 && craftableSelection > -1)
            {
                Genus2D.GameData.CraftableData data = Genus2D.GameData.CraftableData.GetCraftableData(craftableSelection);
                EditCraftingMaterialForm form = new EditCraftingMaterialForm(data, selection);
                form.ShowDialog(this);
                SelectCraftable(craftableSelection);
            }
        }

        private void RemoveCraftingMaterialButton_Click(object sender, EventArgs e)
        {
            int craftableSelection = CraftablesListBox.SelectedIndex;
            int selection = CraftingMaterialListBox.SelectedIndex;

            if (selection > -1 && craftableSelection > -1)
            {
                Genus2D.GameData.CraftableData data = Genus2D.GameData.CraftableData.GetCraftableData(craftableSelection);
                data.Materials.RemoveAt(selection);
                PopulateCraftableMaterialList();
            }
        }

        #endregion

        #region Particle Emitter Data
        private void PopulateEmittersList()
        {
            int selection = ParticlesListBox.SelectedIndex;
            ParticlesListBox.Items.Clear();
            for (int i = 0; i < Genus2D.GameData.ParticleEmitterData.GetEmitterDataCount(); i++)
            {
                ParticlesListBox.Items.Add(Genus2D.GameData.ParticleEmitterData.GetEmitterData(i).Name);
            }
            if (selection < ParticlesListBox.Items.Count)
                ParticlesListBox.SelectedIndex = selection;
            else
                SelectEmitter(-1);
        }

        private void ParticlesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = ParticlesListBox.SelectedIndex;
            SelectEmitter(selection);
        }

        private void SelectEmitter(int index)
        {
            if (index != -1)
            {
                Genus2D.GameData.ParticleEmitterData data = Genus2D.GameData.ParticleEmitterData.GetEmitterData(index);
                ParticleNameBox.Text = data.Name;
                ParticleShapeSelection.SelectedIndex = (int)data.EmitterShape;

                ParticleTextureSelection.SelectedIndex = 0;
                if (data.ParticleTexture != "")
                {
                    for (int i = 0; i < ParticleTextureSelection.Items.Count; i++)
                    {
                        if ((string)ParticleTextureSelection.Items[i] == data.ParticleTexture)
                        {
                            ParticleTextureSelection.SelectedIndex = i;
                            break;
                        }
                    }
                }

                ParticleEmissionSelection.Value = (decimal)data.EmissionRate;
                ParticleAngleMinSelection.Value = (decimal)data.AngleMin;
                ParticleAngleMaxSelection.Value = (decimal)data.AngleMax;
                ParticleOffsetMinSelection.Value = (decimal)data.OffsetMin;
                ParticleOffsetMaxSelection.Value = (decimal)data.OffsetMax;
                ParticleStartVelocitySelection.Value = (decimal)data.StartVelocity;
                ParticleEndVelocitySelection.Value = (decimal)data.EndVelocity;
                ParticleStartZSelection.Value = (decimal)data.StartZ;
                ParticleEndZSelection.Value = (decimal)data.EndZ;
                particleSinZSelection.Value = (decimal)data.SinMaxZ;
                particleSinSpeedSelection.Value = (decimal)data.SinSpeedZ;
                ParticleStartScaleSelection.Value = (decimal)data.StartScale;
                ParticleEndScaleSelection.Value = (decimal)data.EndScale;
                ParticleRotationSpeedSelection.Value = (decimal)data.RotationSpeed;

                Color4 startColour = data.StartColour;
                startColour.A = 1;
                Color sColour = Color.FromArgb(startColour.ToArgb());
                ParticleColorBox1.BackColor = sColour;
                ParticleStartAlphaSelection.Value = (decimal)data.StartColour.A;

                Color4 endColour = data.EndColour;
                endColour.A = 1;
                Color eColour = Color.FromArgb(endColour.ToArgb());
                ParticleColorBox2.BackColor = eColour;
                ParticleEndAlphaSelection.Value = (decimal)data.EndColour.A;

                ParticleMaxLifeSelection.Value = (decimal)data.MaxLife;

                if (ParticlePreviewWindow.PreviewInstance != null)
                {
                    ParticlePreviewWindow.PreviewInstance.SetParticleData(data);
                }
            }
            else
            {
                ParticleNameBox.Text = "";
                ParticleShapeSelection.SelectedIndex = 0;
                ParticleTextureSelection.SelectedIndex = 0;
                ParticleEmissionSelection.Value = 1;
                ParticleAngleMinSelection.Value = 0;
                ParticleAngleMaxSelection.Value = 360;
                ParticleOffsetMinSelection.Value = 0;
                ParticleOffsetMaxSelection.Value = 0;
                ParticleStartVelocitySelection.Value = 1;
                ParticleEndVelocitySelection.Value = 1;
                ParticleStartZSelection.Value = 0;
                ParticleEndZSelection.Value = 0;
                particleSinZSelection.Value = 0;
                particleSinSpeedSelection.Value = 1;
                ParticleStartScaleSelection.Value = 1;
                ParticleEndScaleSelection.Value = 1;
                ParticleRotationSpeedSelection.Value = 0;
                ParticleColorBox1.BackColor = Color.White;
                ParticleStartAlphaSelection.Value = 1;
                ParticleColorBox2.BackColor = Color.White;
                ParticleEndAlphaSelection.Value = 1;
                ParticleMaxLifeSelection.Value = 1;
            }

        }

        private void PopulateParticleShapeSelections()
        {
            ParticleShapeSelection.Items.Clear();
            int max = Enum.GetValues(typeof(PaticleEmitterShape)).Cast<int>().Max() + 1;
            for (int i = 0; i < max; i++)
            {
                ParticleShapeSelection.Items.Add(((PaticleEmitterShape)i).ToString());
            }
        }

        private void PopulateParticleTextureSelections()
        {
            ParticleTextureSelection.Items.Clear();
            ParticleTextureSelection.Items.Add("None");
            if (Directory.Exists("Assets/Textures/Particles"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Particles", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                ParticleTextureSelection.Items.AddRange(files);
            }
        }

        private void ImportParticleTextureButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sourcePath = dialog.FileName;
                Image image = Image.FromFile(sourcePath);
                if (!Directory.Exists("Assets/Textures/Particles"))
                    Directory.CreateDirectory("Assets/Textures/Particles");
                string targetPath = "Assets/Textures/Particles/" + Path.GetFileName(sourcePath);
                File.Copy(sourcePath, targetPath, true);
                PopulateParticleTextureSelections();
            }
        }

        private void ParticleStartColourButton_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Color color = dialog.Color;
                ParticleColorBox1.BackColor = color;
            }
        }

        private void ParticleEndColourButton_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Color color = dialog.Color;
                ParticleColorBox2.BackColor = color;
            }
        }

        private void ParticlePreviewButton_Click(object sender, EventArgs e)
        {
            int selection = ParticlesListBox.SelectedIndex;
            if (selection != -1 && ParticlePreviewWindow.Instance == null)
            {
                ParticleEmitterData data = ParticleEmitterData.GetEmitterData(selection);
                ParticlePreviewWindow window = new ParticlePreviewWindow(data);
                window.Run();
            }
        }

        private void AddParticleButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ParticleEmitterData data = new Genus2D.GameData.ParticleEmitterData("Emitter " + (ParticlesListBox.Items.Count + 1).ToString("000"));
            Genus2D.GameData.ParticleEmitterData.AddEmitterData(data);
            PopulateEmittersList();
            ParticlesListBox.SelectedIndex = ParticlesListBox.Items.Count - 1;

        }

        private void RemoveParticleButton_Click(object sender, EventArgs e)
        {
            int selection = ParticlesListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ParticleEmitterData.RemoveEmitterData(selection);
                PopulateEmittersList();
            }
        }

        private void UndoParticleButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ParticleEmitterData.ReloadData();
            PopulateEmittersList();
        }

        private void ApplyParticleButton_Click(object sender, EventArgs e)
        {
            int selection = ParticlesListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ParticleEmitterData data = Genus2D.GameData.ParticleEmitterData.GetEmitterData(selection);
                data.Name = ParticleNameBox.Text;
                data.EmitterShape = (PaticleEmitterShape)ParticleShapeSelection.SelectedIndex;

                if (ParticleTextureSelection.SelectedIndex > 0)
                    data.ParticleTexture = (string)ParticleTextureSelection.Items[ParticleTextureSelection.SelectedIndex];
                else
                    data.ParticleTexture = "";

                data.EmissionRate = (float)ParticleEmissionSelection.Value;
                data.AngleMin = (float)ParticleAngleMinSelection.Value;
                data.AngleMax = (float)ParticleAngleMaxSelection.Value;
                data.OffsetMin = (float)ParticleOffsetMinSelection.Value;
                data.OffsetMax = (float)ParticleOffsetMaxSelection.Value;
                data.StartVelocity = (float)ParticleStartVelocitySelection.Value;
                data.EndVelocity = (float)ParticleEndVelocitySelection.Value;
                data.StartZ = (float)ParticleStartZSelection.Value;
                data.EndZ = (float)ParticleEndZSelection.Value;
                data.SinMaxZ = (float)particleSinZSelection.Value;
                data.SinSpeedZ = (float)particleSinSpeedSelection.Value;
                data.StartScale = (float)ParticleStartScaleSelection.Value;
                data.EndScale = (float)ParticleEndScaleSelection.Value;
                data.RotationSpeed = (float)ParticleRotationSpeedSelection.Value;

                Color sColour = ParticleColorBox1.BackColor;
                data.StartColour = new OpenTK.Graphics.Color4(sColour.R / 255f, sColour.G / 255f, sColour.B / 255f, (float)ParticleStartAlphaSelection.Value);

                Color eColour = ParticleColorBox2.BackColor;
                data.EndColour = new OpenTK.Graphics.Color4(eColour.R / 255f, eColour.G / 255f, eColour.B / 255f, (float)ParticleEndAlphaSelection.Value);

                data.MaxLife = (float)ParticleMaxLifeSelection.Value;

                PopulateEmittersList();
            }
            Genus2D.GameData.ParticleEmitterData.SaveItemData();
        }
        #endregion

        #region Class Functions

        private void PopulateClassesList()
        {
            int selection = ClassDataList.SelectedIndex;
            ClassDataList.Items.Clear();
            List<string> data = Genus2D.GameData.ClassData.GetClassNames();
            ClassDataList.Items.AddRange(data.ToArray());
            if (selection < data.Count - 1)
                ClassDataList.SelectedIndex = selection;
            else
                ChangeClassData();
        }

        private void ClassDataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeClassData();
        }

        private void ChangeClassData()
        {
            int selection = ClassDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ClassData data = Genus2D.GameData.ClassData.GetClass(selection);
                ClassNameBox.Text = data.Name;
                ClassVitalityControl.Value = data.BaseStats.Vitality;
                ClassInteligenceControl.Value = data.BaseStats.Inteligence;
                ClassStrengthControl.Value = data.BaseStats.Strength;
                ClassAgilityControl.Value = data.BaseStats.Agility;
                ClassMeleeDefenceControl.Value = data.BaseStats.MeleeDefence;
                ClassRangeDefenceControl.Value = data.BaseStats.RangeDefence;
                ClassMagicDefenceControl.Value = data.BaseStats.MagicDefence;
            }
            else
            {
                ClassNameBox.Text = "";
                ClassVitalityControl.Value = 0;
                ClassInteligenceControl.Value = 0;
                ClassStrengthControl.Value = 0;
                ClassAgilityControl.Value = 0;
                ClassMeleeDefenceControl.Value = 0;
                ClassRangeDefenceControl.Value = 0;
                ClassMagicDefenceControl.Value = 0;
            }
        }

        private void AddClassButton_Click(object sender, EventArgs e)
        {
            string name = "Class " + (Genus2D.GameData.ClassData.ClassesCount() + 1).ToString("000");
            Genus2D.GameData.ClassData.AddClass(name);
            PopulateClassesList();
            ClassDataList.SelectedIndex = ClassDataList.Items.Count - 1;
        }

        private void RemoveClassButton_Click(object sender, EventArgs e)
        {
            int selection = ClassDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ClassData.RemoveClass(selection);
                PopulateClassesList();
            }
        }

        private void UndoClassButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ClassData.ReloadData();
            PopulateClassesList();
        }

        private void SaveClassButton_Click(object sender, EventArgs e)
        {
            int selection = ClassDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ClassData data = Genus2D.GameData.ClassData.GetClass(selection);
                data.Name = ClassNameBox.Text;
                data.BaseStats.Vitality = (int)ClassVitalityControl.Value;
                data.BaseStats.Inteligence = (int)ClassInteligenceControl.Value;
                data.BaseStats.Strength = (int)ClassStrengthControl.Value;
                data.BaseStats.Agility = (int)ClassAgilityControl.Value;
                data.BaseStats.MeleeDefence = (int)ClassMeleeDefenceControl.Value;
                data.BaseStats.RangeDefence = (int)ClassRangeDefenceControl.Value;
                data.BaseStats.MagicDefence = (int)ClassMagicDefenceControl.Value;
            }
            Genus2D.GameData.ClassData.SaveData();
        }

        #endregion

        #region Enemy Functions

        private void PopulateEnemyList()
        {
            int selection = EnemyDataList.SelectedIndex;
            EnemyDataList.Items.Clear();
            List<string> data = Genus2D.GameData.EnemyData.GetEnemyNames();
            EnemyDataList.Items.AddRange(data.ToArray());
            if (selection < data.Count)
                EnemyDataList.SelectedIndex = selection;
            else
                ChangeEnemyData();
        }

        private void EnemyDataList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeEnemyData();
        }

        private void ChangeEnemyData()
        {
            int selection = EnemyDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.EnemyData data = Genus2D.GameData.EnemyData.GetEnemy(selection);
                EnemyNameBox.Text = data.Name;
                EnemyVitalityControl.Value = data.BaseStats.Vitality;
                EnemyInteligenceControl.Value = data.BaseStats.Inteligence;
                EnemyStrengthControl.Value = data.BaseStats.Strength;
                EnemyAgilityControl.Value = data.BaseStats.Agility;
                EnemyMeleeDefenceControl.Value = data.BaseStats.MeleeDefence;
                EnemyRangeDefenceControl.Value = data.BaseStats.RangeDefence;
                EnemyMagicDefenceControl.Value = data.BaseStats.MagicDefence;
                EnemyVisionRangeSelection.Value = data.VisionRage;
                EnemyAttackRangeSelection.Value = data.AttackRange;
                EnemyWanderRangeSelection.Value = data.WanderRange;
                EnemyExperienceSelection.Value = data.Experience;
                EnemyAgroLvlSelection.Value = data.AgroLvl;
                EnemyAtkStyleSelection.SelectedIndex = (int)data.AtkStyle - 1;
                EnemyProjectileSelection.SelectedIndex = data.ProjectileID + 1;
                EnemySpriteSelection.SelectedIndex = data.SpriteID + 1;
                EnemySpeedSelection.SelectedIndex = (int)data.Speed;
                EnemyDropTableSelection.SelectedIndex = data.DropTable + 1;
            }
            else
            {
                EnemyNameBox.Text = "";
                EnemyVitalityControl.Value = 0;
                EnemyInteligenceControl.Value = 0;
                EnemyStrengthControl.Value = 0;
                EnemyAgilityControl.Value = 0;
                EnemyMeleeDefenceControl.Value = 0;
                EnemyRangeDefenceControl.Value = 0;
                EnemyMagicDefenceControl.Value = 0;
                EnemyVisionRangeSelection.Value = 1;
                EnemyAttackRangeSelection.Value = 1;
                EnemyWanderRangeSelection.Value = 5;
                EnemyExperienceSelection.Value = 0;
                EnemyAgroLvlSelection.Value = 1;
                EnemyAtkStyleSelection.SelectedIndex = 0;
                EnemyProjectileSelection.SelectedIndex = 0;
                EnemySpriteSelection.SelectedIndex = 0;
                EnemySpeedSelection.SelectedIndex = 0;
                EnemyDropTableSelection.SelectedIndex = 0;
            }
        }

        private void AddEnemyButton_Click(object sender, EventArgs e)
        {
            string name = "Enemy " + (Genus2D.GameData.EnemyData.EmemiesCount() + 1).ToString("000");
            Genus2D.GameData.EnemyData.AddEnemy(name);
            PopulateEnemyList();
            EnemyDataList.SelectedIndex = EnemyDataList.Items.Count - 1;
        }

        private void RemoveEnemyButton_Click(object sender, EventArgs e)
        {
            int selection = EnemyDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.EnemyData.RemoveEnemy(selection);
                PopulateEnemyList();
            }
        }

        private void UndoEnemyChanges_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.EnemyData.ReloadData();
            PopulateEnemyList();
        }

        private void SaveEnemyChanges_Click(object sender, EventArgs e)
        {
            int selection = EnemyDataList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.EnemyData data = Genus2D.GameData.EnemyData.GetEnemy(selection);
                data.Name = EnemyNameBox.Text;
                data.BaseStats.Vitality = (int)EnemyVitalityControl.Value;
                data.BaseStats.Inteligence = (int)EnemyInteligenceControl.Value;
                data.BaseStats.Strength = (int)EnemyStrengthControl.Value;
                data.BaseStats.Agility = (int)EnemyAgilityControl.Value;
                data.BaseStats.MeleeDefence = (int)EnemyMeleeDefenceControl.Value;
                data.BaseStats.RangeDefence = (int)EnemyRangeDefenceControl.Value;
                data.BaseStats.MagicDefence = (int)EnemyMagicDefenceControl.Value;
                data.VisionRage = (int)EnemyVisionRangeSelection.Value;
                data.AttackRange = (int)EnemyAttackRangeSelection.Value;
                data.WanderRange = (int)EnemyWanderRangeSelection.Value;
                data.Experience = (int)EnemyExperienceSelection.Value;
                data.AgroLvl = (int)EnemyAgroLvlSelection.Value;
                data.AtkStyle = (Genus2D.GameData.AttackStyle)(EnemyAtkStyleSelection.SelectedIndex + 1);
                data.ProjectileID = EnemyProjectileSelection.SelectedIndex - 1;
                data.SpriteID = EnemySpriteSelection.SelectedIndex - 1;
                data.Speed = (Genus2D.GameData.MovementSpeed)EnemySpeedSelection.SelectedIndex;
                data.DropTable = EnemyDropTableSelection.SelectedIndex - 1;
            }
            Genus2D.GameData.EnemyData.SaveData();
        }

        #endregion

        #region Drop Table Data

        private void PopulateDropTablesList()
        {
            List<String> tables = Genus2D.GameData.DropTableData.GetDropTableNames();
            int selection = DropTableList.SelectedIndex;

            DropTableList.Items.Clear();
            DropTableList.Items.AddRange(tables.ToArray());

            EnemyDropTableSelection.Items.Clear();
            EnemyDropTableSelection.Items.Add("None");
            EnemyDropTableSelection.Items.AddRange(tables.ToArray());

            if (selection >= 0 && selection < DropTableList.Items.Count)
                DropTableList.SelectedIndex = selection;
            else
                SelectDropTable(-1);
        }

        private void DropTableList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectDropTable(DropTableList.SelectedIndex);
        }

        private void SelectDropTable(int selection)
        {
            DropTableItemsList.Items.Clear();
            if (selection >= 0)
            {
                Genus2D.GameData.DropTableData table = Genus2D.GameData.DropTableData.GetDropTable(selection);
                DropTableNameBox.Text = table.Name;
                List<Genus2D.GameData.DropTableData.DropTableItem> items = table.TableItems;
                for (int i = 0; i < items.Count; i++)
                {
                    DropTableItemsList.Items.Add(items[i].ToString());
                }
            }
            else
            {
                DropTableNameBox.Text = "";
            }

        }

        private void AddDropTableButton_Click(object sender, EventArgs e)
        {
            string name = "Drop Table " + (DropTableList.Items.Count + 1);
            Genus2D.GameData.DropTableData.AddDropTable(name);
            PopulateDropTablesList();
        }

        private void RemoveDropTableButton_Click(object sender, EventArgs e)
        {
            int selection = DropTableList.SelectedIndex;
            if (selection >= 0)
            {
                Genus2D.GameData.DropTableData.RemoveDropTable(selection);
                PopulateDropTablesList();
            }
        }

        private void AddDropItemButton_Click(object sender, EventArgs e)
        {
            int selection = DropTableList.SelectedIndex;
            if (selection >= 0)
            {
                Genus2D.GameData.DropTableData table = Genus2D.GameData.DropTableData.GetDropTable(selection);
                table.TableItems.Add(new Genus2D.GameData.DropTableData.DropTableItem());
                SelectDropTable(selection);
            }
        }

        private void EditDropItemButton_Click(object sender, EventArgs e)
        {
            int selection = DropTableList.SelectedIndex;
            if (selection >= 0)
            {
                int itemSelection = DropTableItemsList.SelectedIndex;
                if (itemSelection >= 0)
                {
                    Genus2D.GameData.DropTableData table = Genus2D.GameData.DropTableData.GetDropTable(selection);
                    EditDropItemForm form = new EditDropItemForm(table.TableItems[itemSelection]);
                    form.ShowDialog(this);
                    SelectDropTable(selection);
                }
            }
        }

        private void RemoveDropItemButton_Click(object sender, EventArgs e)
        {
            int selection = DropTableList.SelectedIndex;
            if (selection >= 0)
            {
                int itemSelection = DropTableItemsList.SelectedIndex;
                if (itemSelection >= 0)
                {
                    Genus2D.GameData.DropTableData table = Genus2D.GameData.DropTableData.GetDropTable(selection);
                    table.TableItems.RemoveAt(itemSelection);
                    SelectDropTable(selection);
                }
            }
        }

        private void UndoDropTable_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.DropTableData.ReloadData();
            PopulateDropTablesList();
        }

        private void ApplyDrobTable_Click(object sender, EventArgs e)
        {
            int selection = DropTableList.SelectedIndex;
            if (selection >= 0)
            {
                Genus2D.GameData.DropTableData.GetDropTable(selection).Name = DropTableNameBox.Text;
                PopulateDropTablesList();
            }
            Genus2D.GameData.DropTableData.SaveData();
        }

        #endregion

        #region Quest Data

        private void PopulateQuestList()
        {
            int selection = QuestList.SelectedIndex;
            QuestList.Items.Clear();
            List<string> names = Genus2D.GameData.QuestData.GetQuestNames();
            QuestList.Items.AddRange(names.ToArray());
            if (selection >= 0 && selection < names.Count)
                QuestList.SelectedIndex = selection;
            else
            {
                QuestList.SelectedIndex = -1;
                SelectQuest(-1);
            }
        }

        private void QuestList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectQuest(QuestList.SelectedIndex);
        }

        private void SelectQuest(int index)
        {
            int selectedObjective = QuestObjectivesList.SelectedIndex;
            QuestObjectivesList.Items.Clear();
            if (index != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(index);
                QuestNameBox.Text = quest.Name;
                List<string> objectiveNames = quest.GetObjectiveNames();
                QuestObjectivesList.Items.AddRange(objectiveNames.ToArray());
                if (selectedObjective >= 0 && selectedObjective < objectiveNames.Count)
                    QuestObjectivesList.SelectedIndex = selectedObjective;
                else
                    SelectQuestObjective(-1);
            }
            else
            {
                QuestNameBox.Text = "";
                SelectQuestObjective(-1);
            }
        }

        private void AddQuestButton_Click(object sender, EventArgs e)
        {
            string name = "Quest " + (Genus2D.GameData.QuestData.DataCount() + 1).ToString("000");
            Genus2D.GameData.QuestData.AddQuest(name);
            PopulateQuestList();
        }

        private void RemoveQuestButton_Click(object sender, EventArgs e)
        {
            int selection = QuestList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.QuestData.RemoveQuest(selection);
                PopulateQuestList();
            }
        }

        private void QuestObjectivesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectQuestObjective(QuestObjectivesList.SelectedIndex);
        }

        private void SelectQuestObjective(int index)
        {
            QuestRewardsList.Items.Clear();
            if (index != -1)
            {
                int selectedQuest = QuestList.SelectedIndex;
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                Genus2D.GameData.QuestData.QuestObective objective = quest.Objectives[index];
                QuestObjectiveNameBox.Text = objective.Name;
                QuestObjectiveDescriptionBox.Text = objective.Description;
                List<string> rewardNames = objective.GetItemRewardNames();
                QuestRewardsList.Items.AddRange(rewardNames.ToArray());
            }
            else
            {
                QuestObjectiveNameBox.Text = "";
                QuestObjectiveDescriptionBox.Text = "";
            }
        }

        private void AddQuestObjectiveButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            if (selectedQuest != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                string name = "Objective " + (quest.Objectives.Count + 1).ToString("000");
                quest.AddObjective(name);
                SelectQuest(selectedQuest);
            }
        }

        private void RemoveQuestObjectiveButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            if (selectedQuest != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                quest.RemoveObjective(selectedQuest);
                SelectQuest(selectedQuest);
            }
        }

        private void AddQuestRewardButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            int selectedObjective = QuestObjectivesList.SelectedIndex;
            if (selectedObjective != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                Genus2D.GameData.QuestData.QuestObective objective = quest.Objectives[selectedObjective];
                objective.ItemRewards.Add(new Tuple<int, int>(-1, 1));
                SelectQuestObjective(selectedObjective);
            }
        }

        private void EditQuestRewardButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            int selectedObjective = QuestObjectivesList.SelectedIndex;
            int selectedReward = QuestRewardsList.SelectedIndex;
            if (selectedReward != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                Genus2D.GameData.QuestData.QuestObective objective = quest.Objectives[selectedObjective];
                EditObjectiveRewardForm dialog = new EditObjectiveRewardForm(objective, selectedReward);
                dialog.ShowDialog(this);
                SelectQuestObjective(selectedObjective);
            }
        }

        private void RemoveQuestRewardButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            int selectedObjective = QuestObjectivesList.SelectedIndex;
            int selectedReward = QuestRewardsList.SelectedIndex;
            if (selectedReward != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                Genus2D.GameData.QuestData.QuestObective objective = quest.Objectives[selectedObjective];
                objective.ItemRewards.RemoveAt(selectedReward);
                SelectQuestObjective(selectedObjective);
            }
        }

        private void UndoQuestsButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.QuestData.ReloadData();
            PopulateQuestList();
        }

        private void ApplyQuestsButton_Click(object sender, EventArgs e)
        {
            int selectedQuest = QuestList.SelectedIndex;
            if (selectedQuest != -1)
            {
                Genus2D.GameData.QuestData quest = Genus2D.GameData.QuestData.GetData(selectedQuest);
                quest.Name = QuestNameBox.Text;
                int selectedObjective = QuestObjectivesList.SelectedIndex;
                if (selectedObjective != -1)
                {
                    Genus2D.GameData.QuestData.QuestObective objective = quest.Objectives[selectedObjective];
                    objective.Name = QuestObjectiveNameBox.Text;
                    objective.Description = QuestObjectiveDescriptionBox.Text;
                }
            }
            Genus2D.GameData.QuestData.SaveData();
            PopulateQuestList();
        }

        #endregion

        #region Shop Data

        private void PopulateShopList()
        {
            int selection = ShopList.SelectedIndex;
            ShopList.Items.Clear();
            List<string> names = Genus2D.GameData.ShopData.GetShopNames();
            ShopList.Items.AddRange(names.ToArray());
            if (selection >= 0 && selection < names.Count)
                ShopList.SelectedIndex = selection;
            else
            {
                ShopList.SelectedIndex = -1;
                SelectShop(-1);
            }
        }

        private void ShopList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectShop(ShopList.SelectedIndex);
        }

        private void SelectShop(int index)
        {
            int selectedShopItem = ShopItemsList.SelectedIndex;
            ShopItemsList.Items.Clear();
            if (index != -1)
            {
                Genus2D.GameData.ShopData shop = Genus2D.GameData.ShopData.GetData(index);
                ShopNameBox.Text = shop.Name;
                List<string> itemNames = shop.GetItemNames();
                ShopItemsList.Items.AddRange(itemNames.ToArray());
                if (selectedShopItem >= 0 && selectedShopItem < itemNames.Count)
                    ShopItemsList.SelectedIndex = selectedShopItem;
            }
            else
            {
                ShopNameBox.Text = "";
            }
        }

        private void AddShopButton_Click(object sender, EventArgs e)
        {
            string name = "Shop " + (Genus2D.GameData.ShopData.DataCount() + 1).ToString("000");
            Genus2D.GameData.ShopData.AddShop(name);
            PopulateShopList();

        }

        private void RemoveShopButton_Click(object sender, EventArgs e)
        {
            int selection = ShopList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ShopData.RemoveShop(selection);
                PopulateShopList();
            }
        }

        private void AddShopItemButton_Click(object sender, EventArgs e)
        {
            int selection = ShopList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ShopData.GetData(selection).AddItem();
                PopulateShopList();
            }
        }

        private void EditShopItemButton_Click(object sender, EventArgs e)
        {
            int shop = ShopList.SelectedIndex;
            int item = ShopItemsList.SelectedIndex;
            if (shop != -1 && item != -1)
            {
                Genus2D.GameData.ShopData.ShopItem shopItem;
                shopItem = Genus2D.GameData.ShopData.GetData(shop).ShopItems[item];
                EditShopItemForm form = new EditShopItemForm(shopItem);
                form.ShowDialog(this);
                PopulateShopList();
            }
        }

        private void RemoveShopItemButton_Click(object sender, EventArgs e)
        {
            int shop = ShopList.SelectedIndex;
            int item = ShopItemsList.SelectedIndex;
            if (shop != -1 && item != -1)
            {
                Genus2D.GameData.ShopData.GetData(shop).RemoveItem(item);
                PopulateShopList();
            }
        }

        private void UndoShopsButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.ShopData.ReloadData();
            PopulateShopList();
        }

        private void ApplyShopsButton_Click(object sender, EventArgs e)
        {
            int selection = ShopList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ShopData.GetData(selection).Name = ShopNameBox.Text;
            }
            Genus2D.GameData.ShopData.SaveData();
            PopulateShopList();
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

        #region System Data

        private void PopulateSytemData()
        {
            BaseXpCurveSelection.Value = Genus2D.GameData.SystemData.GetData().BaseXpCurve;
            XpPowSelection.Value = (decimal)Genus2D.GameData.SystemData.GetData().XpPower;
            XpDivSelection.Value = (decimal)Genus2D.GameData.SystemData.GetData().XpDivision;
            MaxLvlSelection.Value = Genus2D.GameData.SystemData.GetData().MaxLvl;
        }

        private void BaseXpCurveSelection_ValueChanged(object sender, EventArgs e)
        {
            SetTestExperience();
        }

        private void XpPowSelection_ValueChanged(object sender, EventArgs e)
        {
            SetTestExperience();
        }

        private void XpDivSelection_ValueChanged(object sender, EventArgs e)
        {
            SetTestExperience();
        }

        private void TestLvlSelection_ValueChanged(object sender, EventArgs e)
        {
            SetTestExperience();
        }

        private void SetTestExperience()
        {
            int baseXp = (int)BaseXpCurveSelection.Value;
            int targetLvl = (int)TestLvlSelection.Value;
            int targetXp = baseXp;

            for (int i = 1; i < targetLvl; i++)
            {
                targetXp += (int)(Math.Floor(i + baseXp * Math.Pow(2, i / (float)XpPowSelection.Value)) / (float)XpDivSelection.Value);
            }

            TestLvlLabel.Text = "(" + targetLvl + " to " + (targetLvl + 1) + ") " + targetXp;
        }

        private void UndoSystemDataButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SystemData.ReloadData();
            PopulateSytemData();
        }

        private void ApplySystemDataButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SystemData.GetData().BaseXpCurve = (int)BaseXpCurveSelection.Value;
            Genus2D.GameData.SystemData.GetData().XpPower = (float)XpPowSelection.Value;
            Genus2D.GameData.SystemData.GetData().XpDivision = (float)XpDivSelection.Value;
            Genus2D.GameData.SystemData.GetData().MaxLvl = (int)MaxLvlSelection.Value;
            Genus2D.GameData.SystemData.SaveData();
        }


        #endregion
    }
}
