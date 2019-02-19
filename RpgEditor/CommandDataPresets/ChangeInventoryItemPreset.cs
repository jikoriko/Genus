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
    public partial class ChangeInventoryItemPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeInventoryItemPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> items = Genus2D.GameData.ItemData.GetItemNames();
            ItemSelection.Items.AddRange(items.ToArray());

            ItemSelection.SelectedIndex = (int)command.GetParameter("ItemID");
            ItemAmount.Value = (int)command.GetParameter("ItemAmount");
        }


        public void ApplyData()
        {
            _command.SetParameter("ItemID", ItemSelection.SelectedIndex);
            _command.SetParameter("ItemAmount", (int)ItemAmount.Value);
        }
    }
}
