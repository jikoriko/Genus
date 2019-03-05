using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public partial class EditMapEventForm : Form
    {
        private Genus2D.GameData.MapEvent _mapEvent;

        public EditMapEventForm(Genus2D.GameData.MapEvent mapEvent)
        {
            InitializeComponent();

            _mapEvent = mapEvent;

            NameBox.Text = _mapEvent.Name;
            List<string> events = Genus2D.GameData.EventData.GetEventsDataNames();
            EventSelection.Items.Add("None");
            EventSelection.Items.AddRange(events.ToArray());
            if (mapEvent.EventID + 1 < EventSelection.Items.Count)
                EventSelection.SelectedIndex = mapEvent.EventID + 1;
            else
                EventSelection.SelectedIndex = 0;

            EventDirectionSelection.SelectedIndex = (int)mapEvent.EventDirection;

            EventSpriteSelection.Items.Add("None");
            List<string> spriteNames = Genus2D.GameData.SpriteData.GetSpriteNames();
            EventSpriteSelection.Items.AddRange(spriteNames.ToArray());
            if (mapEvent.SpriteID + 1 < EventSpriteSelection.Items.Count)
                EventSpriteSelection.SelectedIndex = mapEvent.SpriteID + 1;
            else
                EventSpriteSelection.SelectedIndex = 0;

            EventTriggerTypeSelection.SelectedIndex = (int)mapEvent.TriggerType;
            RenderPrioritySelection.SelectedIndex = (int)mapEvent.Priority;
            SpeedSelection.SelectedIndex = (int)mapEvent.Speed;
            FrequencySelection.SelectedIndex = (int)mapEvent.Frequency;
            EventPassableCheck.Checked = mapEvent.Passable;
            RandomMovementCheck.Checked = mapEvent.RandomMovement;
            EnabledCheck.Checked = mapEvent.Enabled;

            ApplyData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ApplyData();
            this.Close();
        }

        private void ApplyData()
        {
            if (NameBox.Text != "")
                _mapEvent.Name = NameBox.Text;
            _mapEvent.EventID = EventSelection.SelectedIndex - 1;
            _mapEvent.EventDirection = (Genus2D.GameData.FacingDirection)EventDirectionSelection.SelectedIndex;
            _mapEvent.SpriteID = EventSpriteSelection.SelectedIndex - 1;
            _mapEvent.TriggerType = (Genus2D.GameData.EventTriggerType)EventTriggerTypeSelection.SelectedIndex;
            _mapEvent.Priority = (Genus2D.GameData.RenderPriority)RenderPrioritySelection.SelectedIndex;
            _mapEvent.Speed = (Genus2D.GameData.MovementSpeed)SpeedSelection.SelectedIndex;
            _mapEvent.Frequency = (Genus2D.GameData.MovementFrequency)FrequencySelection.SelectedIndex;
            _mapEvent.Passable = EventPassableCheck.Checked;
            _mapEvent.RandomMovement = RandomMovementCheck.Checked;
            _mapEvent.Enabled = EnabledCheck.Checked;
        }

    }
}
