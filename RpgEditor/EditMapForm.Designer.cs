namespace RpgEditor
{
    partial class EditMapForm
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
            this.NameField = new System.Windows.Forms.TextBox();
            this.WidthField = new System.Windows.Forms.NumericUpDown();
            this.HeightField = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.PvpCheck = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.WidthField)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightField)).BeginInit();
            this.SuspendLayout();
            // 
            // NameField
            // 
            this.NameField.Location = new System.Drawing.Point(56, 6);
            this.NameField.Name = "NameField";
            this.NameField.Size = new System.Drawing.Size(123, 20);
            this.NameField.TabIndex = 0;
            // 
            // WidthField
            // 
            this.WidthField.Location = new System.Drawing.Point(56, 32);
            this.WidthField.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.WidthField.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.WidthField.Name = "WidthField";
            this.WidthField.Size = new System.Drawing.Size(122, 20);
            this.WidthField.TabIndex = 1;
            this.WidthField.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // HeightField
            // 
            this.HeightField.Location = new System.Drawing.Point(56, 59);
            this.HeightField.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.HeightField.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.HeightField.Name = "HeightField";
            this.HeightField.Size = new System.Drawing.Size(122, 20);
            this.HeightField.TabIndex = 2;
            this.HeightField.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 114);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(167, 30);
            this.button1.TabIndex = 4;
            this.button1.Text = "Edit Map";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Width:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Height:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(11, 150);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(167, 30);
            this.button2.TabIndex = 8;
            this.button2.Text = "Delete Map";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // PvpCheck
            // 
            this.PvpCheck.AutoSize = true;
            this.PvpCheck.Location = new System.Drawing.Point(56, 91);
            this.PvpCheck.Name = "PvpCheck";
            this.PvpCheck.Size = new System.Drawing.Size(89, 17);
            this.PvpCheck.TabIndex = 9;
            this.PvpCheck.Text = "PVP Enabled";
            this.PvpCheck.UseVisualStyleBackColor = true;
            // 
            // EditMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 192);
            this.Controls.Add(this.PvpCheck);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.HeightField);
            this.Controls.Add(this.WidthField);
            this.Controls.Add(this.NameField);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditMapForm";
            this.Text = "Edit Map";
            ((System.ComponentModel.ISupportInitialize)(this.WidthField)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightField)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NameField;
        private System.Windows.Forms.NumericUpDown WidthField;
        private System.Windows.Forms.NumericUpDown HeightField;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox PvpCheck;
    }
}