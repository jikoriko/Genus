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
    public partial class TeleportMapEventPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public TeleportMapEventPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            MapSelection.Items.AddRange(maps.ToArray());

            MapSelection.SelectedIndex = (int)command.GetParameter("MapID");

            EventSelection.SelectedIndex = (int)command.GetParameter("EventID");
            MapX.Value = (int)command.GetParameter("MapX");
            MapY.Value = (int)command.GetParameter("MapY");

        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventSelection.Items.Clear();
            int selection = MapSelection.SelectedIndex;
            if (selection != -1)
            {
                MapX.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Width;
                MapY.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(selection).Height;

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
                MapX.Maximum = 0;
                MapY.Maximum = 0;
            }

            MapX.Value = 0;
            MapY.Value = 0;
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

            Genus2D.GameData.MapInfo mapInfo = Genus2D.GameData.MapInfo.GetMapInfo(MapSelection.SelectedIndex);
            if (MapX.Value < mapInfo.Width && MapY.Value < mapInfo.Height)
            {
                _command.SetParameter("MapID", MapSelection.SelectedIndex);
                _command.SetParameter("EventID", EventSelection.SelectedIndex);
                _command.SetParameter("MapX", (int)MapX.Value);
                _command.SetParameter("MapY", (int)MapY.Value);
            }
            else
            {
                MessageBox.Show("Map X or Y coordinate out of map bounds.");
            }
        }
    }
}
