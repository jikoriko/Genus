namespace RpgEditor.ItemDataPresets
{
    partial class EquipmentPreset
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.VitalityBonus = new System.Windows.Forms.NumericUpDown();
            this.EquipmentSlotSelection = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AttackStyleSelection = new System.Windows.Forms.ComboBox();
            this.InteligenceBonus = new System.Windows.Forms.NumericUpDown();
            this.StrengthBonus = new System.Windows.Forms.NumericUpDown();
            this.AgilityBonus = new System.Windows.Forms.NumericUpDown();
            this.MeleeDefenceBonus = new System.Windows.Forms.NumericUpDown();
            this.RangeDefenceBonus = new System.Windows.Forms.NumericUpDown();
            this.MagicDefenceBonus = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.ProjectileSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.MpDrain = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.VitalityBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InteligenceBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StrengthBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AgilityBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeleeDefenceBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RangeDefenceBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicDefenceBonus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MpDrain)).BeginInit();
            this.SuspendLayout();
            // 
            // VitalityBonus
            // 
            this.VitalityBonus.Location = new System.Drawing.Point(131, 57);
            this.VitalityBonus.Name = "VitalityBonus";
            this.VitalityBonus.Size = new System.Drawing.Size(84, 20);
            this.VitalityBonus.TabIndex = 1;
            // 
            // EquipmentSlotSelection
            // 
            this.EquipmentSlotSelection.FormattingEnabled = true;
            this.EquipmentSlotSelection.Items.AddRange(new object[] {
            "Weapon",
            "Shield",
            "Head",
            "Body",
            "Legs",
            "Feet",
            "Necklace",
            "Bracelet",
            "Ring"});
            this.EquipmentSlotSelection.Location = new System.Drawing.Point(130, 3);
            this.EquipmentSlotSelection.Name = "EquipmentSlotSelection";
            this.EquipmentSlotSelection.Size = new System.Drawing.Size(85, 21);
            this.EquipmentSlotSelection.TabIndex = 2;
            this.EquipmentSlotSelection.SelectedIndexChanged += new System.EventHandler(this.EquipmentSlotSelection_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Equipment Slot:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Attack Style:";
            // 
            // AttackStyleSelection
            // 
            this.AttackStyleSelection.FormattingEnabled = true;
            this.AttackStyleSelection.Items.AddRange(new object[] {
            "None",
            "Melee",
            "Range",
            "Magic"});
            this.AttackStyleSelection.Location = new System.Drawing.Point(130, 30);
            this.AttackStyleSelection.Name = "AttackStyleSelection";
            this.AttackStyleSelection.Size = new System.Drawing.Size(85, 21);
            this.AttackStyleSelection.TabIndex = 4;
            this.AttackStyleSelection.SelectedIndexChanged += new System.EventHandler(this.AttackStyleSelection_SelectedIndexChanged);
            // 
            // InteligenceBonus
            // 
            this.InteligenceBonus.Location = new System.Drawing.Point(131, 83);
            this.InteligenceBonus.Name = "InteligenceBonus";
            this.InteligenceBonus.Size = new System.Drawing.Size(84, 20);
            this.InteligenceBonus.TabIndex = 7;
            // 
            // StrengthBonus
            // 
            this.StrengthBonus.Location = new System.Drawing.Point(131, 109);
            this.StrengthBonus.Name = "StrengthBonus";
            this.StrengthBonus.Size = new System.Drawing.Size(84, 20);
            this.StrengthBonus.TabIndex = 8;
            // 
            // AgilityBonus
            // 
            this.AgilityBonus.Location = new System.Drawing.Point(131, 135);
            this.AgilityBonus.Name = "AgilityBonus";
            this.AgilityBonus.Size = new System.Drawing.Size(84, 20);
            this.AgilityBonus.TabIndex = 9;
            // 
            // MeleeDefenceBonus
            // 
            this.MeleeDefenceBonus.Location = new System.Drawing.Point(131, 161);
            this.MeleeDefenceBonus.Name = "MeleeDefenceBonus";
            this.MeleeDefenceBonus.Size = new System.Drawing.Size(84, 20);
            this.MeleeDefenceBonus.TabIndex = 10;
            // 
            // RangeDefenceBonus
            // 
            this.RangeDefenceBonus.Location = new System.Drawing.Point(131, 187);
            this.RangeDefenceBonus.Name = "RangeDefenceBonus";
            this.RangeDefenceBonus.Size = new System.Drawing.Size(84, 20);
            this.RangeDefenceBonus.TabIndex = 11;
            // 
            // MagicDefenceBonus
            // 
            this.MagicDefenceBonus.Location = new System.Drawing.Point(131, 213);
            this.MagicDefenceBonus.Name = "MagicDefenceBonus";
            this.MagicDefenceBonus.Size = new System.Drawing.Size(84, 20);
            this.MagicDefenceBonus.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Vitality Bonus:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Intelligence Bonus:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(42, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Strength Bonus:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(55, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Agility Bonus:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 163);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(116, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Melee Defence Bonus:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 189);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Range Defence Bonus:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 215);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Magic Defence Bonus:";
            // 
            // ProjectileSelection
            // 
            this.ProjectileSelection.FormattingEnabled = true;
            this.ProjectileSelection.Location = new System.Drawing.Point(130, 239);
            this.ProjectileSelection.Name = "ProjectileSelection";
            this.ProjectileSelection.Size = new System.Drawing.Size(85, 21);
            this.ProjectileSelection.TabIndex = 20;
            this.ProjectileSelection.SelectedIndexChanged += new System.EventHandler(this.ProjectileSelection_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 242);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Projectile:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(70, 268);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.TabIndex = 23;
            this.label11.Text = "MP Drain:";
            // 
            // MpDrain
            // 
            this.MpDrain.Location = new System.Drawing.Point(131, 266);
            this.MpDrain.Name = "MpDrain";
            this.MpDrain.Size = new System.Drawing.Size(84, 20);
            this.MpDrain.TabIndex = 22;
            this.MpDrain.ValueChanged += new System.EventHandler(this.MpDrain_ValueChanged);
            // 
            // EquipmentPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label11);
            this.Controls.Add(this.MpDrain);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ProjectileSelection);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MagicDefenceBonus);
            this.Controls.Add(this.RangeDefenceBonus);
            this.Controls.Add(this.MeleeDefenceBonus);
            this.Controls.Add(this.AgilityBonus);
            this.Controls.Add(this.StrengthBonus);
            this.Controls.Add(this.InteligenceBonus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AttackStyleSelection);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EquipmentSlotSelection);
            this.Controls.Add(this.VitalityBonus);
            this.Name = "EquipmentPreset";
            this.Size = new System.Drawing.Size(218, 310);
            ((System.ComponentModel.ISupportInitialize)(this.VitalityBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InteligenceBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StrengthBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AgilityBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MeleeDefenceBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RangeDefenceBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MagicDefenceBonus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MpDrain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown VitalityBonus;
        private System.Windows.Forms.ComboBox EquipmentSlotSelection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox AttackStyleSelection;
        private System.Windows.Forms.NumericUpDown InteligenceBonus;
        private System.Windows.Forms.NumericUpDown StrengthBonus;
        private System.Windows.Forms.NumericUpDown AgilityBonus;
        private System.Windows.Forms.NumericUpDown MeleeDefenceBonus;
        private System.Windows.Forms.NumericUpDown RangeDefenceBonus;
        private System.Windows.Forms.NumericUpDown MagicDefenceBonus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox ProjectileSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown MpDrain;
    }
}
