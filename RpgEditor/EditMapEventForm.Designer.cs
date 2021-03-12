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
            this.label6 = new System.Windows.Forms.Label();
            this.RenderPrioritySelection = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SpeedSelection = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.FrequencySelection = new System.Windows.Forms.ComboBox();
            this.RandomMovementCheck = new System.Windows.Forms.CheckBox();
            this.EnabledCheck = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ParticleEmitterSelection = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // EventSelection
            // 
            this.EventSelection.FormattingEnabled = true;
            this.EventSelection.Location = new System.Drawing.Point(133, 38);
            this.EventSelection.Name = "EventSelection";
            this.EventSelection.Size = new System.Drawing.Size(160, 21);
            this.EventSelection.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 309);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(281, 39);
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
            this.EventDirectionSelection.Location = new System.Drawing.Point(133, 65);
            this.EventDirectionSelection.Name = "EventDirectionSelection";
            this.EventDirectionSelection.Size = new System.Drawing.Size(160, 21);
            this.EventDirectionSelection.TabIndex = 2;
            // 
            // EventSpriteSelection
            // 
            this.EventSpriteSelection.FormattingEnabled = true;
            this.EventSpriteSelection.Location = new System.Drawing.Point(133, 92);
            this.EventSpriteSelection.Name = "EventSpriteSelection";
            this.EventSpriteSelection.Size = new System.Drawing.Size(160, 21);
            this.EventSpriteSelection.TabIndex = 3;
            // 
            // EventPassableCheck
            // 
            this.EventPassableCheck.AutoSize = true;
            this.EventPassableCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EventPassableCheck.Location = new System.Drawing.Point(14, 286);
            this.EventPassableCheck.Name = "EventPassableCheck";
            this.EventPassableCheck.Size = new System.Drawing.Size(72, 17);
            this.EventPassableCheck.TabIndex = 4;
            this.EventPassableCheck.Text = "Passable:";
            this.EventPassableCheck.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Event Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Event Direction:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(90, 95);
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
            this.EventTriggerTypeSelection.Location = new System.Drawing.Point(133, 119);
            this.EventTriggerTypeSelection.Name = "EventTriggerTypeSelection";
            this.EventTriggerTypeSelection.Size = new System.Drawing.Size(160, 21);
            this.EventTriggerTypeSelection.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Trigger Type:";
            // 
            // NameBox
            // 
            this.NameBox.Location = new System.Drawing.Point(133, 12);
            this.NameBox.Name = "NameBox";
            this.NameBox.Size = new System.Drawing.Size(160, 20);
            this.NameBox.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(89, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Name:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(48, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Render Priority:";
            // 
            // RenderPrioritySelection
            // 
            this.RenderPrioritySelection.FormattingEnabled = true;
            this.RenderPrioritySelection.Items.AddRange(new object[] {
            "Below Player",
            "Above Player",
            "On Top"});
            this.RenderPrioritySelection.Location = new System.Drawing.Point(133, 146);
            this.RenderPrioritySelection.Name = "RenderPrioritySelection";
            this.RenderPrioritySelection.Size = new System.Drawing.Size(160, 21);
            this.RenderPrioritySelection.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 176);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(94, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Movement Speed:";
            // 
            // SpeedSelection
            // 
            this.SpeedSelection.FormattingEnabled = true;
            this.SpeedSelection.Items.AddRange(new object[] {
            "ExtraFast",
            "Fast",
            "Normal",
            "Slow",
            "ExtraSlow"});
            this.SpeedSelection.Location = new System.Drawing.Point(133, 173);
            this.SpeedSelection.Name = "SpeedSelection";
            this.SpeedSelection.Size = new System.Drawing.Size(160, 21);
            this.SpeedSelection.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 203);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(113, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Movement Frequency:";
            // 
            // FrequencySelection
            // 
            this.FrequencySelection.FormattingEnabled = true;
            this.FrequencySelection.Items.AddRange(new object[] {
            "Instant",
            "Fast",
            "Normal",
            "Low",
            "Very Low"});
            this.FrequencySelection.Location = new System.Drawing.Point(133, 200);
            this.FrequencySelection.Name = "FrequencySelection";
            this.FrequencySelection.Size = new System.Drawing.Size(160, 21);
            this.FrequencySelection.TabIndex = 16;
            // 
            // RandomMovementCheck
            // 
            this.RandomMovementCheck.AutoSize = true;
            this.RandomMovementCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RandomMovementCheck.Location = new System.Drawing.Point(95, 286);
            this.RandomMovementCheck.Name = "RandomMovementCheck";
            this.RandomMovementCheck.Size = new System.Drawing.Size(122, 17);
            this.RandomMovementCheck.TabIndex = 18;
            this.RandomMovementCheck.Text = "Random Movement:";
            this.RandomMovementCheck.UseVisualStyleBackColor = true;
            // 
            // EnabledCheck
            // 
            this.EnabledCheck.AutoSize = true;
            this.EnabledCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EnabledCheck.Location = new System.Drawing.Point(225, 286);
            this.EnabledCheck.Name = "EnabledCheck";
            this.EnabledCheck.Size = new System.Drawing.Size(68, 17);
            this.EnabledCheck.TabIndex = 19;
            this.EnabledCheck.Text = "Enabled:";
            this.EnabledCheck.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(47, 230);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Particle Emitter:";
            // 
            // ParticleEmitterSelection
            // 
            this.ParticleEmitterSelection.FormattingEnabled = true;
            this.ParticleEmitterSelection.Location = new System.Drawing.Point(133, 227);
            this.ParticleEmitterSelection.Name = "ParticleEmitterSelection";
            this.ParticleEmitterSelection.Size = new System.Drawing.Size(160, 21);
            this.ParticleEmitterSelection.TabIndex = 20;
            // 
            // EditMapEventForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 360);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ParticleEmitterSelection);
            this.Controls.Add(this.EnabledCheck);
            this.Controls.Add(this.RandomMovementCheck);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.FrequencySelection);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SpeedSelection);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.RenderPrioritySelection);
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
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox RenderPrioritySelection;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox SpeedSelection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox FrequencySelection;
        private System.Windows.Forms.CheckBox RandomMovementCheck;
        private System.Windows.Forms.CheckBox EnabledCheck;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox ParticleEmitterSelection;
    }
}