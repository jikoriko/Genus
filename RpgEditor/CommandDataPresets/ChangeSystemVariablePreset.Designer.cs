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
            // ChangeSystemVariablePreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ValueBox);
            this.Controls.Add(this.VariableType);
            this.Controls.Add(this.SystemVariables);
            this.Name = "ChangeSystemVariablePreset";
            this.Size = new System.Drawing.Size(287, 88);
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
    }
}
