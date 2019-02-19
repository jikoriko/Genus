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
    public partial class ShowMessagePreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ShowMessagePreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            MessageBox.Text = (string)command.GetParameter("Message");

        }

        public void ApplyData()
        {
            _command.SetParameter("Message", MessageBox.Text);
        }
    }
}
