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

namespace RpgEditor.ItemDataPresets
{
    public partial class AmmoPreset : UserControl
    {
        public AmmoPreset()
        {
            InitializeComponent();

            ProjectileSelection.Items.Add("None");
            List<string> projectiles = ProjectileData.GetProjectileNames();
            ProjectileSelection.Items.AddRange(projectiles.ToArray());
            ProjectileSelection.SelectedIndex = 0;
        }

        public int GetStrengthBonus()
        {
            return (int)StrengthBonus.Value;
        }

        public void SetStrengthBonus(int value)
        {
            StrengthBonus.Value = value;
        }

        public int GetProjectileID()
        {
            return ProjectileSelection.SelectedIndex - 1;
        }

        public void SetProjectileID(int id)
        {
            ProjectileSelection.SelectedIndex = id + 1;
        }
    }
}
