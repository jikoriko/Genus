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
    public partial class NewMapForm : Form
    {

        private EditorForm _editor;

        public NewMapForm(EditorForm editor)
        {
            InitializeComponent();
            _editor = editor;

            List<string> tilesets = Genus2D.GameData.TilesetData.GetTilesetNames();
            TilesetSelection.Items.AddRange(tilesets.ToArray());
            TilesetSelection.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = NameField.Text;
            int width = (int)WidthField.Value;
            int height = (int)HeightField.Value;
            int tileset = TilesetSelection.SelectedIndex;

            if (name == "")
            {
                MessageBox.Show("Please enter a map name." + tileset);
                return;
            }
            else if (Genus2D.GameData.MapInfo.GetMapInfoStrings().Contains(name))
            {
                MessageBox.Show("A map with that name already exists.");
                return;
            }
            else if (tileset == -1)
            {
                MessageBox.Show("Please select a tileset.");
                return;
            }

            Genus2D.GameData.MapData map = new Genus2D.GameData.MapData(name, width, height, tileset);
            _editor.SetMapData(map, Genus2D.GameData.MapInfo.NumberMaps());

            this.Close();
        }
    }
}
