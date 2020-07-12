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
    public partial class EditObjectiveRewardForm : Form
    {

        private Genus2D.GameData.QuestData.QuestObective _objective;
        private int _rewardID;

        public EditObjectiveRewardForm(Genus2D.GameData.QuestData.QuestObective objective, int rewardID)
        {
            InitializeComponent();
            _objective = objective;
            _rewardID = rewardID;

            List<string> items = Genus2D.GameData.ItemData.GetItemNames();
            ItemSelection.Items.Add("None");
            ItemSelection.Items.AddRange(items.ToArray());
            ItemSelection.SelectedIndex = objective.ItemRewards[rewardID].Item1 + 1;
            ItemCount.Value = objective.ItemRewards[rewardID].Item2;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            _objective.ItemRewards[_rewardID] = new Tuple<int, int>(ItemSelection.SelectedIndex - 1, (int)ItemCount.Value);
            this.Close();
        }
    }
}
