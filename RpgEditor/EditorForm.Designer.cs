namespace RpgEditor
{
    partial class EditorForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SpawnButton = new System.Windows.Forms.RadioButton();
            this.LayerControl = new System.Windows.Forms.NumericUpDown();
            this.EventButton = new System.Windows.Forms.RadioButton();
            this.FloodFillButton = new System.Windows.Forms.RadioButton();
            this.RectangleButton = new System.Windows.Forms.RadioButton();
            this.PencilButton = new System.Windows.Forms.RadioButton();
            this.SaveMapButton = new System.Windows.Forms.Button();
            this.LoadMapButton = new System.Windows.Forms.Button();
            this.NewMapButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.PrioritiesButton = new System.Windows.Forms.RadioButton();
            this.PassabilitiesButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ApplyTilesetButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TilesetSelectionBox = new System.Windows.Forms.ComboBox();
            this.TilesetNameBox = new System.Windows.Forms.TextBox();
            this.AddTilesetButton = new System.Windows.Forms.Button();
            this.RemoveTilesetButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TilesetsList = new System.Windows.Forms.ListBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.EventCommandDataPanel = new System.Windows.Forms.Panel();
            this.RemoveEventCommandButton = new System.Windows.Forms.Button();
            this.AddEventCommandButton = new System.Windows.Forms.Button();
            this.EventCommandsList = new System.Windows.Forms.ListBox();
            this.EventPassableCheck = new System.Windows.Forms.CheckBox();
            this.ApplyEventChangesButton = new System.Windows.Forms.Button();
            this.EventTriggerSelection = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.EventTriggerBox = new System.Windows.Forms.ComboBox();
            this.EventNameBox = new System.Windows.Forms.TextBox();
            this.AddEventButton = new System.Windows.Forms.Button();
            this.RemoveEventButton = new System.Windows.Forms.Button();
            this.EventsList = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LayerControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1160, 737);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.DarkGray;
            this.tabPage1.Controls.Add(this.splitContainer1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1152, 711);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Maps";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.Gainsboro;
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.SaveMapButton);
            this.splitContainer1.Panel1.Controls.Add(this.LoadMapButton);
            this.splitContainer1.Panel1.Controls.Add(this.NewMapButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1146, 705);
            this.splitContainer1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.SpawnButton);
            this.groupBox1.Controls.Add(this.LayerControl);
            this.groupBox1.Controls.Add(this.EventButton);
            this.groupBox1.Controls.Add(this.FloodFillButton);
            this.groupBox1.Controls.Add(this.RectangleButton);
            this.groupBox1.Controls.Add(this.PencilButton);
            this.groupBox1.Location = new System.Drawing.Point(291, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(412, 43);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Map Tools";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(369, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Layer:";
            // 
            // SpawnButton
            // 
            this.SpawnButton.AutoSize = true;
            this.SpawnButton.Location = new System.Drawing.Point(279, 20);
            this.SpawnButton.Name = "SpawnButton";
            this.SpawnButton.Size = new System.Drawing.Size(85, 17);
            this.SpawnButton.TabIndex = 4;
            this.SpawnButton.TabStop = true;
            this.SpawnButton.Text = "Spawn Point";
            this.SpawnButton.UseVisualStyleBackColor = true;
            // 
            // LayerControl
            // 
            this.LayerControl.Location = new System.Drawing.Point(370, 20);
            this.LayerControl.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.LayerControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.LayerControl.Name = "LayerControl";
            this.LayerControl.Size = new System.Drawing.Size(35, 20);
            this.LayerControl.TabIndex = 4;
            this.LayerControl.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // EventButton
            // 
            this.EventButton.AutoSize = true;
            this.EventButton.Location = new System.Drawing.Point(220, 20);
            this.EventButton.Name = "EventButton";
            this.EventButton.Size = new System.Drawing.Size(53, 17);
            this.EventButton.TabIndex = 3;
            this.EventButton.TabStop = true;
            this.EventButton.Text = "Event";
            this.EventButton.UseVisualStyleBackColor = true;
            // 
            // FloodFillButton
            // 
            this.FloodFillButton.AutoSize = true;
            this.FloodFillButton.Location = new System.Drawing.Point(147, 20);
            this.FloodFillButton.Name = "FloodFillButton";
            this.FloodFillButton.Size = new System.Drawing.Size(66, 17);
            this.FloodFillButton.TabIndex = 2;
            this.FloodFillButton.TabStop = true;
            this.FloodFillButton.Text = "Flood Fill";
            this.FloodFillButton.UseVisualStyleBackColor = true;
            // 
            // RectangleButton
            // 
            this.RectangleButton.AutoSize = true;
            this.RectangleButton.Location = new System.Drawing.Point(66, 20);
            this.RectangleButton.Name = "RectangleButton";
            this.RectangleButton.Size = new System.Drawing.Size(74, 17);
            this.RectangleButton.TabIndex = 1;
            this.RectangleButton.TabStop = true;
            this.RectangleButton.Text = "Rectangle";
            this.RectangleButton.UseVisualStyleBackColor = true;
            // 
            // PencilButton
            // 
            this.PencilButton.AutoSize = true;
            this.PencilButton.Location = new System.Drawing.Point(6, 20);
            this.PencilButton.Name = "PencilButton";
            this.PencilButton.Size = new System.Drawing.Size(54, 17);
            this.PencilButton.TabIndex = 0;
            this.PencilButton.TabStop = true;
            this.PencilButton.Text = "Pencil";
            this.PencilButton.UseVisualStyleBackColor = true;
            // 
            // SaveMapButton
            // 
            this.SaveMapButton.BackColor = System.Drawing.Color.AliceBlue;
            this.SaveMapButton.Location = new System.Drawing.Point(195, 4);
            this.SaveMapButton.Name = "SaveMapButton";
            this.SaveMapButton.Size = new System.Drawing.Size(90, 44);
            this.SaveMapButton.TabIndex = 2;
            this.SaveMapButton.Text = "Save Map";
            this.SaveMapButton.UseVisualStyleBackColor = false;
            this.SaveMapButton.Click += new System.EventHandler(this.SaveMapButton_Click);
            // 
            // LoadMapButton
            // 
            this.LoadMapButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LoadMapButton.BackColor = System.Drawing.Color.AliceBlue;
            this.LoadMapButton.Location = new System.Drawing.Point(99, 4);
            this.LoadMapButton.Name = "LoadMapButton";
            this.LoadMapButton.Size = new System.Drawing.Size(90, 44);
            this.LoadMapButton.TabIndex = 1;
            this.LoadMapButton.Text = "Load Map";
            this.LoadMapButton.UseVisualStyleBackColor = false;
            this.LoadMapButton.Click += new System.EventHandler(this.LoadMapButton_Click);
            // 
            // NewMapButton
            // 
            this.NewMapButton.BackColor = System.Drawing.Color.AliceBlue;
            this.NewMapButton.Location = new System.Drawing.Point(3, 3);
            this.NewMapButton.Name = "NewMapButton";
            this.NewMapButton.Size = new System.Drawing.Size(90, 44);
            this.NewMapButton.TabIndex = 0;
            this.NewMapButton.Text = "New Map";
            this.NewMapButton.UseVisualStyleBackColor = false;
            this.NewMapButton.Click += new System.EventHandler(this.NewMapButton_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.AutoScroll = true;
            this.splitContainer2.Panel1.BackColor = System.Drawing.Color.Gainsboro;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.AutoScroll = true;
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.Black;
            this.splitContainer2.Size = new System.Drawing.Size(1146, 651);
            this.splitContainer2.SplitterDistance = 256;
            this.splitContainer2.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.DarkGray;
            this.tabPage2.Controls.Add(this.panel3);
            this.tabPage2.Controls.Add(this.AddTilesetButton);
            this.tabPage2.Controls.Add(this.RemoveTilesetButton);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.TilesetsList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1152, 711);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Tilesets";
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.Color.Gainsboro;
            this.panel3.Controls.Add(this.groupBox2);
            this.panel3.Location = new System.Drawing.Point(197, 27);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(945, 673);
            this.panel3.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.AliceBlue;
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Controls.Add(this.ApplyTilesetButton);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TilesetSelectionBox);
            this.groupBox2.Controls.Add(this.TilesetNameBox);
            this.groupBox2.Location = new System.Drawing.Point(10, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(926, 655);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tileset Data";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.PrioritiesButton);
            this.groupBox3.Controls.Add(this.PassabilitiesButton);
            this.groupBox3.Location = new System.Drawing.Point(610, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(166, 499);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Properties";
            // 
            // PrioritiesButton
            // 
            this.PrioritiesButton.AutoSize = true;
            this.PrioritiesButton.Location = new System.Drawing.Point(6, 42);
            this.PrioritiesButton.Name = "PrioritiesButton";
            this.PrioritiesButton.Size = new System.Drawing.Size(64, 17);
            this.PrioritiesButton.TabIndex = 1;
            this.PrioritiesButton.TabStop = true;
            this.PrioritiesButton.Text = "Priorities";
            this.PrioritiesButton.UseVisualStyleBackColor = true;
            this.PrioritiesButton.CheckedChanged += new System.EventHandler(this.PrioritiesButton_CheckedChanged);
            // 
            // PassabilitiesButton
            // 
            this.PassabilitiesButton.AutoSize = true;
            this.PassabilitiesButton.Location = new System.Drawing.Point(6, 19);
            this.PassabilitiesButton.Name = "PassabilitiesButton";
            this.PassabilitiesButton.Size = new System.Drawing.Size(82, 17);
            this.PassabilitiesButton.TabIndex = 0;
            this.PassabilitiesButton.TabStop = true;
            this.PassabilitiesButton.Text = "Passabilities";
            this.PassabilitiesButton.UseVisualStyleBackColor = true;
            this.PassabilitiesButton.CheckedChanged += new System.EventHandler(this.PassabilitiesButton_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(347, 19);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(256, 500);
            this.panel2.TabIndex = 5;
            // 
            // ApplyTilesetButton
            // 
            this.ApplyTilesetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyTilesetButton.Location = new System.Drawing.Point(798, 602);
            this.ApplyTilesetButton.Name = "ApplyTilesetButton";
            this.ApplyTilesetButton.Size = new System.Drawing.Size(122, 47);
            this.ApplyTilesetButton.TabIndex = 4;
            this.ApplyTilesetButton.Text = "Save Changes";
            this.ApplyTilesetButton.UseVisualStyleBackColor = true;
            this.ApplyTilesetButton.Click += new System.EventHandler(this.ApplyTilesetButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Tileset:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(62, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name:";
            // 
            // TilesetSelectionBox
            // 
            this.TilesetSelectionBox.FormattingEnabled = true;
            this.TilesetSelectionBox.Location = new System.Drawing.Point(106, 46);
            this.TilesetSelectionBox.Name = "TilesetSelectionBox";
            this.TilesetSelectionBox.Size = new System.Drawing.Size(141, 21);
            this.TilesetSelectionBox.TabIndex = 1;
            // 
            // TilesetNameBox
            // 
            this.TilesetNameBox.Location = new System.Drawing.Point(106, 19);
            this.TilesetNameBox.Name = "TilesetNameBox";
            this.TilesetNameBox.Size = new System.Drawing.Size(141, 20);
            this.TilesetNameBox.TabIndex = 0;
            // 
            // AddTilesetButton
            // 
            this.AddTilesetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddTilesetButton.Location = new System.Drawing.Point(6, 597);
            this.AddTilesetButton.Name = "AddTilesetButton";
            this.AddTilesetButton.Size = new System.Drawing.Size(185, 47);
            this.AddTilesetButton.TabIndex = 5;
            this.AddTilesetButton.Text = "Add Tileset";
            this.AddTilesetButton.UseVisualStyleBackColor = true;
            this.AddTilesetButton.Click += new System.EventHandler(this.AddTilesetButton_Click);
            // 
            // RemoveTilesetButton
            // 
            this.RemoveTilesetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveTilesetButton.Location = new System.Drawing.Point(7, 650);
            this.RemoveTilesetButton.Name = "RemoveTilesetButton";
            this.RemoveTilesetButton.Size = new System.Drawing.Size(185, 47);
            this.RemoveTilesetButton.TabIndex = 5;
            this.RemoveTilesetButton.Text = "Remove TIleset";
            this.RemoveTilesetButton.UseVisualStyleBackColor = true;
            this.RemoveTilesetButton.Click += new System.EventHandler(this.RemoveTilesetButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tilesets";
            // 
            // TilesetsList
            // 
            this.TilesetsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.TilesetsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TilesetsList.FormattingEnabled = true;
            this.TilesetsList.ItemHeight = 20;
            this.TilesetsList.Location = new System.Drawing.Point(6, 27);
            this.TilesetsList.Name = "TilesetsList";
            this.TilesetsList.Size = new System.Drawing.Size(185, 564);
            this.TilesetsList.TabIndex = 0;
            this.TilesetsList.SelectedIndexChanged += new System.EventHandler(this.TilesetsList_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.DarkGray;
            this.tabPage3.Controls.Add(this.panel4);
            this.tabPage3.Controls.Add(this.AddEventButton);
            this.tabPage3.Controls.Add(this.RemoveEventButton);
            this.tabPage3.Controls.Add(this.EventsList);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(1152, 711);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Events";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.Color.Gainsboro;
            this.panel4.Controls.Add(this.groupBox4);
            this.panel4.Location = new System.Drawing.Point(198, 27);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(945, 673);
            this.panel4.TabIndex = 8;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColor = System.Drawing.Color.AliceBlue;
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.EventPassableCheck);
            this.groupBox4.Controls.Add(this.ApplyEventChangesButton);
            this.groupBox4.Controls.Add(this.EventTriggerSelection);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.EventTriggerBox);
            this.groupBox4.Controls.Add(this.EventNameBox);
            this.groupBox4.Location = new System.Drawing.Point(10, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(926, 655);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Event Data";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.groupBox6);
            this.groupBox5.Controls.Add(this.RemoveEventCommandButton);
            this.groupBox5.Controls.Add(this.AddEventCommandButton);
            this.groupBox5.Controls.Add(this.EventCommandsList);
            this.groupBox5.Location = new System.Drawing.Point(253, 19);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(667, 577);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Commands";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.EventCommandDataPanel);
            this.groupBox6.Location = new System.Drawing.Point(197, 19);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(464, 552);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Command Data";
            // 
            // EventCommandDataPanel
            // 
            this.EventCommandDataPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.EventCommandDataPanel.Location = new System.Drawing.Point(6, 19);
            this.EventCommandDataPanel.Name = "EventCommandDataPanel";
            this.EventCommandDataPanel.Size = new System.Drawing.Size(452, 527);
            this.EventCommandDataPanel.TabIndex = 0;
            // 
            // RemoveEventCommandButton
            // 
            this.RemoveEventCommandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveEventCommandButton.Location = new System.Drawing.Point(6, 524);
            this.RemoveEventCommandButton.Name = "RemoveEventCommandButton";
            this.RemoveEventCommandButton.Size = new System.Drawing.Size(185, 47);
            this.RemoveEventCommandButton.TabIndex = 9;
            this.RemoveEventCommandButton.Text = "Remove Command";
            this.RemoveEventCommandButton.UseVisualStyleBackColor = true;
            this.RemoveEventCommandButton.Click += new System.EventHandler(this.RemoveEventCommandButton_Click);
            // 
            // AddEventCommandButton
            // 
            this.AddEventCommandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddEventCommandButton.Location = new System.Drawing.Point(6, 471);
            this.AddEventCommandButton.Name = "AddEventCommandButton";
            this.AddEventCommandButton.Size = new System.Drawing.Size(185, 47);
            this.AddEventCommandButton.TabIndex = 8;
            this.AddEventCommandButton.Text = "Add Command";
            this.AddEventCommandButton.UseVisualStyleBackColor = true;
            this.AddEventCommandButton.Click += new System.EventHandler(this.AddEventCommandButton_Click);
            // 
            // EventCommandsList
            // 
            this.EventCommandsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EventCommandsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EventCommandsList.FormattingEnabled = true;
            this.EventCommandsList.ItemHeight = 20;
            this.EventCommandsList.Location = new System.Drawing.Point(6, 19);
            this.EventCommandsList.Name = "EventCommandsList";
            this.EventCommandsList.Size = new System.Drawing.Size(185, 444);
            this.EventCommandsList.TabIndex = 4;
            this.EventCommandsList.SelectedIndexChanged += new System.EventHandler(this.EventCommandsList_SelectedIndexChanged);
            // 
            // EventPassableCheck
            // 
            this.EventPassableCheck.AutoSize = true;
            this.EventPassableCheck.Location = new System.Drawing.Point(149, 92);
            this.EventPassableCheck.Name = "EventPassableCheck";
            this.EventPassableCheck.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.EventPassableCheck.Size = new System.Drawing.Size(69, 17);
            this.EventPassableCheck.TabIndex = 5;
            this.EventPassableCheck.Text = "Passable";
            this.EventPassableCheck.UseVisualStyleBackColor = true;
            // 
            // ApplyEventChangesButton
            // 
            this.ApplyEventChangesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ApplyEventChangesButton.Location = new System.Drawing.Point(798, 602);
            this.ApplyEventChangesButton.Name = "ApplyEventChangesButton";
            this.ApplyEventChangesButton.Size = new System.Drawing.Size(122, 47);
            this.ApplyEventChangesButton.TabIndex = 4;
            this.ApplyEventChangesButton.Text = "Save Changes";
            this.ApplyEventChangesButton.UseVisualStyleBackColor = true;
            this.ApplyEventChangesButton.Click += new System.EventHandler(this.ApplyEventChangesButton_Click);
            // 
            // EventTriggerSelection
            // 
            this.EventTriggerSelection.AutoSize = true;
            this.EventTriggerSelection.Location = new System.Drawing.Point(30, 68);
            this.EventTriggerSelection.Name = "EventTriggerSelection";
            this.EventTriggerSelection.Size = new System.Drawing.Size(43, 13);
            this.EventTriggerSelection.TabIndex = 3;
            this.EventTriggerSelection.Text = "Trigger:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(33, 41);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Name:";
            // 
            // EventTriggerBox
            // 
            this.EventTriggerBox.FormattingEnabled = true;
            this.EventTriggerBox.Items.AddRange(new object[] {
            "Action",
            "Player Touch"});
            this.EventTriggerBox.Location = new System.Drawing.Point(77, 65);
            this.EventTriggerBox.Name = "EventTriggerBox";
            this.EventTriggerBox.Size = new System.Drawing.Size(141, 21);
            this.EventTriggerBox.TabIndex = 1;
            // 
            // EventNameBox
            // 
            this.EventNameBox.Location = new System.Drawing.Point(77, 38);
            this.EventNameBox.Name = "EventNameBox";
            this.EventNameBox.Size = new System.Drawing.Size(141, 20);
            this.EventNameBox.TabIndex = 0;
            // 
            // AddEventButton
            // 
            this.AddEventButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddEventButton.Location = new System.Drawing.Point(7, 597);
            this.AddEventButton.Name = "AddEventButton";
            this.AddEventButton.Size = new System.Drawing.Size(185, 47);
            this.AddEventButton.TabIndex = 7;
            this.AddEventButton.Text = "Add Event";
            this.AddEventButton.UseVisualStyleBackColor = true;
            this.AddEventButton.Click += new System.EventHandler(this.AddEventButton_Click);
            // 
            // RemoveEventButton
            // 
            this.RemoveEventButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveEventButton.Location = new System.Drawing.Point(7, 650);
            this.RemoveEventButton.Name = "RemoveEventButton";
            this.RemoveEventButton.Size = new System.Drawing.Size(185, 47);
            this.RemoveEventButton.TabIndex = 6;
            this.RemoveEventButton.Text = "Remove Event";
            this.RemoveEventButton.UseVisualStyleBackColor = true;
            this.RemoveEventButton.Click += new System.EventHandler(this.RemoveEventButton_Click);
            // 
            // EventsList
            // 
            this.EventsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.EventsList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EventsList.FormattingEnabled = true;
            this.EventsList.ItemHeight = 20;
            this.EventsList.Location = new System.Drawing.Point(7, 27);
            this.EventsList.Name = "EventsList";
            this.EventsList.Size = new System.Drawing.Size(185, 564);
            this.EventsList.TabIndex = 3;
            this.EventsList.SelectedIndexChanged += new System.EventHandler(this.EventsList_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 24);
            this.label5.TabIndex = 2;
            this.label5.Text = "Events";
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1152, 711);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Sprites";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabPage5
            // 
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(1152, 711);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Items";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // tabPage6
            // 
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(1152, 711);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Npc\'s";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // EditorForm
            // 
            this.ClientSize = new System.Drawing.Size(1184, 761);
            this.Controls.Add(this.tabControl1);
            this.Name = "EditorForm";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LayerControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button NewMapButton;
        private System.Windows.Forms.Button SaveMapButton;
        private System.Windows.Forms.Button LoadMapButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton SpawnButton;
        private System.Windows.Forms.RadioButton EventButton;
        private System.Windows.Forms.RadioButton FloodFillButton;
        private System.Windows.Forms.RadioButton RectangleButton;
        private System.Windows.Forms.RadioButton PencilButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown LayerControl;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox TilesetsList;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox TilesetSelectionBox;
        private System.Windows.Forms.TextBox TilesetNameBox;
        private System.Windows.Forms.Button ApplyTilesetButton;
        private System.Windows.Forms.Button AddTilesetButton;
        private System.Windows.Forms.Button RemoveTilesetButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton PrioritiesButton;
        private System.Windows.Forms.RadioButton PassabilitiesButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox EventsList;
        private System.Windows.Forms.Button AddEventButton;
        private System.Windows.Forms.Button RemoveEventButton;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox EventPassableCheck;
        private System.Windows.Forms.Button ApplyEventChangesButton;
        private System.Windows.Forms.Label EventTriggerSelection;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox EventTriggerBox;
        private System.Windows.Forms.TextBox EventNameBox;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Panel EventCommandDataPanel;
        private System.Windows.Forms.Button RemoveEventCommandButton;
        private System.Windows.Forms.Button AddEventCommandButton;
        private System.Windows.Forms.ListBox EventCommandsList;
    }
}

