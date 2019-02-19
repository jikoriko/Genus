namespace RpgEditor
{
    partial class EditMapEventForm
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
            this.EventSelection = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.EventDirectionSelection = new System.Windows.Forms.ComboBox();
            this.EventSpriteSelection = new System.Windows.Forms.ComboBox();
            this.EventPassableCheck = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.EventTriggerTypeSelection = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.NameBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // EventSelection
            // 
            this.EventSelection.FormattingEnabled = true;
            this.EventSelection.Location = new System.Drawing.Point(109, 38);
            this.EventSelection.Name = "EventSelection";
            this.EventSelection.Size = new System.Drawing.Size(160, 21);
            this.EventSelection.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(257, 39);
            this.button1.TabIndex = 1;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // EventDirectionSelection
            // 
            this.EventDirectionSelection.FormattingEnabled = true;
            this.EventDirectionSelection.Items.AddRange(new object[] {
            "Down",
            "Left",
            "Right",
            "Up"});
            this.EventDirectionSelection.Location = new System.Drawing.Point(109, 65);
            this.EventDirectionSelection.Name = "EventDirectionSelection";
            this.EventDirectionSelection.Size = new System.Drawing.Size(160, 21);
            this.EventDirectionSelection.TabIndex = 2;
            // 
            // EventSpriteSelection
            // 
            this.EventSpriteSelection.FormattingEnabled = true;
            this.EventSpriteSelection.Location = new System.Drawing.Point(109, 92);
            this.EventSpriteSelection.Name = "EventSpriteSelection";
            this.EventSpriteSelection.Size = new System.Drawing.Size(160, 21);
            this.EventSpriteSelection.TabIndex = 3;
            // 
            // EventPassableCheck
            // 
            this.EventPassableCheck.AutoSize = true;
            this.EventPassableCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EventPassableCheck.Location = new System.Drawing.Point(200, 146);
            this.EventPassableCheck.Name = "EventPassableCheck";
            this.EventPassableCheck.Size = new System.Drawing.Size(69, 17);
            this.EventPassableCheck.TabIndex = 4;
            this.EventPassableCheck.Text = "Passable";
            this.EventPassableCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Event Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Event Direction:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(66, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Sprite:";
            // 
            // EventTriggerTypeSelection
            // 
            this.EventTriggerTypeSelection.FormattingEnabled = true;
            this.EventTriggerTypeSelection.Items.AddRange(new object[] {
            "None",
            "Action",
            "Player Touch",
            "Event Touch",
            "Autorun"});
            this.EventTriggerTypeSelection.Location = new System.Drawing.Point(109, 119);
            this.EventTriggerTypeSelection.Name = "EventTriggerTypeSelection";
            this.EventTriggerTypeSelection.Size = new System.Drawing.Size(160, 21);
            this.EventTriggerTypeSelection.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Trigger Type:";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(109, 12);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(160, 20);
            this.NameBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(65, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Name:";
            // 
            // EditMapEventForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 217);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.NameBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.EventTriggerTypeSelection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventPassableCheck);
            this.Controls.Add(this.EventSpriteSelection);
            this.Controls.Add(this.EventDirectionSelection);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.EventSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EditMapEventForm";
            this.Text = "Edit Map Event";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox EventSelection;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox EventDirectionSelection;
        private System.Windows.Forms.ComboBox EventSpriteSelection;
        private System.Windows.Forms.CheckBox EventPassableCheck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox EventTriggerTypeSelection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox NameBox;
        private System.Windows.Forms.Label label5;
    }
}