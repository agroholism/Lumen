using System;

namespace Lumen.Studio {
	partial class MainForm {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.textBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.output = new global::Lumen.Studio.ConsoleEmulator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.projectImages = new System.Windows.Forms.ImageList(this.components);
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.bottomMenu = new System.Windows.Forms.MenuStrip();
            this.outputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interactiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStringipMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStringipMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStringipMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStringipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.headerPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.textBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.output)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.bottomMenu.SuspendLayout();
            this.topMenu.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox._UnsafeText = "";
            this.textBox.AutoCompleteBrackets = true;
            this.textBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.textBox.AutoIndentCharsPatterns = "";
            this.textBox.AutoScrollMinSize = new System.Drawing.Size(42, 17);
            this.textBox.BackBrush = null;
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.textBox.BookmarkColor = System.Drawing.Color.BlueViolet;
            this.textBox.BracketsHighlightStringategy = FastColoredTextBoxNS.BracketsHighlightStringategy.StringATEGY_1;
            this.textBox.CharHeight = 17;
            this.textBox.CharWidth = 7;
            this.textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.textBox.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
            this.textBox.Hotkeys = resources.GetString("textBox.Hotkeys");
            this.textBox.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBox.IsReplaceMode = false;
            this.textBox.LeftPadding = 10;
            this.textBox.LineInterval = 2;
            this.textBox.LineNumberColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Name = "textBox";
            this.textBox.Paddings = new System.Windows.Forms.Padding(0);
            this.textBox.PreferredLineWidth = 5;
            this.textBox.ReservedCountOfLineNumberChars = 2;
            this.textBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.textBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("textBox.ServiceColors")));
            this.textBox.ServiceLinesColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBox.Size = new System.Drawing.Size(402, 210);
            this.textBox.TabIndex = 0;
            this.textBox.TextAreaBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBox.WordWrapIndent = 1;
            this.textBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
            this.textBox.Zoom = 100;
            this.textBox.ToolTipNeeded += new System.EventHandler<FastColoredTextBoxNS.ToolTipNeededEventArgs>(this.TextBox_ToolTipNeeded);
            this.textBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextBox_TextChanged);
            this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.textBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseDoubleClick);
            // 
            // output
            // 
            this.output._UnsafeText = "";
            this.output.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.output.AutoScrollMinSize = new System.Drawing.Size(2, 13);
            this.output.BackBrush = null;
            this.output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.output.BracketsHighlightStringategy = FastColoredTextBoxNS.BracketsHighlightStringategy.StringATEGY_1;
            this.output.CharHeight = 13;
            this.output.CharWidth = 7;
            this.output.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.output.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.output.Dock = System.Windows.Forms.DockStyle.Fill;
            this.output.Font = new System.Drawing.Font("Courier New", 9F);
            this.output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.output.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
            this.output.Hotkeys = resources.GetString("output.Hotkeys");
            this.output.IsReadLineMode = false;
            this.output.IsReplaceMode = false;
            this.output.Location = new System.Drawing.Point(0, 0);
            this.output.Name = "output";
            this.output.Paddings = new System.Windows.Forms.Padding(0);
            this.output.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.output.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("output.ServiceColors")));
            this.output.ShowLineNumbers = false;
            this.output.Size = new System.Drawing.Size(598, 88);
            this.output.TabIndex = 1;
            this.output.TextAreaBorderColor = System.Drawing.Color.SlateGray;
            this.output.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
            this.output.Zoom = 100;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.splitContainer1.Location = new System.Drawing.Point(1, 26);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.bottomPanel);
            this.splitContainer1.Panel2.Controls.Add(this.bottomMenu);
            this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer1.Size = new System.Drawing.Size(598, 323);
            this.splitContainer1.SplitterDistance = 210;
            this.splitContainer1.SplitterWidth = 1;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.textBox);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.treeView1);
            this.splitContainer2.Size = new System.Drawing.Size(598, 210);
            this.splitContainer2.SplitterDistance = 402;
            this.splitContainer2.SplitterWidth = 1;
            this.splitContainer2.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.projectImages;
            this.treeView1.ItemHeight = 16;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(195, 210);
            this.treeView1.TabIndex = 0;
            this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
            // 
            // projectImages
            // 
            this.projectImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("projectImages.ImageStream")));
            this.projectImages.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.projectImages.Images.SetKeyName(0, "fold.png");
            this.projectImages.Images.SetKeyName(1, "other.png");
            this.projectImages.Images.SetKeyName(2, "html.png");
            this.projectImages.Images.SetKeyName(3, "py.png");
            this.projectImages.Images.SetKeyName(4, "icons8-лямбда-filled-96.png");
            this.projectImages.Images.SetKeyName(5, "css.png");
            this.projectImages.Images.SetKeyName(6, "img.png");
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.output);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(0, 0);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(598, 88);
            this.bottomPanel.TabIndex = 3;
            // 
            // bottomMenu
            // 
            this.bottomMenu.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outputToolStripMenuItem,
            this.interactiveToolStripMenuItem});
            this.bottomMenu.Location = new System.Drawing.Point(0, 88);
            this.bottomMenu.Name = "bottomMenu";
            this.bottomMenu.Size = new System.Drawing.Size(598, 24);
            this.bottomMenu.TabIndex = 2;
            this.bottomMenu.Text = "menuStrip1";
            // 
            // outputToolStripMenuItem
            // 
            this.outputToolStripMenuItem.Name = "outputToolStripMenuItem";
            this.outputToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.outputToolStripMenuItem.Text = "Output";
            this.outputToolStripMenuItem.Click += new System.EventHandler(this.OutputToolStripMenuItem_Click);
            // 
            // interactiveToolStripMenuItem
            // 
            this.interactiveToolStripMenuItem.Name = "interactiveToolStripMenuItem";
            this.interactiveToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.interactiveToolStripMenuItem.Text = "Interactive";
            this.interactiveToolStripMenuItem.Click += new System.EventHandler(this.ShowInteractive);
            // 
            // topMenu
            // 
            this.topMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.topMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.topMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStringipMenuItem,
            this.viewToolStringipMenuItem,
            this.settingsToolStringipMenuItem,
            this.runToolStringipMenuItem});
            this.topMenu.Location = new System.Drawing.Point(29, 0);
            this.topMenu.Name = "topMenu";
            this.topMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.topMenu.Size = new System.Drawing.Size(178, 24);
            this.topMenu.TabIndex = 1;
            this.topMenu.Text = "menuStringip1";
            // 
            // fileToolStringipMenuItem
            // 
            this.fileToolStringipMenuItem.Checked = true;
            this.fileToolStringipMenuItem.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.fileToolStringipMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStringipMenuItem,
            this.createToolStringipMenuItem,
            this.saveToolStringipMenuItem,
            this.saveAsToolStringipMenuItem});
            this.fileToolStringipMenuItem.Name = "fileToolStringipMenuItem";
            this.fileToolStringipMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStringipMenuItem.Text = "File";
            // 
            // openToolStringipMenuItem
            // 
            this.openToolStringipMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStringipMenuItem1,
            this.projectToolStringipMenuItem});
            this.openToolStringipMenuItem.Name = "openToolStringipMenuItem";
            this.openToolStringipMenuItem.Size = new System.Drawing.Size(112, 22);
            this.openToolStringipMenuItem.Text = "Open";
            // 
            // fileToolStringipMenuItem1
            // 
            this.fileToolStringipMenuItem1.Name = "fileToolStringipMenuItem1";
            this.fileToolStringipMenuItem1.Size = new System.Drawing.Size(111, 22);
            this.fileToolStringipMenuItem1.Text = "File";
            this.fileToolStringipMenuItem1.Click += new System.EventHandler(this.OpenToolStringipMenuItem_Click);
            // 
            // projectToolStringipMenuItem
            // 
            this.projectToolStringipMenuItem.Name = "projectToolStringipMenuItem";
            this.projectToolStringipMenuItem.Size = new System.Drawing.Size(111, 22);
            this.projectToolStringipMenuItem.Text = "Project";
            this.projectToolStringipMenuItem.Click += new System.EventHandler(this.ProjectToolStringipMenuItem_Click);
            // 
            // createToolStringipMenuItem
            // 
            this.createToolStringipMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStringipMenuItem2,
            this.projectToolStringipMenuItem1});
            this.createToolStringipMenuItem.Name = "createToolStringipMenuItem";
            this.createToolStringipMenuItem.Size = new System.Drawing.Size(112, 22);
            this.createToolStringipMenuItem.Text = "Create";
            // 
            // fileToolStringipMenuItem2
            // 
            this.fileToolStringipMenuItem2.Name = "fileToolStringipMenuItem2";
            this.fileToolStringipMenuItem2.Size = new System.Drawing.Size(111, 22);
            this.fileToolStringipMenuItem2.Text = "File";
            // 
            // projectToolStringipMenuItem1
            // 
            this.projectToolStringipMenuItem1.Name = "projectToolStringipMenuItem1";
            this.projectToolStringipMenuItem1.Size = new System.Drawing.Size(111, 22);
            this.projectToolStringipMenuItem1.Text = "Project";
            this.projectToolStringipMenuItem1.Click += new System.EventHandler(this.CreateProjectClick);
            // 
            // saveToolStringipMenuItem
            // 
            this.saveToolStringipMenuItem.Name = "saveToolStringipMenuItem";
            this.saveToolStringipMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveToolStringipMenuItem.Text = "Save";
            this.saveToolStringipMenuItem.Click += new System.EventHandler(this.SaveToolStringipMenuItem_Click);
            // 
            // saveAsToolStringipMenuItem
            // 
            this.saveAsToolStringipMenuItem.Name = "saveAsToolStringipMenuItem";
            this.saveAsToolStringipMenuItem.Size = new System.Drawing.Size(112, 22);
            this.saveAsToolStringipMenuItem.Text = "Save as";
            // 
            // viewToolStringipMenuItem
            // 
            this.viewToolStringipMenuItem.Name = "viewToolStringipMenuItem";
            this.viewToolStringipMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStringipMenuItem.Text = "View";
            // 
            // settingsToolStringipMenuItem
            // 
            this.settingsToolStringipMenuItem.Name = "settingsToolStringipMenuItem";
            this.settingsToolStringipMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStringipMenuItem.Text = "Settings";
            // 
            // runToolStringipMenuItem
            // 
            this.runToolStringipMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.runToolStringipMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
            this.runToolStringipMenuItem.Image = global::Lumen.Studio.Properties.Resources.Starter;
            this.runToolStringipMenuItem.Name = "runToolStringipMenuItem";
            this.runToolStringipMenuItem.Size = new System.Drawing.Size(28, 20);
            this.runToolStringipMenuItem.Click += new System.EventHandler(this.Run);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.White;
            this.imageList1.Images.SetKeyName(0, "3d.png");
            this.imageList1.Images.SetKeyName(1, "hexagon.png");
            this.imageList1.Images.SetKeyName(2, "triangle.png");
            this.imageList1.Images.SetKeyName(3, "is_mainEnum.png");
            this.imageList1.Images.SetKeyName(4, "module.png");
            this.imageList1.Images.SetKeyName(5, "keyword.png");
            // 
            // headerPanel
            // 
            this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.headerPanel.Controls.Add(this.topMenu);
            this.headerPanel.Controls.Add(this.panel4);
            this.headerPanel.Controls.Add(this.panel3);
            this.headerPanel.Controls.Add(this.panel2);
            this.headerPanel.Location = new System.Drawing.Point(1, 1);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(598, 24);
            this.headerPanel.TabIndex = 4;
            this.headerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FormMouseDown);
            this.headerPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FormMouseMove);
            this.headerPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.FormMouseUp);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackgroundImage = global::Lumen.Studio.Properties.Resources.fullscreen;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel4.Location = new System.Drawing.Point(553, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(16, 16);
            this.panel4.TabIndex = 2;
            this.panel4.Click += new System.EventHandler(this.Panel4_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackgroundImage = global::Lumen.Studio.Properties.Resources.close2;
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panel3.Location = new System.Drawing.Point(575, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(16, 16);
            this.panel3.TabIndex = 1;
            this.panel3.Click += new System.EventHandler(this.Panel3_Click);
            // 
            // panel2
            // 
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Location = new System.Drawing.Point(10, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(16, 16);
            this.panel2.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(600, 350);
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.topMenu;
            this.MinimumSize = new System.Drawing.Size(600, 350);
            this.Name = "MainForm";
            this.Text = "Lumen Studio";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.textBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.output)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.bottomMenu.ResumeLayout(false);
            this.bottomMenu.PerformLayout();
            this.topMenu.ResumeLayout(false);
            this.topMenu.PerformLayout();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);

		}

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox textBox;
		public ConsoleEmulator output;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.MenuStrip topMenu;
		private System.Windows.Forms.ToolStripMenuItem runToolStringipMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		public System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStringipMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStringipMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStringipMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStringipMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStringipMenuItem;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStringipMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem projectToolStringipMenuItem;
		private System.Windows.Forms.ImageList projectImages;
		private System.Windows.Forms.ToolStripMenuItem createToolStringipMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStringipMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem projectToolStringipMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStringipMenuItem;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.MenuStrip bottomMenu;
        private System.Windows.Forms.ToolStripMenuItem outputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interactiveToolStripMenuItem;
    }
}

