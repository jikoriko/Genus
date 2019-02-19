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
    public partial class MovePlayerPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public MovePlayerPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            string[] directions = new string[8];
            for (int i = 0; i < 8; i++)
            {
                directions[i] = ((Genus2D.GameData.MovementDirection)i).ToString();
            }
            this.DirectionSelection.Items.AddRange(directions);
            this.DirectionSelection.SelectedIndex = (int)command.GetParameter("Direction");
        }

        public void ApplyData()
        {
            _command.SetParameter("Direction", (Genus2D.GameData.MovementDirection)DirectionSelection.SelectedIndex);
        }
    }
}
