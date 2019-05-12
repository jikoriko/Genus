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
    public partial class ChangeGoldPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangeGoldPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            GoldControl.Value = (int)command.GetParameter("Gold");
        }

        public void ApplyData()
        {
            _command.SetParameter("Gold", (int)GoldControl.Value);
        }
    }
}
