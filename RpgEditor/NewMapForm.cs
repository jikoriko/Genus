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
            NameField.Text = "Map " + (Genus2D.GameData.MapInfo.NumberMaps() + 1).ToString("000");
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
            else if (Genus2D.GameData.MapInfo.GetMapInfoStrings().Contains(name))
            {
                MessageBox.Show("A map with that name already exists.");
                return;
            }

            Genus2D.GameData.MapData map = new Genus2D.GameData.MapData(name, width, height);
            _editor.SetMapData(map, Genus2D.GameData.MapInfo.NumberMaps());
            Genus2D.GameData.MapInfo.SaveMap(map);

            this.Close();
        }
    }
}
