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
    public partial class LoadMapForm : Form
    {

        private EditorForm _editor;

        public LoadMapForm(EditorForm editor)
        {
            InitializeComponent();

            _editor = editor;

            MapSelection.Items.AddRange(Genus2D.GameData.MapInfo.GetMapInfoStrings().ToArray());
            MapSelection.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MapSelection.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a map");
                return;
            }

            Genus2D.GameData.MapData map = Genus2D.GameData.MapInfo.LoadMap(MapSelection.SelectedIndex);
            _editor.SetMapData(map, MapSelection.SelectedIndex);

            this.Close();

        }
    }
}
