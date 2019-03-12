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
    public partial class ConsumablePreset : UserControl
    {
        public ConsumablePreset()
        {
            InitializeComponent();
        }

        public int GetHpHeal()
        {
            return (int)HpHealControl.Value;
        }

        public void SetHpHeal(int value)
        {
            HpHealControl.Value = value;
        }

        public int GetMpHeal()
        {
            return (int)MpHealControl.Value;
        }

        public void SetMpHeal(int value)
        {
            MpHealControl.Value = value;
        }

        public int GetStaminaHeal()
        {
            return (int)StaminaHealControl.Value;
        }

        public void SetStaminaHeal(int value)
        {
            StaminaHealControl.Value = value;
        }
    }
}
