namespace RpgEditor.ItemDataPresets
{
    partial class MatarialPreset
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
            this.MaterialIdControl = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.MaterialIdControl)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Material ID:";
            // 
            // MaterialIdControl
            // 
            this.MaterialIdControl.Location = new System.Drawing.Point(85, 3);
            this.MaterialIdControl.Name = "MaterialIdControl";
            this.MaterialIdControl.Size = new System.Drawing.Size(120, 20);
            this.MaterialIdControl.TabIndex = 1;
            // 
            // MatarialPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MaterialIdControl);
            this.Controls.Add(this.label1);
            this.Name = "MatarialPreset";
            this.Size = new System.Drawing.Size(213, 32);
            ((System.ComponentModel.ISupportInitialize)(this.MaterialIdControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown MaterialIdControl;
    }
}
