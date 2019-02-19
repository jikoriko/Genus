namespace RpgEditor.CommandDataPresets
{
    partial class ShowOptionsPreset
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
            this.MessageTextBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.MessageOptions = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.RemoveOptionButton = new System.Windows.Forms.Button();
            this.OptionNameBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.OptionEvents = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AddOptionButton = new System.Windows.Forms.Button();
            this.ChangeOptionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MessageTextBox
            // 
            this.MessageTextBox.Location = new System.Drawing.Point(6, 29);
            this.MessageTextBox.Name = "MessageTextBox";
            this.MessageTextBox.Size = new System.Drawing.Size(443, 148);
            this.MessageTextBox.TabIndex = 0;
            this.MessageTextBox.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Message:";
            // 
            // MessageOptions
            // 
            this.MessageOptions.FormattingEnabled = true;
            this.MessageOptions.Location = new System.Drawing.Point(64, 183);
            this.MessageOptions.Name = "MessageOptions";
            this.MessageOptions.Size = new System.Drawing.Size(172, 21);
            this.MessageOptions.TabIndex = 2;
            this.MessageOptions.SelectedIndexChanged += new System.EventHandler(this.MessageOptions_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 186);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Options:";
            // 
            // RemoveOptionButton
            // 
            this.RemoveOptionButton.Location = new System.Drawing.Point(242, 183);
            this.RemoveOptionButton.Name = "RemoveOptionButton";
            this.RemoveOptionButton.Size = new System.Drawing.Size(98, 23);
            this.RemoveOptionButton.TabIndex = 4;
            this.RemoveOptionButton.Text = "Remove Option";
            this.RemoveOptionButton.UseVisualStyleBackColor = true;
            this.RemoveOptionButton.Click += new System.EventHandler(this.RemoveOptionButton_Click);
            // 
            // OptionNameBox
            // 
            this.OptionNameBox.Location = new System.Drawing.Point(90, 210);
            this.OptionNameBox.Name = "OptionNameBox";
            this.OptionNameBox.Size = new System.Drawing.Size(146, 20);
            this.OptionNameBox.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 213);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Option Name:";
            // 
            // OptionEvents
            // 
            this.OptionEvents.FormattingEnabled = true;
            this.OptionEvents.Location = new System.Drawing.Point(90, 236);
            this.OptionEvents.Name = "OptionEvents";
            this.OptionEvents.Size = new System.Drawing.Size(146, 21);
            this.OptionEvents.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Option Event:";
            // 
            // AddOptionButton
            // 
            this.AddOptionButton.Location = new System.Drawing.Point(346, 208);
            this.AddOptionButton.Name = "AddOptionButton";
            this.AddOptionButton.Size = new System.Drawing.Size(98, 23);
            this.AddOptionButton.TabIndex = 9;
            this.AddOptionButton.Text = "Add Option";
            this.AddOptionButton.UseVisualStyleBackColor = true;
            this.AddOptionButton.Click += new System.EventHandler(this.AddOptionButton_Click);
            // 
            // ChangeOptionButton
            // 
            this.ChangeOptionButton.Location = new System.Drawing.Point(242, 208);
            this.ChangeOptionButton.Name = "ChangeOptionButton";
            this.ChangeOptionButton.Size = new System.Drawing.Size(98, 23);
            this.ChangeOptionButton.TabIndex = 10;
            this.ChangeOptionButton.Text = "Change Name";
            this.ChangeOptionButton.UseVisualStyleBackColor = true;
            this.ChangeOptionButton.Click += new System.EventHandler(this.ChangeOptionButton_Click);
            // 
            // ShowOptionsPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ChangeOptionButton);
            this.Controls.Add(this.AddOptionButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.OptionEvents);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.OptionNameBox);
            this.Controls.Add(this.RemoveOptionButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MessageOptions);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MessageTextBox);
            this.Name = "ShowOptionsPreset";
            this.Size = new System.Drawing.Size(452, 429);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox MessageTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox MessageOptions;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RemoveOptionButton;
        private System.Windows.Forms.TextBox OptionNameBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox OptionEvents;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AddOptionButton;
        private System.Windows.Forms.Button ChangeOptionButton;
    }
}
