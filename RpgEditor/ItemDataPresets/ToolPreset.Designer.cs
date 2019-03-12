namespace RpgEditor.ItemDataPresets
{
    partial class ToolPreset
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
            this.ToolTypeSelection = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tool Type:";
            // 
            // ToolTypeSelection
            // 
            this.ToolTypeSelection.FormattingEnabled = true;
            this.ToolTypeSelection.Items.AddRange(new object[] {
            "Axe",
            "Pickaxe",
            "Shovel"});
            this.ToolTypeSelection.Location = new System.Drawing.Point(72, 3);
            this.ToolTypeSelection.Name = "ToolTypeSelection";
            this.ToolTypeSelection.Size = new System.Drawing.Size(121, 21);
            this.ToolTypeSelection.TabIndex = 1;
            // 
            // ToolTypePreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolTypeSelection);
            this.Controls.Add(this.label1);
            this.Name = "ToolTypePreset";
            this.Size = new System.Drawing.Size(202, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ToolTypeSelection;
    }
}
