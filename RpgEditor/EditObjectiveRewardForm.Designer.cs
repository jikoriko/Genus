namespace RpgEditor
{
    partial class EditObjectiveRewardForm
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
            this.ItemCount = new System.Windows.Forms.NumericUpDown();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ItemSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ItemCount)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemCount
            // 
            this.ItemCount.Location = new System.Drawing.Point(49, 34);
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
            this.ItemCount.Size = new System.Drawing.Size(122, 20);
            this.ItemCount.TabIndex = 2;
            this.ItemCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(5, 60);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(167, 30);
            this.ApplyButton.TabIndex = 4;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 36);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Count:";
            // 
            // ItemSelection
            // 
            this.ItemSelection.FormattingEnabled = true;
            this.ItemSelection.Location = new System.Drawing.Point(49, 7);
            this.ItemSelection.Name = "ItemSelection";
            this.ItemSelection.Size = new System.Drawing.Size(121, 21);
            this.ItemSelection.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Item:";
            // 
            // EditObjectiveRewardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 98);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ItemSelection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.ItemCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditObjectiveRewardForm";
            this.Text = "Edit Reward";
            ((System.ComponentModel.ISupportInitialize)(this.ItemCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown ItemCount;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox ItemSelection;
        private System.Windows.Forms.Label label1;
    }
}