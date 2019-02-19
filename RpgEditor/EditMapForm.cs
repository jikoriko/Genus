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
    public partial class EditMapForm : Form
    {

        private EditorForm _editor;
        private Genus2D.GameData.MapData _mapData;

        public EditMapForm(EditorForm editor, Genus2D.GameData.MapData mapData)
        {
            InitializeComponent();
            _editor = editor;
            _mapData = mapData;

            NameField.Text = mapData.GetMapName();
            WidthField.Value = mapData.GetWidth();
            HeightField.Value = mapData.GetHeight();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = NameField.Text;
            int width = (int)WidthField.Value;
            int height = (int)HeightField.Value;

            if (name == "")
            {
                MessageBox.Show("Please enter a map name.");
                return;
            }

            int mapID = Genus2D.GameData.MapInfo.GetMapID(_mapData.GetMapName());
            if (!Genus2D.GameData.MapInfo.RenameMap(mapID, name))
            {
                MessageBox.Show("A map with that name already exists.");
                return;
            }
            _mapData.SetMapName(name);

            Genus2D.GameData.MapInfo.ResizeMap(mapID, width, height);
            _mapData.Resize(width, height);
            Genus2D.GameData.MapInfo.SaveMap(_mapData);

            _editor.SetMapData(_mapData, mapID);

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int mapID = Genus2D.GameData.MapInfo.GetMapID(_mapData.GetMapName());

            Genus2D.GameData.MapInfo.DeleteMap(mapID);
            _editor.SetMapData(null, -1);

            this.Close();
        }
    }
}
