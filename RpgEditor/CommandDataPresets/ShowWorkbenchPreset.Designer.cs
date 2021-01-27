namespace RpgEditor.CommandDataPresets
{
    partial class ShowWorkbenchPreset
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
            this.WorkbenchSelectionBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Workbench:";
            // 
            // WorkbenchSelectionBox
            // 
            this.WorkbenchSelectionBox.FormattingEnabled = true;
            this.WorkbenchSelectionBox.Location = new System.Drawing.Point(83, 3);
            this.WorkbenchSelectionBox.Name = "WorkbenchSelectionBox";
            this.WorkbenchSelectionBox.Size = new System.Drawing.Size(121, 21);
            this.WorkbenchSelectionBox.TabIndex = 2;
            // 
            // ShowWorkbenchPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.WorkbenchSelectionBox);
            this.Controls.Add(this.label1);
            this.Name = "ShowWorkbenchPreset";
            this.Size = new System.Drawing.Size(211, 33);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox WorkbenchSelectionBox;
    }
}
