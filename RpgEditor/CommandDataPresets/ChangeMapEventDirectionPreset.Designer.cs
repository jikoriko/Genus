namespace RpgEditor.CommandDataPresets
{
    partial class ChangeMapEventDirection
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
            this.MapSelection = new System.Windows.Forms.ComboBox();
            this.EventSelection = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DirectionSelection = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // MapSelection
            // 
            this.MapSelection.FormattingEnabled = true;
            this.MapSelection.Location = new System.Drawing.Point(74, 3);
            this.MapSelection.Name = "MapSelection";
            this.MapSelection.Size = new System.Drawing.Size(121, 21);
            this.MapSelection.TabIndex = 0;
            this.MapSelection.SelectedIndexChanged += new System.EventHandler(this.MapSelection_SelectedIndexChanged);
            // 
            // EventSelection
            // 
            this.EventSelection.FormattingEnabled = true;
            this.EventSelection.Location = new System.Drawing.Point(74, 30);
            this.EventSelection.Name = "EventSelection";
            this.EventSelection.Size = new System.Drawing.Size(121, 21);
            this.EventSelection.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Map:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Map Event:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Direction:";
            // 
            // DirectionSelection
            // 
            this.DirectionSelection.FormattingEnabled = true;
            this.DirectionSelection.Location = new System.Drawing.Point(74, 56);
            this.DirectionSelection.Name = "DirectionSelection";
            this.DirectionSelection.Size = new System.Drawing.Size(121, 21);
            this.DirectionSelection.TabIndex = 8;
            // 
            // MoveMapEventPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DirectionSelection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventSelection);
            this.Controls.Add(this.MapSelection);
            this.Name = "MoveMapEventPreset";
            this.Size = new System.Drawing.Size(209, 90);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox MapSelection;
        private System.Windows.Forms.ComboBox EventSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DirectionSelection;
    }
}
