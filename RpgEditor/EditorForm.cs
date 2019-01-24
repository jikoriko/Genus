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

            PopulateTilesetsList();
            PopulateTilesetSelections();
            PopulateEventsList();
            PopulateSpritesList();
            PopulateSpriteSelections();
        }

        /*
         * MAP FUNCTIONS
         */

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


        /*
         * TILESET FUNCTIONS 
         */

        private void PopulateTilesetsList()
        {
            List<string> tilesets = Genus2D.GameData.TilesetData.GetTilesetNames();
            TilesetsList.Items.Clear();
            TilesetsList.Items.AddRange(tilesets.ToArray());
        }

        private void PopulateTilesetSelections()
        {
            if (Directory.Exists("Assets/Textures/Tilesets"))
            {
                string[] files = Directory.GetFiles("Assets/Textures/Tilesets", "*.png");
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                TilesetSelectionBox.Items.Clear();
                TilesetSelectionBox.Items.AddRange(files);
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

        private void ApplyTilesetDataChange()
        {
            Genus2D.GameData.TilesetData.Tileset tileset = GetSelectedTileset();
            if (tileset != null)
            {
                int index = TilesetsList.SelectedIndex;
                tileset.Name = TilesetNameBox.Text;
                tileset.SetImagePath(TilesetSelectionBox.Text);
                Genus2D.GameData.TilesetData.SaveData();
                PopulateTilesetsList();
                TilesetsList.SelectedIndex = index;
            }
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
                ChangeTileset();
            }
        }

        /*
         * EVENT FUNCTIONS - the chunk of the code
         */

        private void PopulateEventsList()
        {
            int selection = EventsList.SelectedIndex;
            EventsList.Items.Clear();
            List<string> events = Genus2D.GameData.MapEventData.GetMapEventsDataNames();
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
                Genus2D.GameData.MapEventData eventData = Genus2D.GameData.MapEventData.GetMapEventData(selection);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[selection];

                Point pos = new Point(10, 10);

                switch (command.Type)
                {
                    case Genus2D.GameData.EventCommand.CommandType.EventWaitTimer:

                        NumericUpDown timerControl = new NumericUpDown();
                        timerControl.DecimalPlaces = 3;
                        timerControl.Value = (decimal)((float)command.GetParameter("Time"));

                        EventCommandDataPanel.Controls.Add(timerControl);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MapTransfer:

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

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MovePlayer:

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

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowMessage:

                        RichTextBox textBox = new RichTextBox();
                        textBox.Location = pos;
                        textBox.Size = new Size(EventCommandDataPanel.Width - 20, EventCommandDataPanel.Height - 20);
                        textBox.Text = (string)command.GetParameter("Message");

                        EventCommandDataPanel.Controls.Add(textBox);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.ShowOptions:

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
                        List<string> options = Genus2D.GameData.MapEventData.GetMapEventsDataNames();
                        options.Insert(0, "None");
                        eventOptionsBox.Items.AddRange(options.ToArray());
                        eventOptionsBox.SelectedIndexChanged += ChangeEventMessageOption;

                        EventCommandDataPanel.Controls.Add(textBox2);
                        EventCommandDataPanel.Controls.Add(messageOptionsBox);
                        EventCommandDataPanel.Controls.Add(removeOptionButton);
                        EventCommandDataPanel.Controls.Add(optionsTextBox);
                        EventCommandDataPanel.Controls.Add(addOptionButton);
                        EventCommandDataPanel.Controls.Add(eventOptionsBox);

                        break;
                }

            }
        }

        private void ChangeMessageOption(object sender, EventArgs e)
        {
            ComboBox messageOptionsBox = (ComboBox)sender;
            if (messageOptionsBox.SelectedIndex != -1)
            {
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
                Genus2D.GameData.EventCommand command = data.EventCommands[selection];

                switch (command.Type)
                {
                    case Genus2D.GameData.EventCommand.CommandType.EventWaitTimer:

                        NumericUpDown timerControl = (NumericUpDown)EventCommandDataPanel.Controls[0];
                        command.SetParameter("Time", (float)timerControl.Value);

                        break;
                    case Genus2D.GameData.EventCommand.CommandType.MapTransfer:

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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
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
            Genus2D.GameData.MapEventData data = new Genus2D.GameData.MapEventData("Event " + (Genus2D.GameData.MapEventData.MapEventsDataCount() + 1).ToString("000"));
            Genus2D.GameData.MapEventData.AddMapEventData(data);
            PopulateEventsList();
            EventsList.SelectedIndex = EventsList.Items.Count - 1;
        }

        private void RemoveEventButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.MapEventData.RemoveMapEventData(EventsList.SelectedIndex);
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
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
                data.AddEventCommand(command);
                PopulateEventCommandsList();
            }
        }

        private void RemoveEventCommandButton_Click(object sender, EventArgs e)
        {
            if (EventCommandsList.SelectedIndex != -1)
            {
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
                data.RemoveEventCommand(EventCommandsList.SelectedIndex);
                PopulateEventCommandsList();
            }
        }

        private void ApplyEventChangesButton_Click(object sender, EventArgs e)
        {
            if (EventsList.SelectedIndex != -1)
            {
                Genus2D.GameData.MapEventData data = Genus2D.GameData.MapEventData.GetMapEventData(EventsList.SelectedIndex);
                data.Name = EventNameBox.Text;
                data.SetTriggerType((Genus2D.GameData.MapEventData.TriggerType)EventTriggerBox.SelectedIndex);
                data.SetPassable(EventPassableCheck.Checked);
                data.SetSpriteID(EventSpriteSelection.SelectedIndex - 1);

                ApplyEventCommandData();

                Genus2D.GameData.MapEventData.SaveMapEventsData();
                PopulateEventsList();
            }
        }

        //sprite panel

        private void AddSpriteButton_Click(object sender, EventArgs e)
        {
            Genus2D.GameData.SpriteData.AddSpriteData();
            PopulateSpritesList();
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
                Genus2D.GameData.SpriteData.SaveData();
            }
            else
            {
                spriteViewerPanel.SetSprite(null);
            }
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
            for (int i = 0; i < Genus2D.GameData.SpriteData.NumSprites(); i++)
            {
                SpritesList.Items.Add("Sprite " + (i + 1).ToString("000"));
                EventSpriteSelection.Items.Add("Sprite " + (i + 1).ToString("000"));
            }
            if (selection < SpritesList.Items.Count)
                SpritesList.SelectedIndex = selection;
        }

        private void PopulateSpriteSelections()
        {
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
        }

        private void SpritesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = SpritesList.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.SpriteData sprite = Genus2D.GameData.SpriteData.GetSpriteData(selection);
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

    }
}
