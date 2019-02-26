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
    public partial class ChangeMapEventSprite : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeMapEventSprite(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();

            _command = command;

            List<string> maps = Genus2D.GameData.MapInfo.GetMapInfoStrings();
            MapSelection.Items.AddRange(maps.ToArray());

            MapSelection.SelectedIndex = (int)command.GetParameter("MapID");
            MapEventSelection.SelectedIndex = (int)command.GetParameter("EventID");

            List<string> sprites = Genus2D.GameData.SpriteData.GetSpriteNames();
            SpriteSelection.Items.Add("None");
            SpriteSelection.Items.AddRange(sprites.ToArray());
            SpriteSelection.SelectedIndex = (int)command.GetParameter("SpriteID") + 1;
        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapEventSelection.Items.Clear();
            int selection = MapSelection.SelectedIndex;
            if (selection != -1)
            {
                Genus2D.GameData.MapData data = Genus2D.GameData.MapInfo.LoadMap(selection);
                for (int i = 0; i < data.MapEventsCount(); i++)
                {
                    MapEventSelection.Items.Add(data.GetMapEvent(i).Name);
                }
                MapEventSelection.SelectedIndex = MapEventSelection.Items.Count > 0 ? 0 : -1;
            }
            else
            {
                MapEventSelection.SelectedIndex = -1;
            }
        }

        public void ApplyData()
        {
            if (MapSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Select a valid map.");
                return;
            }

            if (MapEventSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Select a valid event.");
                return;
            }


            _command.SetParameter("MapID", MapSelection.SelectedIndex);
            _command.SetParameter("EventID", MapEventSelection.SelectedIndex);
            _command.SetParameter("SpriteID", SpriteSelection.SelectedIndex - 1);
        }
    }
}
