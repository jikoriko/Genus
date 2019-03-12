using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgEditor.ItemDataPresets
{
    public partial class AmmoPreset : UserControl
    {
        public AmmoPreset()
        {
            InitializeComponent();
        }

        public int GetStrengthBonus()
        {
            return (int)StrengthBonus.Value;
        }

        public void SetStrengthBonus(int value)
        {
            StrengthBonus.Value = value;
        }
    }
}
