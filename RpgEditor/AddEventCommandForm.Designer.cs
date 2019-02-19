namespace RpgEditor
{
    partial class AddEventCommandForm
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
            this.CommandSelection = new System.Windows.Forms.ComboBox();
            this.AddCommandButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CommandSelection
            // 
            this.CommandSelection.FormattingEnabled = true;
            this.CommandSelection.Location = new System.Drawing.Point(12, 12);
            this.CommandSelection.Name = "CommandSelection";
            this.CommandSelection.Size = new System.Drawing.Size(160, 21);
            this.CommandSelection.TabIndex = 0;
            // 
            // AddCommandButton
            // 
            this.AddCommandButton.Location = new System.Drawing.Point(12, 102);
            this.AddCommandButton.Name = "AddCommandButton";
            this.AddCommandButton.Size = new System.Drawing.Size(160, 47);
            this.AddCommandButton.TabIndex = 1;
            this.AddCommandButton.Text = "Add Command";
            this.AddCommandButton.UseVisualStyleBackColor = true;
            this.AddCommandButton.Click += new System.EventHandler(this.AddCommandButton_Click);
            // 
            // AddEventCommandForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(184, 161);
            this.Controls.Add(this.AddCommandButton);
            this.Controls.Add(this.CommandSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddEventCommandForm";
            this.Text = "Add Command";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CommandSelection;
        private System.Windows.Forms.Button AddCommandButton;
    }
}