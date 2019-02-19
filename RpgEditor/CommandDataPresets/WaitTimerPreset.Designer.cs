namespace RpgEditor.CommandDataPresets
{
    partial class WaitTimerPreset
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
            this.TimerControl = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.TimerControl)).BeginInit();
            this.SuspendLayout();
            // 
            // TimerControl
            // 
            this.TimerControl.DecimalPlaces = 3;
            this.TimerControl.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.TimerControl.Location = new System.Drawing.Point(49, 3);
            this.TimerControl.Name = "TimerControl";
            this.TimerControl.Size = new System.Drawing.Size(120, 20);
            this.TimerControl.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Timer:";
            // 
            // WaitTimerPreset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TimerControl);
            this.Name = "WaitTimerPreset";
            this.Size = new System.Drawing.Size(172, 33);
            ((System.ComponentModel.ISupportInitialize)(this.TimerControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown TimerControl;
        private System.Windows.Forms.Label label1;
    }
}
