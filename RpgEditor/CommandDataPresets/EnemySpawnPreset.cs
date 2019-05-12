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
    public partial class EnemySpawnPreset : UserControl, CommandDataInterface
    {

        private Genus2D.GameData.EventCommand _command;

        public EnemySpawnPreset(Genus2D.GameData.EventCommand command)
        {
            InitializeComponent();
            _command = command;

            List<string> enemies = Genus2D.GameData.EnemyData.GetEnemyNames();
            EnemySelection.Items.AddRange(enemies.ToArray());
            EnemySelection.SelectedIndex = (int)command.GetParameter("EnemyID");
            EnemyCount.Value = (int)command.GetParameter("Count");
            RespawnTime.Value = (decimal)((float)command.GetParameter("RespawnTime"));
            SpawnRadius.Value = (int)command.GetParameter("SpawnRadius");
        }

        public void ApplyData()
        {
            if (EnemySelection.SelectedIndex > -1)
            {
                _command.SetParameter("EnemyID", EnemySelection.SelectedIndex);
                _command.SetParameter("Count", (int)EnemyCount.Value);
                _command.SetParameter("RespawnTime", (float)RespawnTime.Value);
                _command.SetParameter("SpawnRadius", (int)SpawnRadius.Value);
            }
            else
            {
                MessageBox.Show("Please select an enemy to spawn.");
            }
        }
    }
}
