namespace RpgEditor.CommandDataPresets
{
    partial class ChangeInventoryItemPreset
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
            this.ItemSelection = new System.Windows.Forms.ComboBox();
            this.ItemAmount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ItemAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item:";
            // 
            // ItemSelection
            // 
            this.ItemSelection.FormattingEnabled = true;
            this.ItemSelection.Location = new System.Drawing.Point(58, 3);
            this.ItemSelection.Name = "ItemSelection";
            this.ItemSelection.Size = new System.Drawing.Size(121, 21);
            this.ItemSelection.TabIndex = 1;
            // 
            // ItemAmount
            // 
            this.ItemAmount.Location = new System.Drawing.Point(59, 30);
            this.ItemAmount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.ItemAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ItemAmount.Name = "ItemAmount";
            this.ItemAmount.Size = new System.Drawing.Size(120, 20);
            this.ItemAmount.TabIndex = 2;
            this.ItemAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Amount:";
            // 
            // ChangeInventoryItemPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ItemAmount);
            this.Controls.Add(this.ItemSelection);
            this.Controls.Add(this.label1);
            this.Name = "ChangeInventoryItemPreset";
            this.Size = new System.Drawing.Size(191, 64);
            ((System.ComponentModel.ISupportInitialize)(this.ItemAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ItemSelection;
        private System.Windows.Forms.NumericUpDown ItemAmount;
        private System.Windows.Forms.Label label2;
    }
}
