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
    public partial class EditEventForm : Form
    {
        private Genus2D.GameData.MapEvent _mapEvent;

        public EditEventForm(Genus2D.GameData.MapEvent mapEvent)
        {
            InitializeComponent();

            _mapEvent = mapEvent;

            List<string> events = Genus2D.GameData.MapEventData.GetMapEventsDataNames();
            EventSelection.Items.AddRange(events.ToArray());
            EventSelection.SelectedIndex = _mapEvent.EventID;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (EventSelection.SelectedIndex != -1)
            {
                _mapEvent.EventID = EventSelection.SelectedIndex;
                this.Close();
            }
        }
    }
}
