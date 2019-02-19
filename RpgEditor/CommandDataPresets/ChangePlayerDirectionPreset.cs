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
    public partial class ChangePlayerDirectionPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ChangePlayerDirectionPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();

            _command = command;

            string[] directions = new string[4];
            for (int i = 0; i < 4; i++)
            {
                directions[i] = ((Genus2D.GameData.FacingDirection)i).ToString();
            }
            this.DirectionSelection.Items.AddRange(directions);
            this.DirectionSelection.SelectedIndex = (int)command.GetParameter("Direction");
        }

        public void ApplyData()
        {
            _command.SetParameter("Direction", (Genus2D.GameData.FacingDirection)DirectionSelection.SelectedIndex);
        }
    }
}
