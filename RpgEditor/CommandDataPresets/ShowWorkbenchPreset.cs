using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Genus2D.GameData;

namespace RpgEditor.CommandDataPresets
{
    public partial class ShowWorkbenchPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ShowWorkbenchPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            for (int i = 0; i < CraftableData.GetWorkbenchDataCount(); i++)
            {
                WorkbenchSelectionBox.Items.Add(CraftableData.GetWorkbench(i));
            }

            int workbenchID = (int)command.GetParameter("WorkbenchID");
            if (workbenchID < WorkbenchSelectionBox.Items.Count)
                WorkbenchSelectionBox.SelectedIndex = workbenchID;

        }

        public void ApplyData()
        {
            _command.SetParameter("WorkbenchID", WorkbenchSelectionBox.SelectedIndex);
        }
    }
}
