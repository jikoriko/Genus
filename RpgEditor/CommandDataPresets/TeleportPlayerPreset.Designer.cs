namespace RpgEditor.CommandDataPresets
{
    partial class TeleportPlayerPreset
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
            this.MapSelection = new System.Windows.Forms.ComboBox();
            this.MapX = new System.Windows.Forms.NumericUpDown();
            this.MapY = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.MapX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapY)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Map:";
            // 
            // MapSelection
            // 
            this.MapSelection.FormattingEnabled = true;
            this.MapSelection.Location = new System.Drawing.Point(51, 3);
            this.MapSelection.Name = "MapSelection";
            this.MapSelection.Size = new System.Drawing.Size(121, 21);
            this.MapSelection.TabIndex = 1;
            this.MapSelection.SelectedIndexChanged += new System.EventHandler(this.MapSelection_SelectedIndexChanged);
            // 
            // MapX
            // 
            this.MapX.Location = new System.Drawing.Point(52, 30);
            this.MapX.Name = "MapX";
            this.MapX.Size = new System.Drawing.Size(120, 20);
            this.MapX.TabIndex = 2;
            // 
            // MapY
            // 
            this.MapY.Location = new System.Drawing.Point(51, 56);
            this.MapY.Name = "MapY";
            this.MapY.Size = new System.Drawing.Size(120, 20);
            this.MapY.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Map X:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Map Y:";
            // 
            // TeleportPlayerPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MapY);
            this.Controls.Add(this.MapX);
            this.Controls.Add(this.MapSelection);
            this.Controls.Add(this.label1);
            this.Name = "TeleportPlayerPreset";
            this.Size = new System.Drawing.Size(176, 84);
            ((System.ComponentModel.ISupportInitialize)(this.MapX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MapSelection;
        private System.Windows.Forms.NumericUpDown MapX;
        private System.Windows.Forms.NumericUpDown MapY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
