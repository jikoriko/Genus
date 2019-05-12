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
    public partial class EditDropItemForm : Form
    {
        private DropTableItem _dropTableItem;

        public EditDropItemForm(DropTableItem dropItem)
        {
            InitializeComponent();
            _dropTableItem = dropItem;

            List<string> items = ItemData.GetItemNames();
            ItemSelection.Items.Add("None");
            ItemSelection.Items.AddRange(items.ToArray());

            ItemSelection.SelectedIndex = dropItem.ItemID + 1;
            ItemCount.Value = dropItem.ItemCount;
            ItemChance.Value = dropItem.Chance;


        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            _dropTableItem.ItemID = ItemSelection.SelectedIndex - 1;
            _dropTableItem.ItemCount = (int)ItemCount.Value;
            _dropTableItem.Chance = (int)ItemChance.Value;
            this.Close();
        }
    }
}
