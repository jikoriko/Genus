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
    public partial class TeleportPlayerPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public TeleportPlayerPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            MapSelection.Items.AddRange(maps.ToArray());
            int mapID = (int)command.GetParameter("MapID");
            MapSelection.SelectedIndex = mapID;

            MapX.Value = (int)command.GetParameter("MapX");
            MapY.Value = (int)command.GetParameter("MapY");

            MapX.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Width - 1;
            MapY.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Height - 1;
        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int mapID = MapSelection.SelectedIndex;
            MapX.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Width - 1;
            MapY.Maximum = Genus2D.GameData.MapInfo.GetMapInfo(mapID).Height - 1;
        }

        public void ApplyData()
        {
            Genus2D.GameData.MapInfo mapInfo = Genus2D.GameData.MapInfo.GetMapInfo(MapSelection.SelectedIndex);
            if (MapX.Value < mapInfo.Width && MapY.Value < mapInfo.Height)
            {
                _command.SetParameter("MapID", MapSelection.SelectedIndex);
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
