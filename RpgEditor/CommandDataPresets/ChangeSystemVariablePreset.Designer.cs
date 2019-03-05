namespace RpgEditor.CommandDataPresets
{
    partial class ChangeSystemVariablePreset
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
            this.SystemVariables = new System.Windows.Forms.ComboBox();
            this.VariableType = new System.Windows.Forms.ComboBox();
            this.ValueBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.RandomIntCheck = new System.Windows.Forms.CheckBox();
            this.RandomFloatCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RandomMin = new System.Windows.Forms.NumericUpDown();
            this.RandomMax = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RandomMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomMax)).BeginInit();
            this.SuspendLayout();
            // 
            // SystemVariables
            // 
            this.SystemVariables.FormattingEnabled = true;
            this.SystemVariables.Location = new System.Drawing.Point(101, 3);
            this.SystemVariables.Name = "SystemVariables";
            this.SystemVariables.Size = new System.Drawing.Size(172, 21);
            this.SystemVariables.TabIndex = 0;
            // 
            // VariableType
            // 
            this.VariableType.FormattingEnabled = true;
            this.VariableType.Items.AddRange(new object[] {
            "Integer",
            "Float",
            "Bool",
            "Text"});
            this.VariableType.Location = new System.Drawing.Point(101, 30);
            this.VariableType.Name = "VariableType";
            this.VariableType.Size = new System.Drawing.Size(172, 21);
            this.VariableType.TabIndex = 1;
            // 
            // ValueBox
            // 
            this.ValueBox.Location = new System.Drawing.Point(101, 57);
            this.ValueBox.Name = "ValueBox";
            this.ValueBox.Size = new System.Drawing.Size(172, 20);
            this.ValueBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "System Variable:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Variable Type:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Variable Value:";
            // 
            // RandomIntCheck
            // 
            this.RandomIntCheck.AutoSize = true;
            this.RandomIntCheck.Location = new System.Drawing.Point(20, 92);
            this.RandomIntCheck.Name = "RandomIntCheck";
            this.RandomIntCheck.Size = new System.Drawing.Size(81, 17);
            this.RandomIntCheck.TabIndex = 6;
            this.RandomIntCheck.Text = "Random Int";
            this.RandomIntCheck.UseVisualStyleBackColor = true;
            this.RandomIntCheck.CheckedChanged += new System.EventHandler(this.RandomIntCheck_CheckedChanged);
            // 
            // RandomFloatCheck
            // 
            this.RandomFloatCheck.AutoSize = true;
            this.RandomFloatCheck.Location = new System.Drawing.Point(20, 115);
            this.RandomFloatCheck.Name = "RandomFloatCheck";
            this.RandomFloatCheck.Size = new System.Drawing.Size(89, 17);
            this.RandomFloatCheck.TabIndex = 7;
            this.RandomFloatCheck.Text = "Random float";
            this.RandomFloatCheck.UseVisualStyleBackColor = true;
            this.RandomFloatCheck.CheckedChanged += new System.EventHandler(this.RandomFloatCheck_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.RandomMax);
            this.groupBox1.Controls.Add(this.RandomMin);
            this.groupBox1.Location = new System.Drawing.Point(23, 138);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 78);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Random Value";
            // 
            // RandomMin
            // 
            this.RandomMin.DecimalPlaces = 3;
            this.RandomMin.Location = new System.Drawing.Point(124, 19);
            this.RandomMin.Name = "RandomMin";
            this.RandomMin.Size = new System.Drawing.Size(120, 20);
            this.RandomMin.TabIndex = 0;
            this.RandomMin.ValueChanged += new System.EventHandler(this.RandomMin_ValueChanged);
            // 
            // RandomMax
            // 
            this.RandomMax.DecimalPlaces = 3;
            this.RandomMax.Location = new System.Drawing.Point(124, 45);
            this.RandomMax.Name = "RandomMax";
            this.RandomMax.Size = new System.Drawing.Size(120, 20);
            this.RandomMax.TabIndex = 1;
            this.RandomMax.ValueChanged += new System.EventHandler(this.RandomMax_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(91, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Min:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Max:";
            // 
            // ChangeSystemVariablePreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RandomFloatCheck);
            this.Controls.Add(this.RandomIntCheck);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.VariableType);
            this.Controls.Add(this.SystemVariables);
            this.Name = "ChangeSystemVariablePreset";
            this.Size = new System.Drawing.Size(287, 237);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RandomMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RandomMax)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SystemVariables;
        private System.Windows.Forms.ComboBox VariableType;
        private System.Windows.Forms.TextBox ValueBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox RandomIntCheck;
        private System.Windows.Forms.CheckBox RandomFloatCheck;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown RandomMax;
        private System.Windows.Forms.NumericUpDown RandomMin;
    }
}
