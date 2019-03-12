namespace RpgEditor.ItemDataPresets
{
    partial class ConsumablePreset
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
            this.label1 = new System.Windows.Forms.Label();
            this.HpHealControl = new System.Windows.Forms.NumericUpDown();
            this.MpHealControl = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.StaminaHealControl = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.HpHealControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MpHealControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaminaHealControl)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "HP Heal:";
            // 
            // HpHealControl
            // 
            this.HpHealControl.Location = new System.Drawing.Point(85, 3);
            this.HpHealControl.Name = "HpHealControl";
            this.HpHealControl.Size = new System.Drawing.Size(120, 20);
            this.HpHealControl.TabIndex = 1;
            // 
            // MpHealControl
            // 
            this.MpHealControl.Location = new System.Drawing.Point(85, 29);
            this.MpHealControl.Name = "MpHealControl";
            this.MpHealControl.Size = new System.Drawing.Size(120, 20);
            this.MpHealControl.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MP Heal:";
            // 
            // StaminaHealControl
            // 
            this.StaminaHealControl.Location = new System.Drawing.Point(85, 55);
            this.StaminaHealControl.Name = "StaminaHealControl";
            this.StaminaHealControl.Size = new System.Drawing.Size(120, 20);
            this.StaminaHealControl.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Stamina Heal:";
            // 
            // ConsumabePreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StaminaHealControl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MpHealControl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HpHealControl);
            this.Controls.Add(this.label1);
            this.Name = "ConsumabePreset";
            this.Size = new System.Drawing.Size(213, 86);
            ((System.ComponentModel.ISupportInitialize)(this.HpHealControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MpHealControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StaminaHealControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown HpHealControl;
        private System.Windows.Forms.NumericUpDown MpHealControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown StaminaHealControl;
        private System.Windows.Forms.Label label3;
    }
}
