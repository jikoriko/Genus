namespace RpgEditor.CommandDataPresets
{
    partial class EnemySpawnPreset
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
            this.label1 = new System.Windows.Forms.Label();
            this.EnemySelection = new System.Windows.Forms.ComboBox();
            this.EnemyCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.RespawnTime = new System.Windows.Forms.NumericUpDown();
            this.SpawnRadius = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.EnemyCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RespawnTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpawnRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Enemy:";
            // 
            // EnemySelection
            // 
            this.EnemySelection.FormattingEnabled = true;
            this.EnemySelection.Location = new System.Drawing.Point(94, 3);
            this.EnemySelection.Name = "EnemySelection";
            this.EnemySelection.Size = new System.Drawing.Size(121, 21);
            this.EnemySelection.TabIndex = 2;
            // 
            // EnemyCount
            // 
            this.EnemyCount.Location = new System.Drawing.Point(94, 30);
            this.EnemyCount.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.EnemyCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.EnemyCount.Name = "EnemyCount";
            this.EnemyCount.Size = new System.Drawing.Size(120, 20);
            this.EnemyCount.TabIndex = 3;
            this.EnemyCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Count:";
            // 
            // RespawnTime
            // 
            this.RespawnTime.DecimalPlaces = 3;
            this.RespawnTime.Location = new System.Drawing.Point(94, 56);
            this.RespawnTime.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.RespawnTime.Name = "RespawnTime";
            this.RespawnTime.Size = new System.Drawing.Size(120, 20);
            this.RespawnTime.TabIndex = 5;
            // 
            // SpawnRadius
            // 
            this.SpawnRadius.Location = new System.Drawing.Point(94, 82);
            this.SpawnRadius.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.SpawnRadius.Name = "SpawnRadius";
            this.SpawnRadius.Size = new System.Drawing.Size(120, 20);
            this.SpawnRadius.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Respawn Time:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Spawn Radius:";
            // 
            // EnemySpawnPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.SpawnRadius);
            this.Controls.Add(this.RespawnTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.EnemyCount);
            this.Controls.Add(this.EnemySelection);
            this.Controls.Add(this.label1);
            this.Name = "EnemySpawnPreset";
            this.Size = new System.Drawing.Size(219, 112);
            ((System.ComponentModel.ISupportInitialize)(this.EnemyCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RespawnTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpawnRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox EnemySelection;
        private System.Windows.Forms.NumericUpDown EnemyCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown RespawnTime;
        private System.Windows.Forms.NumericUpDown SpawnRadius;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}
