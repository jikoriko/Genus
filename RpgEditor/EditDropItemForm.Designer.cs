namespace RpgEditor
{
    partial class EditDropItemForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ItemSelection = new System.Windows.Forms.ComboBox();
            this.ItemCount = new System.Windows.Forms.NumericUpDown();
            this.ItemChance = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ApplyChangesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemChance)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemSelection
            // 
            this.ItemSelection.FormattingEnabled = true;
            this.ItemSelection.Location = new System.Drawing.Point(111, 10);
            this.ItemSelection.Name = "ItemSelection";
            this.ItemSelection.Size = new System.Drawing.Size(187, 21);
            this.ItemSelection.TabIndex = 0;
            // 
            // ItemCount
            // 
            this.ItemCount.Location = new System.Drawing.Point(111, 37);
            this.ItemCount.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.ItemCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ItemCount.Name = "ItemCount";
            this.ItemCount.Size = new System.Drawing.Size(187, 20);
            this.ItemCount.TabIndex = 1;
            this.ItemCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ItemChance
            // 
            this.ItemChance.Location = new System.Drawing.Point(111, 63);
            this.ItemChance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ItemChance.Name = "ItemChance";
            this.ItemChance.Size = new System.Drawing.Size(187, 20);
            this.ItemChance.TabIndex = 2;
            this.ItemChance.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Item:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Count:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Chance (1/100):";
            // 
            // ApplyChangesButton
            // 
            this.ApplyChangesButton.Location = new System.Drawing.Point(12, 89);
            this.ApplyChangesButton.Name = "ApplyChangesButton";
            this.ApplyChangesButton.Size = new System.Drawing.Size(286, 47);
            this.ApplyChangesButton.TabIndex = 6;
            this.ApplyChangesButton.Text = "Apply Changes";
            this.ApplyChangesButton.UseVisualStyleBackColor = true;
            this.ApplyChangesButton.Click += new System.EventHandler(this.ApplyChangesButton_Click);
            // 
            // EditDropItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 147);
            this.Controls.Add(this.ApplyChangesButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ItemChance);
            this.Controls.Add(this.ItemCount);
            this.Controls.Add(this.ItemSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditDropItemForm";
            this.Text = "Edit Drop Item";
            ((System.ComponentModel.ISupportInitialize)(this.ItemCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemChance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox ItemSelection;
        private System.Windows.Forms.NumericUpDown ItemCount;
        private System.Windows.Forms.NumericUpDown ItemChance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ApplyChangesButton;
    }
}