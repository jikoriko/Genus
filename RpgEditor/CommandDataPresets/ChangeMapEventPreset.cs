using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor.CommandDataPresets
{
    public partial class ChangeMapEventPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeMapEventPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();

            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            MapSelection.Items.AddRange(maps.ToArray());

            List<string> sprites = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpriteSelection.Items.Add("None");
            SpriteSelection.Items.AddRange(sprites.ToArray());

            MapSelection.SelectedIndex = (int)command.GetParameter("MapID");
            EventSelection.SelectedIndex = (int)command.GetParameter("EventID");
            SetMapEventProperty((Genus2D.GameData.ChangeMapEventProperty)command.GetParameter("Property"));
            MapXSelection.Value = (int)command.GetParameter("MapX");
            MapYSelection.Value = (int)command.GetParameter("MapY");
            MovementSelection.SelectedIndex = (int)command.GetParameter("MovementDirection");
            DirectionSelection.SelectedIndex = (int)command.GetParameter("FacingDirection");
            SpriteSelection.SelectedIndex = (int)command.GetParameter("SpriteID") + 1;
            RenderPrioritySelection.SelectedIndex = (int)command.GetParameter("RenderPriority");
            MovementSpeedSelection.SelectedIndex = (int)command.GetParameter("MovementSpeed");
            MovementFrequencySelection.SelectedIndex = (int)command.GetParameter("MovementFrequency");
            PassableCheck.Checked = (bool)command.GetParameter("Passable");
            RandomMovementCheck.Checked = (bool)command.GetParameter("RandomMovement");
            EnabledCheck.Checked = (bool)command.GetParameter("Enabled");
        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventSelection.Items.Clear();
            int selection = MapSelection.SelectedIndex;

            if (selection != -1)
            {
                MapXSelection.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Width;
                MapYSelection.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Height;

                Genus2D.GameData.MapData data = Genus2D.GameData.MapInfo.LoadMap(selection);
                for (int i = 0; i < data.MapEventsCount(); i++)
                {
                    EventSelection.Items.Add(data.GetMapEvent(i).Name);
                }
                EventSelection.SelectedIndex = EventSelection.Items.Count > 0 ? 0 : -1;
            }
            else
            {
                EventSelection.SelectedIndex = -1;
                MapXSelection.Maximum = 0;
                MapYSelection.Maximum = 0;
            }

            MapXSelection.Value = 0;
            MapYSelection.Value = 0;
        }

        private Genus2D.GameData.ChangeMapEventProperty GetMapEventProperty()
        {
            if (TeleportRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.Teleport;
            if (MovementRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.Move;
            if (DirectionRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.Direction;
            if (SpriteRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.Sprite;
            if (RenderPriorityRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.RenderPriority;
            if (MovementSpeedRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.MovementSpeed;
            if (MovementFrequencyRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.MovementFrequency;
            if (PassableRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.Passable;
            if (RandomMovementRadio.Checked) return Genus2D.GameData.ChangeMapEventProperty.RandomMovement;
            return Genus2D.GameData.ChangeMapEventProperty.Enabled;
        }

        private void SetMapEventProperty(Genus2D.GameData.ChangeMapEventProperty property)
        {
            switch (property)
            {
                case Genus2D.GameData.ChangeMapEventProperty.Teleport:
                    TeleportRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.Move:
                    MovementRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.Direction:
                    DirectionRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.Sprite:
                    SpriteRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.RenderPriority:
                    RenderPriorityRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.MovementSpeed:
                    MovementSpeedRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.MovementFrequency:
                    MovementFrequencyRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.Passable:
                    PassableRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.RandomMovement:
                    RandomMovementRadio.Checked = true;
                    break;
                case Genus2D.GameData.ChangeMapEventProperty.Enabled:
                    EnabledRadio.Checked = true;
                    break;
            }
        }

        public void ApplyData()
        {
            if (MapSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Select a valid map.");
                return;
            }

            if (EventSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Select a valid event.");
                return;
            }

            _command.SetParameter("MapID", MapSelection.SelectedIndex);
            _command.SetParameter("EventID", EventSelection.SelectedIndex);
            _command.SetParameter("Property", GetMapEventProperty());
            _command.SetParameter("MapX", (int)MapXSelection.Value);
            _command.SetParameter("MapY", (int)MapYSelection.Value);
            _command.SetParameter("MovementDirection", (Genus2D.GameData.MovementDirection)MovementSelection.SelectedIndex);
            _command.SetParameter("FacingDirection", (Genus2D.GameData.FacingDirection)DirectionSelection.SelectedIndex);
            _command.SetParameter("SpriteID", (int)SpriteSelection.SelectedIndex - 1);
            _command.SetParameter("RenderPriority", (Genus2D.GameData.RenderPriority)RenderPrioritySelection.SelectedIndex);
            _command.SetParameter("MovementSpeed", (Genus2D.GameData.MovementSpeed)MovementSpeedSelection.SelectedIndex);
            _command.SetParameter("MovementFrequency", (Genus2D.GameData.MovementFrequency)MovementFrequencySelection.SelectedIndex);
            _command.SetParameter("Passable", PassableCheck.Checked);
            _command.SetParameter("RandomMovement", RandomMovementCheck.Checked);
            _command.SetParameter("Enabled", EnabledCheck.Checked);
        }
    }
}
