namespace RpgEditor.CommandDataPresets
{
    partial class ChangeGoldPreset
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
            this.GoldControl = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.GoldControl)).BeginInit();
            this.SuspendLayout();
            // 
            // GoldControl
            // 
            this.GoldControl.Location = new System.Drawing.Point(49, 3);
            this.GoldControl.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.GoldControl.Name = "GoldControl";
            this.GoldControl.Size = new System.Drawing.Size(120, 20);
            this.GoldControl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Gold:";
            // 
            // ChangeGoldPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.GoldControl);
            this.Name = "ChangeGoldPreset";
            this.Size = new System.Drawing.Size(172, 33);
            ((System.ComponentModel.ISupportInitialize)(this.GoldControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown GoldControl;
        private System.Windows.Forms.Label label1;
    }
}
