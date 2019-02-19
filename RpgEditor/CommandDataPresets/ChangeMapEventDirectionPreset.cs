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
    public partial class ChangeMapEventDirection : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeMapEventDirection(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();

            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            MapSelection.Items.AddRange(maps.ToArray());

            MapSelection.SelectedIndex = (int)command.GetParameter("MapID");
            EventSelection.SelectedIndex = (int)command.GetParameter("EventID");

            string[] directions = new string[4];
            for (int i = 0; i < 4; i++)
            {
                directions[i] = ((Genus2D.GameData.FacingDirection)i).ToString();
            }
            DirectionSelection.Items.AddRange(directions);
            DirectionSelection.SelectedIndex = (int)command.GetParameter("Direction");
        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventSelection.Items.Clear();
            int selection = MapSelection.SelectedIndex;
            if (selection != -1)
            {
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
            _command.SetParameter("Direction", (Genus2D.GameData.FacingDirection)DirectionSelection.SelectedIndex);
        }
    }
}
