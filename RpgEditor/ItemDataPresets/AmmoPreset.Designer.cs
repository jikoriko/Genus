namespace RpgEditor.ItemDataPresets
{
    partial class AmmoPreset
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
            this.StrengthBonus = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.StrengthBonus)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Strength Bonus:";
            // 
            // StrengthBonus
            // 
            this.StrengthBonus.Location = new System.Drawing.Point(94, 3);
            this.StrengthBonus.Name = "StrengthBonus";
            this.StrengthBonus.Size = new System.Drawing.Size(120, 20);
            this.StrengthBonus.TabIndex = 1;
            // 
            // AmmoPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StrengthBonus);
            this.Controls.Add(this.label1);
            this.Name = "AmmoPreset";
            this.Size = new System.Drawing.Size(220, 32);
            ((System.ComponentModel.ISupportInitialize)(this.StrengthBonus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown StrengthBonus;
    }
}
