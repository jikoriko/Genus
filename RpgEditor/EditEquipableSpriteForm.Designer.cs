namespace RpgEditor
{
    partial class EditEquipableSpriteForm
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
            this.AnchorXSelection = new System.Windows.Forms.NumericUpDown();
            this.AnchorYSelection = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SpriteSelection = new System.Windows.Forms.ComboBox();
            this.DirectionSelection = new System.Windows.Forms.ComboBox();
            this.FrameSelection = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SpriteViewerPanel = new System.Windows.Forms.Panel();
            this.ApplyButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorXSelection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorYSelection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrameSelection)).BeginInit();
            this.SuspendLayout();
            // 
            // AnchorXSelection
            // 
            this.AnchorXSelection.Location = new System.Drawing.Point(71, 135);
            this.AnchorXSelection.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.AnchorXSelection.Name = "AnchorXSelection";
            this.AnchorXSelection.Size = new System.Drawing.Size(122, 20);
            this.AnchorXSelection.TabIndex = 1;
            this.AnchorXSelection.ValueChanged += new System.EventHandler(this.AnchorXSelection_ValueChanged);
            // 
            // AnchorYSelection
            // 
            this.AnchorYSelection.Location = new System.Drawing.Point(71, 162);
            this.AnchorYSelection.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.AnchorYSelection.Name = "AnchorYSelection";
            this.AnchorYSelection.Size = new System.Drawing.Size(122, 20);
            this.AnchorYSelection.TabIndex = 2;
            this.AnchorYSelection.ValueChanged += new System.EventHandler(this.AnchorYSelection_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Anchor X:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Anchor Y:";
            // 
            // SpriteSelection
            // 
            this.SpriteSelection.FormattingEnabled = true;
            this.SpriteSelection.Location = new System.Drawing.Point(71, 12);
            this.SpriteSelection.Name = "SpriteSelection";
            this.SpriteSelection.Size = new System.Drawing.Size(121, 21);
            this.SpriteSelection.TabIndex = 8;
            this.SpriteSelection.SelectedIndexChanged += new System.EventHandler(this.SpriteSelection_SelectedIndexChanged);
            // 
            // DirectionSelection
            // 
            this.DirectionSelection.FormattingEnabled = true;
            this.DirectionSelection.Location = new System.Drawing.Point(71, 59);
            this.DirectionSelection.Name = "DirectionSelection";
            this.DirectionSelection.Size = new System.Drawing.Size(121, 21);
            this.DirectionSelection.TabIndex = 9;
            this.DirectionSelection.SelectedIndexChanged += new System.EventHandler(this.DirectionSelection_SelectedIndexChanged);
            // 
            // FrameSelection
            // 
            this.FrameSelection.Location = new System.Drawing.Point(70, 86);
            this.FrameSelection.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.FrameSelection.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameSelection.Name = "FrameSelection";
            this.FrameSelection.Size = new System.Drawing.Size(122, 20);
            this.FrameSelection.TabIndex = 10;
            this.FrameSelection.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.FrameSelection.ValueChanged += new System.EventHandler(this.FrameSelection_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Frame:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Direction:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Sprite:";
            // 
            // SpriteViewerPanel
            // 
            this.SpriteViewerPanel.BackColor = System.Drawing.Color.White;
            this.SpriteViewerPanel.Location = new System.Drawing.Point(199, 12);
            this.SpriteViewerPanel.Name = "SpriteViewerPanel";
            this.SpriteViewerPanel.Size = new System.Drawing.Size(762, 519);
            this.SpriteViewerPanel.TabIndex = 14;
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(12, 188);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(181, 33);
            this.ApplyButton.TabIndex = 0;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // EditEquipableSpriteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(973, 543);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.SpriteViewerPanel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FrameSelection);
            this.Controls.Add(this.DirectionSelection);
            this.Controls.Add(this.SpriteSelection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AnchorYSelection);
            this.Controls.Add(this.AnchorXSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditEquipableSpriteForm";
            this.Text = "Edit Equipable Sprite";
            ((System.ComponentModel.ISupportInitialize)(this.AnchorXSelection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AnchorYSelection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FrameSelection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown AnchorXSelection;
        private System.Windows.Forms.NumericUpDown AnchorYSelection;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox SpriteSelection;
        private System.Windows.Forms.ComboBox DirectionSelection;
        private System.Windows.Forms.NumericUpDown FrameSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel SpriteViewerPanel;
        private System.Windows.Forms.Button ApplyButton;
    }
}