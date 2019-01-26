using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor
{
    public partial class AddEventCommandForm : Form
    {
        public AddEventCommandForm()
        {
            InitializeComponent();
            int numCommands = (int)Enum.GetValues(typeof(Genus2D.GameData.EventCommand.CommandType)).Cast<Genus2D.GameData.EventCommand.CommandType>().Last() + 1;
            for (int i = 0; i < numCommands; i++)
            {
                CommandSelection.Items.Add(((Genus2D.GameData.EventCommand.CommandType)i).ToString());
            }
        }

        private void AddCommandButton_Click(object sender, EventArgs e)
        {
            int selectedCommand = CommandSelection.SelectedIndex;
            if (selectedCommand != -1)
            {
                Genus2D.GameData.EventCommand.CommandType command = (Genus2D.GameData.EventCommand.CommandType)selectedCommand;
                EditorForm.Instance.AddEventCommand(command);
                this.Close();
            }
        }
    }
}
