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
    public partial class ProgressQuestPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public ProgressQuestPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> quests = Genus2D.GameData.QuestData.GetQuestNames();
            QuestSelection.Items.AddRange(quests.ToArray());

            QuestSelection.SelectedIndex = (int)command.GetParameter("QuestID");
        }


        public void ApplyData()
        {
            _command.SetParameter("QuestID", QuestSelection.SelectedIndex);
        }
    }
}
