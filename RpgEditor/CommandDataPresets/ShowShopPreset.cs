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
    public partial class ShowShopPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ShowShopPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> shops = Genus2D.GameData.ShopData.GetShopNames();
            ShopSelection.Items.AddRange(shops.ToArray());

            ShopSelection.SelectedIndex = (int)command.GetParameter("ShopID");
        }


        public void ApplyData()
        {
            _command.SetParameter("ShopID", ShopSelection.SelectedIndex);
        }
    }
}
