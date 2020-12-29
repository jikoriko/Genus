using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genus2D.GameData;
using static Genus2D.GameData.DropTableData;

namespace RpgEditor
{
    public partial class EditCraftingMaterialForm : Form
    {
        private CraftableData _craftable;
        private int _materialID;

        public EditCraftingMaterialForm(CraftableData craftable, int materialID)
        {
            InitializeComponent();
            _craftable = craftable;
            _materialID = materialID;

            List<string> items = ItemData.GetItemNames();
            ItemSelection.Items.Add("None");
            ItemSelection.Items.AddRange(items.ToArray());

            Tuple<int, int> material = craftable.Materials[materialID];

            ItemSelection.SelectedIndex = material.Item1 + 1;
            ItemCount.Value = material.Item2;
        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            Tuple<int, int> material = new Tuple<int, int>(ItemSelection.SelectedIndex - 1, (int)ItemCount.Value);
            _craftable.Materials[_materialID] = material;
            this.Close();
        }
    }
}
