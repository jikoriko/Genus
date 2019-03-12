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
    public partial class EquipmentPreset : UserControl
    {
        public EquipmentPreset()
        {
            InitializeComponent();
        }

        public EquipmentSlot GetEquipmentSlot()
        {
            return (EquipmentSlot)EquipmentSlotSelection.SelectedIndex;
        }

        public void SetEquipmentSlot(EquipmentSlot slot)
        {
            EquipmentSlotSelection.SelectedIndex = (int)slot;
        }

        public AttackStyle GetAttackStyle()
        {
            return (AttackStyle)AttackStyleSelection.SelectedIndex;
        }

        public void SetAttackStyle(AttackStyle style)
        {
            AttackStyleSelection.SelectedIndex = (int)style;
        }

        public int GetVitalityBonus()
        {
            return (int)VitalityBonus.Value;
        }

        public void SetVitalityBonus(int value)
        {
            VitalityBonus.Value = value;
        }

        public int GetInteligenceBonus()
        {
            return (int)InteligenceBonus.Value;
        }

        public void SetInteligenceBonus(int value)
        {
            InteligenceBonus.Value = value;
        }

        public int GetStrengthBonus()
        {
            return (int)StrengthBonus.Value;
        }

        public void SetStrengthBonus(int value)
        {
            StrengthBonus.Value = value;
        }

        public int GetAgilityBonus()
        {
            return (int)AgilityBonus.Value;
        }

        public void SetAgilityBonus(int value)
        {
            AgilityBonus.Value = value;
        }

        public int GetMeleeDefenceBonus()
        {
            return (int)MeleeDefenceBonus.Value;
        }

        public void SetMeleeDefenceBonus(int value)
        {
            MeleeDefenceBonus.Value = value;
        }

        public int GetRangeDefenceBonus()
        {
            return (int)RangeDefenceBonus.Value;
        }

        public void SetRangeDefenceBonus(int value)
        {
            RangeDefenceBonus.Value = value;
        }

        public int GetMagicDefenceBonus()
        {
            return (int)MagicDefenceBonus.Value;
        }

        public void SetMagicDefenceBonus(int value)
        {
            MagicDefenceBonus.Value = value;
        }

        private void AttackStyleSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EquipmentSlotSelection.SelectedIndex != 0 && AttackStyleSelection.SelectedIndex > 0)
                AttackStyleSelection.SelectedIndex = 0;

        }

        private void EquipmentSlotSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EquipmentSlotSelection.SelectedIndex == 0)
                AttackStyleSelection.SelectedIndex = 0;
        }
    }
}
