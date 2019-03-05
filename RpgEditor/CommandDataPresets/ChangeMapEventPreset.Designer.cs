namespace RpgEditor.CommandDataPresets
{
    partial class ChangeMapEventPreset
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
            this.DirectionSelection = new System.Windows.Forms.ComboBox();
            this.TeleportRadio = new System.Windows.Forms.RadioButton();
            this.MovementRadio = new System.Windows.Forms.RadioButton();
            this.DirectionRadio = new System.Windows.Forms.RadioButton();
            this.SpriteRadio = new System.Windows.Forms.RadioButton();
            this.RenderPriorityRadio = new System.Windows.Forms.RadioButton();
            this.MovementSpeedRadio = new System.Windows.Forms.RadioButton();
            this.MovementFrequencyRadio = new System.Windows.Forms.RadioButton();
            this.MovementSelection = new System.Windows.Forms.ComboBox();
            this.PassableCheck = new System.Windows.Forms.CheckBox();
            this.PassableRadio = new System.Windows.Forms.RadioButton();
            this.RandomMovementRadio = new System.Windows.Forms.RadioButton();
            this.EnabledRadio = new System.Windows.Forms.RadioButton();
            this.RandomMovementCheck = new System.Windows.Forms.CheckBox();
            this.EnabledCheck = new System.Windows.Forms.CheckBox();
            this.SpriteSelection = new System.Windows.Forms.ComboBox();
            this.RenderPrioritySelection = new System.Windows.Forms.ComboBox();
            this.MovementSpeedSelection = new System.Windows.Forms.ComboBox();
            this.MovementFrequencySelection = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.MapYSelection = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.MapXSelection = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapYSelection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapXSelection)).BeginInit();
            this.SuspendLayout();
            // 
            // MapSelection
            // 
            this.MapSelection.FormattingEnabled = true;
            this.MapSelection.Location = new System.Drawing.Point(124, 3);
            this.MapSelection.Name = "MapSelection";
            this.MapSelection.Size = new System.Drawing.Size(169, 21);
            this.MapSelection.TabIndex = 0;
            this.MapSelection.SelectedIndexChanged += new System.EventHandler(this.MapSelection_SelectedIndexChanged);
            // 
            // EventSelection
            // 
            this.EventSelection.FormattingEnabled = true;
            this.EventSelection.Location = new System.Drawing.Point(124, 30);
            this.EventSelection.Name = "EventSelection";
            this.EventSelection.Size = new System.Drawing.Size(169, 21);
            this.EventSelection.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Map:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Map Event:";
            // 
            // DirectionSelection
            // 
            this.DirectionSelection.FormattingEnabled = true;
            this.DirectionSelection.Items.AddRange(new object[] {
            "Down",
            "Left",
            "Right",
            "Up"});
            this.DirectionSelection.Location = new System.Drawing.Point(6, 19);
            this.DirectionSelection.Name = "DirectionSelection";
            this.DirectionSelection.Size = new System.Drawing.Size(174, 21);
            this.DirectionSelection.TabIndex = 8;
            // 
            // TeleportRadio
            // 
            this.TeleportRadio.AutoSize = true;
            this.TeleportRadio.Location = new System.Drawing.Point(12, 29);
            this.TeleportRadio.Name = "TeleportRadio";
            this.TeleportRadio.Size = new System.Drawing.Size(67, 17);
            this.TeleportRadio.TabIndex = 9;
            this.TeleportRadio.TabStop = true;
            this.TeleportRadio.Text = "Teleport:";
            this.TeleportRadio.UseVisualStyleBackColor = true;
            // 
            // MovementRadio
            // 
            this.MovementRadio.AutoSize = true;
            this.MovementRadio.Location = new System.Drawing.Point(12, 111);
            this.MovementRadio.Name = "MovementRadio";
            this.MovementRadio.Size = new System.Drawing.Size(123, 17);
            this.MovementRadio.TabIndex = 10;
            this.MovementRadio.TabStop = true;
            this.MovementRadio.Text = "Movement Direction:";
            this.MovementRadio.UseVisualStyleBackColor = true;
            // 
            // DirectionRadio
            // 
            this.DirectionRadio.AutoSize = true;
            this.DirectionRadio.Location = new System.Drawing.Point(13, 195);
            this.DirectionRadio.Name = "DirectionRadio";
            this.DirectionRadio.Size = new System.Drawing.Size(105, 17);
            this.DirectionRadio.TabIndex = 11;
            this.DirectionRadio.TabStop = true;
            this.DirectionRadio.Text = "Facing Direction:";
            this.DirectionRadio.UseVisualStyleBackColor = true;
            // 
            // SpriteRadio
            // 
            this.SpriteRadio.AutoSize = true;
            this.SpriteRadio.Location = new System.Drawing.Point(12, 279);
            this.SpriteRadio.Name = "SpriteRadio";
            this.SpriteRadio.Size = new System.Drawing.Size(55, 17);
            this.SpriteRadio.TabIndex = 12;
            this.SpriteRadio.TabStop = true;
            this.SpriteRadio.Text = "Sprite:";
            this.SpriteRadio.UseVisualStyleBackColor = true;
            // 
            // RenderPriorityRadio
            // 
            this.RenderPriorityRadio.AutoSize = true;
            this.RenderPriorityRadio.Location = new System.Drawing.Point(223, 111);
            this.RenderPriorityRadio.Name = "RenderPriorityRadio";
            this.RenderPriorityRadio.Size = new System.Drawing.Size(97, 17);
            this.RenderPriorityRadio.TabIndex = 13;
            this.RenderPriorityRadio.TabStop = true;
            this.RenderPriorityRadio.Text = "Render Priority:";
            this.RenderPriorityRadio.UseVisualStyleBackColor = true;
            // 
            // MovementSpeedRadio
            // 
            this.MovementSpeedRadio.AutoSize = true;
            this.MovementSpeedRadio.Location = new System.Drawing.Point(223, 195);
            this.MovementSpeedRadio.Name = "MovementSpeedRadio";
            this.MovementSpeedRadio.Size = new System.Drawing.Size(112, 17);
            this.MovementSpeedRadio.TabIndex = 14;
            this.MovementSpeedRadio.TabStop = true;
            this.MovementSpeedRadio.Text = "Movement Speed:";
            this.MovementSpeedRadio.UseVisualStyleBackColor = true;
            // 
            // MovementFrequencyRadio
            // 
            this.MovementFrequencyRadio.AutoSize = true;
            this.MovementFrequencyRadio.Location = new System.Drawing.Point(223, 279);
            this.MovementFrequencyRadio.Name = "MovementFrequencyRadio";
            this.MovementFrequencyRadio.Size = new System.Drawing.Size(131, 17);
            this.MovementFrequencyRadio.TabIndex = 15;
            this.MovementFrequencyRadio.TabStop = true;
            this.MovementFrequencyRadio.Text = "Movement Frequency:";
            this.MovementFrequencyRadio.UseVisualStyleBackColor = true;
            // 
            // MovementSelection
            // 
            this.MovementSelection.FormattingEnabled = true;
            this.MovementSelection.Items.AddRange(new object[] {
            "Down",
            "Left",
            "Right",
            "Up",
            "Upper Left",
            "Upper Right",
            "Lower Left",
            "Lower Right"});
            this.MovementSelection.Location = new System.Drawing.Point(6, 19);
            this.MovementSelection.Name = "MovementSelection";
            this.MovementSelection.Size = new System.Drawing.Size(174, 21);
            this.MovementSelection.TabIndex = 16;
            // 
            // PassableCheck
            // 
            this.PassableCheck.AutoSize = true;
            this.PassableCheck.Location = new System.Drawing.Point(89, 365);
            this.PassableCheck.Name = "PassableCheck";
            this.PassableCheck.Size = new System.Drawing.Size(15, 14);
            this.PassableCheck.TabIndex = 17;
            this.PassableCheck.UseVisualStyleBackColor = true;
            // 
            // PassableRadio
            // 
            this.PassableRadio.AutoSize = true;
            this.PassableRadio.Location = new System.Drawing.Point(12, 363);
            this.PassableRadio.Name = "PassableRadio";
            this.PassableRadio.Size = new System.Drawing.Size(71, 17);
            this.PassableRadio.TabIndex = 20;
            this.PassableRadio.TabStop = true;
            this.PassableRadio.Text = "Passable:";
            this.PassableRadio.UseVisualStyleBackColor = true;
            // 
            // RandomMovementRadio
            // 
            this.RandomMovementRadio.AutoSize = true;
            this.RandomMovementRadio.Location = new System.Drawing.Point(12, 386);
            this.RandomMovementRadio.Name = "RandomMovementRadio";
            this.RandomMovementRadio.Size = new System.Drawing.Size(121, 17);
            this.RandomMovementRadio.TabIndex = 21;
            this.RandomMovementRadio.TabStop = true;
            this.RandomMovementRadio.Text = "Random Movement:";
            this.RandomMovementRadio.UseVisualStyleBackColor = true;
            // 
            // EnabledRadio
            // 
            this.EnabledRadio.AutoSize = true;
            this.EnabledRadio.Location = new System.Drawing.Point(12, 409);
            this.EnabledRadio.Name = "EnabledRadio";
            this.EnabledRadio.Size = new System.Drawing.Size(67, 17);
            this.EnabledRadio.TabIndex = 22;
            this.EnabledRadio.TabStop = true;
            this.EnabledRadio.Text = "Enabled:";
            this.EnabledRadio.UseVisualStyleBackColor = true;
            // 
            // RandomMovementCheck
            // 
            this.RandomMovementCheck.AutoSize = true;
            this.RandomMovementCheck.Location = new System.Drawing.Point(139, 388);
            this.RandomMovementCheck.Name = "RandomMovementCheck";
            this.RandomMovementCheck.Size = new System.Drawing.Size(15, 14);
            this.RandomMovementCheck.TabIndex = 23;
            this.RandomMovementCheck.UseVisualStyleBackColor = true;
            // 
            // EnabledCheck
            // 
            this.EnabledCheck.AutoSize = true;
            this.EnabledCheck.Location = new System.Drawing.Point(85, 411);
            this.EnabledCheck.Name = "EnabledCheck";
            this.EnabledCheck.Size = new System.Drawing.Size(15, 14);
            this.EnabledCheck.TabIndex = 24;
            this.EnabledCheck.UseVisualStyleBackColor = true;
            // 
            // SpriteSelection
            // 
            this.SpriteSelection.FormattingEnabled = true;
            this.SpriteSelection.Location = new System.Drawing.Point(6, 19);
            this.SpriteSelection.Name = "SpriteSelection";
            this.SpriteSelection.Size = new System.Drawing.Size(173, 21);
            this.SpriteSelection.TabIndex = 25;
            // 
            // RenderPrioritySelection
            // 
            this.RenderPrioritySelection.FormattingEnabled = true;
            this.RenderPrioritySelection.Items.AddRange(new object[] {
            "Below Player",
            "Above Player",
            "On Top"});
            this.RenderPrioritySelection.Location = new System.Drawing.Point(6, 19);
            this.RenderPrioritySelection.Name = "RenderPrioritySelection";
            this.RenderPrioritySelection.Size = new System.Drawing.Size(175, 21);
            this.RenderPrioritySelection.TabIndex = 26;
            // 
            // MovementSpeedSelection
            // 
            this.MovementSpeedSelection.FormattingEnabled = true;
            this.MovementSpeedSelection.Items.AddRange(new object[] {
            "Extra Fast",
            "Fast",
            "Normal",
            "Extra Slow",
            "Slow"});
            this.MovementSpeedSelection.Location = new System.Drawing.Point(6, 19);
            this.MovementSpeedSelection.Name = "MovementSpeedSelection";
            this.MovementSpeedSelection.Size = new System.Drawing.Size(175, 21);
            this.MovementSpeedSelection.TabIndex = 27;
            // 
            // MovementFrequencySelection
            // 
            this.MovementFrequencySelection.FormattingEnabled = true;
            this.MovementFrequencySelection.Items.AddRange(new object[] {
            "Instant",
            "High",
            "Normal",
            "Low",
            "Very Low"});
            this.MovementFrequencySelection.Location = new System.Drawing.Point(6, 19);
            this.MovementFrequencySelection.Name = "MovementFrequencySelection";
            this.MovementFrequencySelection.Size = new System.Drawing.Size(175, 21);
            this.MovementFrequencySelection.TabIndex = 28;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.EnabledCheck);
            this.groupBox1.Controls.Add(this.EnabledRadio);
            this.groupBox1.Controls.Add(this.RandomMovementCheck);
            this.groupBox1.Controls.Add(this.RandomMovementRadio);
            this.groupBox1.Controls.Add(this.PassableCheck);
            this.groupBox1.Controls.Add(this.PassableRadio);
            this.groupBox1.Controls.Add(this.MovementFrequencyRadio);
            this.groupBox1.Controls.Add(this.SpriteRadio);
            this.groupBox1.Controls.Add(this.MovementSpeedRadio);
            this.groupBox1.Controls.Add(this.DirectionRadio);
            this.groupBox1.Controls.Add(this.RenderPriorityRadio);
            this.groupBox1.Controls.Add(this.MovementRadio);
            this.groupBox1.Controls.Add(this.TeleportRadio);
            this.groupBox1.Controls.Add(this.groupBox8);
            this.groupBox1.Controls.Add(this.groupBox7);
            this.groupBox1.Controls.Add(this.groupBox6);
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(9, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(426, 450);
            this.groupBox1.TabIndex = 29;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Property";
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.MovementFrequencySelection);
            this.groupBox8.Location = new System.Drawing.Point(223, 302);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(187, 55);
            this.groupBox8.TabIndex = 27;
            this.groupBox8.TabStop = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.MovementSpeedSelection);
            this.groupBox7.Location = new System.Drawing.Point(223, 218);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(187, 55);
            this.groupBox7.TabIndex = 27;
            this.groupBox7.TabStop = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.RenderPrioritySelection);
            this.groupBox6.Location = new System.Drawing.Point(223, 134);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(187, 55);
            this.groupBox6.TabIndex = 26;
            this.groupBox6.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.SpriteSelection);
            this.groupBox5.Location = new System.Drawing.Point(13, 302);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(187, 55);
            this.groupBox5.TabIndex = 16;
            this.groupBox5.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.DirectionSelection);
            this.groupBox4.Location = new System.Drawing.Point(13, 218);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(187, 55);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.MovementSelection);
            this.groupBox3.Location = new System.Drawing.Point(13, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(187, 55);
            this.groupBox3.TabIndex = 14;
            this.groupBox3.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.MapYSelection);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.MapXSelection);
            this.groupBox2.Location = new System.Drawing.Point(13, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(397, 51);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(207, 21);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Map Y:";
            // 
            // MapYSelection
            // 
            this.MapYSelection.Location = new System.Drawing.Point(254, 19);
            this.MapYSelection.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MapYSelection.Name = "MapYSelection";
            this.MapYSelection.Size = new System.Drawing.Size(120, 20);
            this.MapYSelection.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Map X:";
            // 
            // MapXSelection
            // 
            this.MapXSelection.Location = new System.Drawing.Point(61, 19);
            this.MapXSelection.Maximum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.MapXSelection.Name = "MapXSelection";
            this.MapXSelection.Size = new System.Drawing.Size(120, 20);
            this.MapXSelection.TabIndex = 11;
            // 
            // ChangeMapEventPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EventSelection);
            this.Controls.Add(this.MapSelection);
            this.Name = "ChangeMapEventPreset";
            this.Size = new System.Drawing.Size(445, 531);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MapYSelection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MapXSelection)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox MapSelection;
        private System.Windows.Forms.ComboBox EventSelection;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox DirectionSelection;
        private System.Windows.Forms.RadioButton TeleportRadio;
        private System.Windows.Forms.RadioButton MovementRadio;
        private System.Windows.Forms.RadioButton DirectionRadio;
        private System.Windows.Forms.RadioButton SpriteRadio;
        private System.Windows.Forms.RadioButton RenderPriorityRadio;
        private System.Windows.Forms.RadioButton MovementSpeedRadio;
        private System.Windows.Forms.RadioButton MovementFrequencyRadio;
        private System.Windows.Forms.ComboBox MovementSelection;
        private System.Windows.Forms.CheckBox PassableCheck;
        private System.Windows.Forms.RadioButton PassableRadio;
        private System.Windows.Forms.RadioButton RandomMovementRadio;
        private System.Windows.Forms.RadioButton EnabledRadio;
        private System.Windows.Forms.CheckBox RandomMovementCheck;
        private System.Windows.Forms.CheckBox EnabledCheck;
        private System.Windows.Forms.ComboBox SpriteSelection;
        private System.Windows.Forms.ComboBox RenderPrioritySelection;
        private System.Windows.Forms.ComboBox MovementSpeedSelection;
        private System.Windows.Forms.ComboBox MovementFrequencySelection;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown MapYSelection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown MapXSelection;
    }
}
