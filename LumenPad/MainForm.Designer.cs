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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.textBox = new FastColoredTextBoxNS.FastColoredTextBox();
			this.output = new Lumen.Studio.ConsoleEmulator();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.projectImages = new System.Windows.Forms.ImageList(this.components);
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.errorTable = new System.Windows.Forms.DataGridView();
			this.errorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.errorText = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cmdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fileToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.projectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
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
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorTable)).BeginInit();
			this.menuStrip1.SuspendLayout();
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
			this.textBox.AutoScrollMinSize = new System.Drawing.Size(32, 17);
			this.textBox.BackBrush = null;
			this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.textBox.BookmarkColor = System.Drawing.Color.BlueViolet;
			this.textBox.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.STRATEGY_1;
			this.textBox.CharHeight = 17;
			this.textBox.CharWidth = 7;
			this.textBox.CommentPrefix = "--";
			this.textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Font = new System.Drawing.Font("Consolas", 9.75F);
			this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.textBox.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.CHANGED_RANGE;
			this.textBox.Hotkeys = resources.GetString("textBox.Hotkeys");
			this.textBox.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.textBox.IsReplaceMode = false;
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
			this.textBox.Size = new System.Drawing.Size(409, 236);
			this.textBox.TabIndex = 0;
			this.textBox.TextAreaBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.textBox.WordWrapIndent = 1;
			this.textBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WORD_WRAP_CONTROL_WIDTH;
			this.textBox.Zoom = 100;
			this.textBox.ToolTipNeeded += new System.EventHandler<FastColoredTextBoxNS.ToolTipNeededEventArgs>(this.TextBox_ToolTipNeeded);
			this.textBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextBox_TextChanged);
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
			this.output.AutoScrollMinSize = new System.Drawing.Size(2, 15);
			this.output.BackBrush = null;
			this.output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.output.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.STRATEGY_1;
			this.output.CharHeight = 15;
			this.output.CharWidth = 7;
			this.output.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.output.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.output.Dock = System.Windows.Forms.DockStyle.Fill;
			this.output.Font = new System.Drawing.Font("Consolas", 9.75F);
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
			this.output.Size = new System.Drawing.Size(409, 92);
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
			this.splitContainer1.Location = new System.Drawing.Point(0, 25);
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
			this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
			this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.splitContainer1.Size = new System.Drawing.Size(608, 329);
			this.splitContainer1.SplitterDistance = 236;
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
			this.splitContainer2.Size = new System.Drawing.Size(608, 236);
			this.splitContainer2.SplitterDistance = 409;
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
			this.treeView1.Size = new System.Drawing.Size(198, 236);
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
			this.projectImages.Images.SetKeyName(4, "lm.png");
			this.projectImages.Images.SetKeyName(5, "css.png");
			this.projectImages.Images.SetKeyName(6, "img.png");
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.output);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.errorTable);
			this.splitContainer3.Size = new System.Drawing.Size(608, 92);
			this.splitContainer3.SplitterDistance = 409;
			this.splitContainer3.SplitterWidth = 1;
			this.splitContainer3.TabIndex = 2;
			// 
			// errorTable
			// 
			dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.errorTable.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
			this.errorTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.errorTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.errorTable.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.errorTable.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.errorTable.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.errorTable.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
			this.errorTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.errorTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.errorName,
            this.errorText});
			this.errorTable.Cursor = System.Windows.Forms.Cursors.Arrow;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.errorTable.DefaultCellStyle = dataGridViewCellStyle6;
			this.errorTable.EnableHeadersVisualStyles = false;
			this.errorTable.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.errorTable.Location = new System.Drawing.Point(-1, -1);
			this.errorTable.Margin = new System.Windows.Forms.Padding(0);
			this.errorTable.Name = "errorTable";
			this.errorTable.ReadOnly = true;
			this.errorTable.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.errorTable.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.errorTable.RowHeadersVisible = false;
			this.errorTable.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.errorTable.Size = new System.Drawing.Size(274, 92);
			this.errorTable.TabIndex = 0;
			// 
			// errorName
			// 
			this.errorName.HeaderText = "Name";
			this.errorName.Name = "errorName";
			this.errorName.ReadOnly = true;
			// 
			// errorText
			// 
			this.errorText.HeaderText = "Text";
			this.errorText.Name = "errorText";
			this.errorText.ReadOnly = true;
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.runToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.menuStrip1.Size = new System.Drawing.Size(608, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.Checked = true;
			this.fileToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.createToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem1,
            this.projectToolStripMenuItem});
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.openToolStripMenuItem.Text = "Open";
			// 
			// fileToolStripMenuItem1
			// 
			this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
			this.fileToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
			this.fileToolStripMenuItem1.Text = "File";
			this.fileToolStripMenuItem1.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
			// 
			// projectToolStripMenuItem
			// 
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.projectToolStripMenuItem.Text = "Project";
			this.projectToolStripMenuItem.Click += new System.EventHandler(this.ProjectToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveAsToolStripMenuItem.Text = "Save as";
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "View";
			// 
			// cmdToolStripMenuItem
			// 
			this.cmdToolStripMenuItem.Name = "cmdToolStripMenuItem";
			this.cmdToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
			this.cmdToolStripMenuItem.Text = "cmd";
			this.cmdToolStripMenuItem.Click += new System.EventHandler(this.CmdButtonClick);
			// 
			// runToolStripMenuItem
			// 
			this.runToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.runToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			this.runToolStripMenuItem.Image = global::Lumen.Studio.Properties.Resources.Starter;
			this.runToolStripMenuItem.Name = "runToolStripMenuItem";
			this.runToolStripMenuItem.Size = new System.Drawing.Size(28, 20);
			this.runToolStripMenuItem.Click += new System.EventHandler(this.Run);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "func.png");
			this.imageList1.Images.SetKeyName(1, "type.png");
			this.imageList1.Images.SetKeyName(2, "var.png");
			// 
			// createToolStripMenuItem
			// 
			this.createToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem2,
            this.projectToolStripMenuItem1});
			this.createToolStripMenuItem.Name = "createToolStripMenuItem";
			this.createToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.createToolStripMenuItem.Text = "Create";
			// 
			// fileToolStripMenuItem2
			// 
			this.fileToolStripMenuItem2.Name = "fileToolStripMenuItem2";
			this.fileToolStripMenuItem2.Size = new System.Drawing.Size(180, 22);
			this.fileToolStripMenuItem2.Text = "File";
			// 
			// projectToolStripMenuItem1
			// 
			this.projectToolStripMenuItem1.Name = "projectToolStripMenuItem1";
			this.projectToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
			this.projectToolStripMenuItem1.Text = "Project";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
			this.ClientSize = new System.Drawing.Size(608, 354);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "HybridPad";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.textBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.output)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorTable)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FastColoredTextBoxNS.FastColoredTextBox textBox;
		public ConsoleEmulator output;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		public System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.DataGridView errorTable;
		private System.Windows.Forms.DataGridViewTextBoxColumn errorName;
		private System.Windows.Forms.DataGridViewTextBoxColumn errorText;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cmdToolStripMenuItem;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
		private System.Windows.Forms.ImageList projectImages;
		private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem1;
	}
}

