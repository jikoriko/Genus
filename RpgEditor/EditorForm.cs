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

        public enum MapTool
        {
            Pencil, Rectangle, FloodFill, Event, SpawnPoint, None
        }

        public enum TilesetProperties
        {
            Passabilities, Priorities, None
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

            splitContainer2.Panel1.Controls.Add(tilesetSelectionPanel);
            splitContainer2.Panel2.Controls.Add(mapPanel);
            panel2.Controls.Add(tilesetDataPanel);
            SpriteViewerParent.Controls.Add(spriteViewerPanel);

            PencilButton.Checked = true;
            PassabilitiesButton.Checked = true;

            InitializeDataPanels();
        }

        private void InitializeDataPanels()
        {
            PopulateTilesetsList();
            PopulateTilesetSelections();
            PopulateEventsList();
            PopulateSpritesList();
            PopulateSpriteSelections();
            PopulateItemList();
            PopulateItemIconSelections();
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

            int tilesetID = mapData.GetTilesetID();
            Genus2D.GameData.TilesetData.Tileset tileset = Genus2D.GameData.TilesetData.GetTileset(tilesetID);
            Image tilesetImage = Image.FromFile("Assets/Textures/Tilesets/" + tileset.ImagePath);
            tilesetSelectionPanel.SetTilesetImage(tilesetImage);
            mapPanel.TilesetImage = tilesetImage;

            tilesetSelectionPanel.Refresh();
            mapPanel.Refresh();

        }

        private void NewMapButton_Click(object sender, EventArgs e)
        {
            if (Genus2D.GameData.TilesetData.TilesetCount() == 0)
            {
                MessageBox.Show("Add tilesets first!");
                return;
            }

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
                if (tileset.ImagePath != "")
                    tilesetDataPanel.SetTilesetImage(Image.FromFile("Assets/Textures/Tilesets/" + tileset.ImagePath));
                else
                    tilesetDataPanel.SetTilesetImage(null);
            }
            else
            {
                TilesetNameBox.Text = "";
                TilesetSelectionBox.SelectedIndex = -1;
                tilesetDataPanel.SetTilesetImage(null);
            }
        }

        private void ImportTilesetButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG files | *.png; *.PNG;";
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (!Directory.Exists("Assets/Textures/Tilesets"))
                    Directory.CreateDirectory("Assets/Textures/Tilesets");
                string sourcePath = dialog.FileName;
                string targetPath = "Assets/Textures/Tilesets/" + Path.GetFileName(sourcePath);
                File.Copy(sourcePath, targetPath, true);
            }
            PopulateTilesetSelections();
        }

        private void ApplyTilesetDataChange()
        {
            Genus2D.GameData.TilesetData.Tileset tileset = GetSelectedTileset();
            if (tileset != null)
            {
                tileset.Name = TilesetNameBox.Text;
                tileset.SetImagePath(TilesetSelectionBox.Text);
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
            else if (PrioritiesButton.Checked) return TilesetProperties.Priorities;
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

        private void PrioritiesButton_CheckedChanged(object sender, EventArgs e)
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
                EventCommandsList.Items.AddRange(eventData.GetEventCommandStrings().ToArray());
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

                switch (command.Type)
                {
                    case Genus2D.GameData.EventCommand.CommandType.WaitTimer:
                        BuildWaitTimerCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportPlayer:
                        BuildTeleportPlayerCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MovePlayer:
                        BuildMovePlayerCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangePlayerDirection:
                        BuildChangePlayerDirectionCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportMapEvent:
                        BuildTeleportMapEventCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MoveMapEvent:
                        BuildMoveMapEventCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangeMapEventDirection:
                        BuildChangeMapEventDirectionCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowMessage:
                        BuildShowMessageCommands(command);
                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowOptions:
                        BuildShowOptionsCommand(command);
                        break;
                }

            }
        }

        private void BuildWaitTimerCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);
            NumericUpDown timerControl = new NumericUpDown();
            timerControl.Location = pos;
            timerControl.DecimalPlaces = 3;
            timerControl.Value = (decimal)((float)command.GetParameter("Time"));
            EventCommandDataPanel.Controls.Add(timerControl);
        }

        private void BuildTeleportPlayerCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);
            ComboBox mapSelection = new ComboBox();
            mapSelection.Location = pos;
            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            mapSelection.Items.AddRange(maps.ToArray());
            mapSelection.SelectedIndex = (int)command.GetParameter("MapID");

            NumericUpDown mapXControl = new NumericUpDown();
            pos.Y += mapSelection.Size.Height + 10;
            mapXControl.Location = pos;
            mapXControl.Value = (int)command.GetParameter("MapX");

            NumericUpDown mapYControl = new NumericUpDown();
            pos.Y += mapXControl.Size.Height + 10;
            mapYControl.Location = pos;
            mapYControl.Value = (int)command.GetParameter("MapY");

            EventCommandDataPanel.Controls.Add(mapSelection);
            EventCommandDataPanel.Controls.Add(mapXControl);
            EventCommandDataPanel.Controls.Add(mapYControl);
        }

        private void BuildMovePlayerCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);
            string[] directions = new string[4];
            for (int i = 0; i < 4; i++)
            {
                directions[i] = ((Genus2D.GameData.Direction)i).ToString();
            }

            ComboBox directionSelection = new ComboBox();
            directionSelection.Location = pos;
            directionSelection.Items.AddRange(directions);
            directionSelection.SelectedIndex = (int)((Genus2D.GameData.Direction)command.GetParameter("Direction"));

            EventCommandDataPanel.Controls.Add(directionSelection);
        }

        private void BuildChangePlayerDirectionCommands(Genus2D.GameData.EventCommand command)
        {
            BuildMovePlayerCommands(command);
        }

        private void BuildTeleportMapEventCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);

            ComboBox mapSelection = new ComboBox();
            mapSelection.Location = pos;
            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            mapSelection.Items.AddRange(maps.ToArray());
            mapSelection.SelectedIndexChanged += ChangeTeleportMapEventMapID;

            pos.Y += mapSelection.Size.Height + 10;
            ComboBox eventSelection = new ComboBox();
            eventSelection.Location = pos;

            NumericUpDown mapXControl = new NumericUpDown();
            pos.Y += mapSelection.Size.Height + 10;
            mapXControl.Location = pos;
            mapXControl.Value = (int)command.GetParameter("MapX");

            NumericUpDown mapYControl = new NumericUpDown();
            pos.Y += mapXControl.Size.Height + 10;
            mapYControl.Location = pos;
            mapYControl.Value = (int)command.GetParameter("MapY");

            EventCommandDataPanel.Controls.Add(mapSelection);
            EventCommandDataPanel.Controls.Add(eventSelection);
            EventCommandDataPanel.Controls.Add(mapXControl);
            EventCommandDataPanel.Controls.Add(mapYControl);

            mapSelection.SelectedIndex = (int)command.GetParameter("MapID");
            eventSelection.SelectedIndex = (int)command.GetParameter("EventID");
        }

        private void BuildMoveMapEventCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);

            ComboBox mapSelection = new ComboBox();
            mapSelection.Location = pos;
            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            mapSelection.Items.AddRange(maps.ToArray());
            mapSelection.SelectedIndexChanged += ChangeTeleportMapEventMapID;

            pos.Y += mapSelection.Size.Height + 10;
            ComboBox eventSelection = new ComboBox();
            eventSelection.Location = pos;

            string[] directions = new string[4];
            for (int i = 0; i < 4; i++)
            {
                directions[i] = ((Genus2D.GameData.Direction)i).ToString();
            }

            pos.Y += mapSelection.Size.Height + 10;
            ComboBox directionSelection = new ComboBox();
            directionSelection.Location = pos;
            directionSelection.Items.AddRange(directions);
            directionSelection.SelectedIndex = (int)((Genus2D.GameData.Direction)command.GetParameter("Direction"));

            EventCommandDataPanel.Controls.Add(mapSelection);
            EventCommandDataPanel.Controls.Add(eventSelection);
            EventCommandDataPanel.Controls.Add(directionSelection);

            mapSelection.SelectedIndex = (int)command.GetParameter("MapID");
            eventSelection.SelectedIndex = (int)command.GetParameter("EventID");
        }

        private void BuildChangeMapEventDirectionCommands(Genus2D.GameData.EventCommand command)
        {
            BuildMoveMapEventCommands(command);
        }

        private void BuildShowMessageCommands(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);
            RichTextBox textBox = new RichTextBox();
            textBox.Location = pos;
            textBox.Size = new Size(EventCommandDataPanel.Width - 20, EventCommandDataPanel.Height - 20);
            textBox.Text = (string)command.GetParameter("Message");
            EventCommandDataPanel.Controls.Add(textBox);
        }

        private void BuildShowOptionsCommand(Genus2D.GameData.EventCommand command)
        {
            Point pos = new Point(10, 10);

            RichTextBox textBox2 = new RichTextBox();
            textBox2.Location = pos;
            textBox2.Size = new Size(EventCommandDataPanel.Width - 20, EventCommandDataPanel.Height - 200);
            textBox2.Text = (string)command.GetParameter("Message");

            ComboBox messageOptionsBox = new ComboBox();
            pos.Y += textBox2.Height + 10;
            messageOptionsBox.Location = pos;
            List<Genus2D.GameData.MessageOption> messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
            for (int i = 0; i < messageOptions.Count; i++)
            {
                messageOptionsBox.Items.Add(messageOptions[i].Option);
            }
            messageOptionsBox.SelectedIndexChanged += ChangeMessageOption;

            Button removeOptionButton = new Button();
            pos.X += messageOptionsBox.Width + 10;
            removeOptionButton.Location = pos;
            removeOptionButton.Text = "Remove Option";
            removeOptionButton.Click += RemoveMessageOption;
            pos.X = 10;

            TextBox optionsTextBox = new TextBox();
            pos.Y += messageOptionsBox.Height + 10;
            optionsTextBox.Location = pos;

            Button addOptionButton = new Button();
            pos.X += optionsTextBox.Width + 10;
            addOptionButton.Location = pos;
            addOptionButton.Text = "Add Option";
            addOptionButton.Click += AddMessageOption;
            pos.X = 10;

            ComboBox eventOptionsBox = new ComboBox();
            pos.Y += optionsTextBox.Height + 10;
            eventOptionsBox.Location = pos;
            List<string> options = Genus2D.GameData.EventData.GetEventsDataNames();
            options.Insert(0, "None");
            eventOptionsBox.Items.AddRange(options.ToArray());
            eventOptionsBox.SelectedIndexChanged += ChangeEventMessageOption;

            EventCommandDataPanel.Controls.Add(textBox2);
            EventCommandDataPanel.Controls.Add(messageOptionsBox);
            EventCommandDataPanel.Controls.Add(removeOptionButton);
            EventCommandDataPanel.Controls.Add(optionsTextBox);
            EventCommandDataPanel.Controls.Add(addOptionButton);
            EventCommandDataPanel.Controls.Add(eventOptionsBox);
        }

        private void ChangeTeleportMapEventMapID(object sender, EventArgs e)
        {
            ComboBox mapSelectionBox = (ComboBox)sender;
            ComboBox eventSelectionBox = (ComboBox)EventCommandDataPanel.Controls[1];
            eventSelectionBox.Items.Clear();
            int selection = mapSelectionBox.SelectedIndex;
            if (selection != -1)
            {
                for (int i = 0; i < Genus2D.GameData.MapInfo.GetMapInfo(selection).NumberMapEvents; i++)
                {
                    eventSelectionBox.Items.Add("Map Event " + (i + 1).ToString("000"));
                }
            }
        }

        private void ChangeMoveMapEventMapID(object sender, EventArgs e)
        {
            ComboBox mapSelectionBox = (ComboBox)sender;
            ComboBox eventSelectionBox = (ComboBox)EventCommandDataPanel.Controls[1];
            eventSelectionBox.Items.Clear();
            int selection = mapSelectionBox.SelectedIndex;
            if (selection != -1)
            {
                for (int i = 0; i < Genus2D.GameData.MapInfo.GetMapInfo(selection).NumberMapEvents; i++)
                {
                    eventSelectionBox.Items.Add("Map Event " + (i + 1).ToString("000"));
                }
            }
        }

        private void ChangeMessageOption(object sender, EventArgs e)
        {
            ComboBox messageOptionsBox = (ComboBox)sender;
            if (messageOptionsBox.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[EventCommandsList.SelectedIndex];
                List<Genus2D.GameData.MessageOption> messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
                ((ComboBox)EventCommandDataPanel.Controls[5]).SelectedIndex = messageOptions[messageOptionsBox.SelectedIndex].OptionEventID + 1;
            }
            else
            {
                ((ComboBox)EventCommandDataPanel.Controls[5]).SelectedIndex = -1;
            }
        }

        private void ChangeEventMessageOption(object sender, EventArgs e)
        {
            ComboBox messageOptionsBox = (ComboBox)EventCommandDataPanel.Controls[1];
            if (messageOptionsBox.SelectedIndex != -1)
            {
                ComboBox eventOptionsBox = (ComboBox)sender;
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[EventCommandsList.SelectedIndex];
                List<Genus2D.GameData.MessageOption> messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
                messageOptions[messageOptionsBox.SelectedIndex].OptionEventID = eventOptionsBox.SelectedIndex - 1;
            }
        }

        private void AddMessageOption(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)EventCommandDataPanel.Controls[3];
            if (textBox.Text != "")
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[EventCommandsList.SelectedIndex];
                List<Genus2D.GameData.MessageOption> messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
                for (int i = 0; i < messageOptions.Count; i++)
                {
                    if (messageOptions[i].Option == textBox.Text)
                    {
                        MessageBox.Show("Option already exists");
                        return;
                    }
                }

                Genus2D.GameData.MessageOption option = new Genus2D.GameData.MessageOption();
                option.Option = textBox.Text;
                messageOptions.Add(option);
                ((ComboBox)EventCommandDataPanel.Controls[1]).Items.Add(textBox.Text);
                ((ComboBox)EventCommandDataPanel.Controls[1]).SelectedIndex = ((ComboBox)EventCommandDataPanel.Controls[1]).Items.Count - 1;
                textBox.Text = "";
            }
        }

        private void RemoveMessageOption(object sender, EventArgs e)
        {
            ComboBox optionsBox = (ComboBox)EventCommandDataPanel.Controls[1];
            if (optionsBox.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[EventCommandsList.SelectedIndex];
                List<Genus2D.GameData.MessageOption> messageOptions = (List<Genus2D.GameData.MessageOption>)command.GetParameter("Options");
                messageOptions.RemoveAt(optionsBox.SelectedIndex);
                optionsBox.Items.RemoveAt(optionsBox.SelectedIndex);
                optionsBox.SelectedIndex = -1;
                optionsBox.Text = "";
                ((ComboBox)EventCommandDataPanel.Controls[5]).SelectedIndex = -1;
            }
        }

        private void ApplyEventCommandData()
        {
            int selection = EventCommandsList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[selection];

                switch (command.Type)
                {
                    case Genus2D.GameData.EventCommand.CommandType.WaitTimer:

                        NumericUpDown timerControl = (NumericUpDown)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Time", (float)timerControl.Value);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportPlayer:

                        ComboBox mapSelection = (ComboBox)EventCommandDataPanel.Controls[0];
                        NumericUpDown mapXControl = (NumericUpDown)EventCommandDataPanel.Controls[1];
                        NumericUpDown mapYControl = (NumericUpDown)EventCommandDataPanel.Controls[2];
                        Genus2D.GameData.MapInfo mapInfo = Genus2D.GameData.MapInfo.GetMapInfo(mapSelection.SelectedIndex);
                        if (mapXControl.Value < mapInfo.Width && mapYControl.Value < mapInfo.Height)
                        {
                            command.SetParameter("MapID", mapSelection.SelectedIndex);
                            command.SetParameter("MapX", (int)mapXControl.Value);
                            command.SetParameter("MapY", (int)mapYControl.Value);
                        }
                        else
                        {
                            MessageBox.Show("Map X or Y coordinate out of map bounds.");
                        }

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MovePlayer:

                        ComboBox directionSelection = (ComboBox)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Direction", (Genus2D.GameData.Direction)directionSelection.SelectedIndex);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangePlayerDirection:

                        ComboBox directionSelection2 = (ComboBox)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Direction", (Genus2D.GameData.Direction)directionSelection2.SelectedIndex);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.TeleportMapEvent:

                        ComboBox mapSelection2 = (ComboBox)EventCommandDataPanel.Controls[0];
                        if (mapSelection2.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid map.");
                            return;
                        }
                        ComboBox eventSelection = (ComboBox)EventCommandDataPanel.Controls[1];
                        if (eventSelection.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid event.");
                            return;
                        }
                        NumericUpDown mapXControl2 = (NumericUpDown)EventCommandDataPanel.Controls[2];
                        NumericUpDown mapYControl2 = (NumericUpDown)EventCommandDataPanel.Controls[3];
                        Genus2D.GameData.MapInfo mapInfo2 = Genus2D.GameData.MapInfo.GetMapInfo(mapSelection2.SelectedIndex);
                        if (mapXControl2.Value < mapInfo2.Width && mapYControl2.Value < mapInfo2.Height)
                        {
                            command.SetParameter("MapID", mapSelection2.SelectedIndex);
                            command.SetParameter("EventID", eventSelection.SelectedIndex);
                            command.SetParameter("MapX", (int)mapXControl2.Value);
                            command.SetParameter("MapY", (int)mapYControl2.Value);
                        }
                        else
                        {
                            MessageBox.Show("Map X or Y coordinate out of map bounds.");
                        }

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MoveMapEvent:

                        ComboBox mapSelection3 = (ComboBox)EventCommandDataPanel.Controls[0];
                        if (mapSelection3.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid map.");
                            return;
                        }
                        ComboBox eventSelection2 = (ComboBox)EventCommandDataPanel.Controls[1];
                        if (eventSelection2.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid event.");
                            return;
                        }

                        ComboBox directionSelection3 = (ComboBox)EventCommandDataPanel.Controls[2];

                        command.SetParameter("MapID", mapSelection3.SelectedIndex);
                        command.SetParameter("EventID", eventSelection2.SelectedIndex);
                        command.SetParameter("Direction", (Genus2D.GameData.Direction)directionSelection3.SelectedIndex);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ChangeMapEventDirection:

                        ComboBox mapSelection4 = (ComboBox)EventCommandDataPanel.Controls[0];
                        if (mapSelection4.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid map.");
                            return;
                        }
                        ComboBox eventSelection3 = (ComboBox)EventCommandDataPanel.Controls[1];
                        if (eventSelection3.SelectedIndex == -1)
                        {
                            MessageBox.Show("Select a valid event.");
                            return;
                        }

                        ComboBox directionSelection4 = (ComboBox)EventCommandDataPanel.Controls[2];

                        command.SetParameter("MapID", mapSelection4.SelectedIndex);
                        command.SetParameter("EventID", eventSelection3.SelectedIndex);
                        command.SetParameter("Direction", (Genus2D.GameData.Direction)directionSelection4.SelectedIndex);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowMessage:

                        RichTextBox textBox = (RichTextBox)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Message", textBox.Text);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowOptions:

                        RichTextBox textBox2 = (RichTextBox)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Message", textBox2.Text);

                        break;
                }

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
                EventTriggerBox.SelectedIndex = (int)data.GetTriggerType();
                EventPassableCheck.Checked = data.Passable();
                EventSpriteSelection.SelectedIndex = data.GetSpriteID() + 1;
            }
            else
            {
                EventNameBox.Text = "";
                EventTriggerBox.SelectedIndex = -1;
                EventPassableCheck.Checked = false;
                EventSpriteSelection.SelectedIndex = 0;
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

        public void AddEventCommand(Genus2D.GameData.EventCommand.CommandType command)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                data.AddEventCommand(command);
                PopulateEventCommandsList();
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

        private void ApplyEventChangesButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.EventData data = Genus2D.GameData.EventData.GetEventData(EventsList.SelectedIndex);
                data.Name = EventNameBox.Text;
                data.SetTriggerType((Genus2D.GameData.EventData.TriggerType)EventTriggerBox.SelectedIndex);
                data.SetPassable(EventPassableCheck.Checked);
                data.SetSpriteID(EventSpriteSelection.SelectedIndex - 1);

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
            EventSpriteSelection.Items.Clear();
            EventSpriteSelection.Items.Add("None");
            EventSpriteSelection.SelectedIndex = 0;

            List<string> spriteNames = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpritesList.Items.AddRange(spriteNames.ToArray());
            EventSpriteSelection.Items.AddRange(spriteNames.ToArray());

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
                if (!Directory.Exists("Assets/Textures/Sprites"))
                    Directory.CreateDirectory("Assets/Textures/Sprites");
                string sourcePath = dialog.FileName;
                string targetPath = "Assets/Textures/Sprites/" + Path.GetFileName(sourcePath);
                File.Copy(sourcePath, targetPath, true);
            }
            PopulateSpriteSelections();
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
                ItemIconSelection.Text = data.IconImage;
                ItemTypeSelection.SelectedIndex = (int)data.GetItemType();
                ItemMaxStack.Value = data.GetMaxStack();
            }
            else
            {
                ItemNameBox.Text = "";
                ItemIconSelection.SelectedIndex = -1;
                ItemTypeSelection.SelectedIndex = 0;
                ItemMaxStack.Value = 1;
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
                if (!Directory.Exists("Assets/Textures/Icons"))
                    Directory.CreateDirectory("Assets/Textures/Icons");
                string sourcePath = dialog.FileName;
                string targetPath = "Assets/Textures/Icons/" + Path.GetFileName(sourcePath);
                File.Copy(sourcePath, targetPath, true);
            }
            PopulateItemIconSelections();
        }

        private void ApplyItemButton_Click(object sender, EventArgs e)
        {
            int selection = ItemListBox.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.ItemData data = Genus2D.GameData.ItemData.GetItemData(selection);
                data.Name = ItemNameBox.Text;
                data.IconImage = ItemIconSelection.SelectedText;
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

    }
}
