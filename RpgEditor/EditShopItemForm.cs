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
using static Genus2D.GameData.ShopData;

namespace RpgEditor
{
    public partial class EditShopItemForm : Form
    {
        private ShopItem _shopItem;

        public EditShopItemForm(ShopItem shopItem)
        {
            InitializeComponent();
            _shopItem = shopItem;

            List<string> items = ItemData.GetItemNames();
            ItemSelection.Items.Add("None");
            ItemSelection.Items.AddRange(items.ToArray());

            ItemSelection.SelectedIndex = shopItem.ItemID + 1;
            ItemCost.Value = shopItem.Cost;


        }

        private void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            _shopItem.ItemID = ItemSelection.SelectedIndex - 1;
            _shopItem.Cost = (int)ItemCost.Value;
            this.Close();
        }
    }
}
