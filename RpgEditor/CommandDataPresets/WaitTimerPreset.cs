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
    public partial class WaitTimerPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public WaitTimerPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            TimerControl.Value = (decimal)((float)command.GetParameter("Time"));
        }

        public void ApplyData()
        {
            _command.SetParameter("Time", (float)TimerControl.Value);
        }
    }
}
