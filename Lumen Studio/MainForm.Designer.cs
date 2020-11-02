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
			this.Output = new Lumen.Studio.ConsoleEmulator();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.BottomMenu = new System.Windows.Forms.MenuStrip();
			this.BottomMenuOutput = new System.Windows.Forms.ToolStripMenuItem();
			this.BottomMenuLumenIntercative = new System.Windows.Forms.ToolStripMenuItem();
			this.projectImages = new System.Windows.Forms.ImageList(this.components);
			this.TopMenu = new System.Windows.Forms.MenuStrip();
			this.TopMenuFile = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuOpenFile = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuOpenProject = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuCreate = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuCreateFile = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuCreateProject = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuSave = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuEditSplitter = new System.Windows.Forms.ToolStripSeparator();
			this.TopMenuCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuPaste = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuCut = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuView = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuTools = new System.Windows.Forms.ToolStripMenuItem();
			this.studioHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuPanels = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.TopMenuRunner = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.headerPanel = new System.Windows.Forms.Panel();
			this.TopMenuMaximize = new System.Windows.Forms.Panel();
			this.TopMenuClose = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.TextBoxContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			((System.ComponentModel.ISupportInitialize)(this.textBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Output)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			this.BottomMenu.SuspendLayout();
			this.TopMenu.SuspendLayout();
			this.headerPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.AutoCompleteBrackets = true;
			this.textBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"'};
			this.textBox.AutoIndent = false;
			this.textBox.AutoIndentCharsPatterns = "^\\s*\\.+";
			this.textBox.AutoScrollMinSize = new System.Drawing.Size(42, 16);
			this.textBox.BackBrush = null;
			this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.textBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.textBox.BookmarkColor = System.Drawing.Color.BlueViolet;
			this.textBox.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy1;
			this.textBox.CharHeight = 16;
			this.textBox.CharWidth = 7;
			this.textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Font = new System.Drawing.Font("Consolas", 9F);
			this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.textBox.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.AllTextRange;
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
			this.textBox.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WordWrapControlWidth;
			this.textBox.Zoom = 100;
			this.textBox.ToolTipNeeded += new System.EventHandler<FastColoredTextBoxNS.ToolTipNeededEventArgs>(this.TextBox_ToolTipNeeded);
			this.textBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextBox_TextChanged);
			this.textBox.SelectionChanged += new System.EventHandler(this.TextBox_SelectionChanged);
			this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
			this.textBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseDoubleClick);
			// 
			// Output
			// 
			this.Output.AutoCompleteBracketsList = new char[] {
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
			this.Output.AutoScrollMinSize = new System.Drawing.Size(2, 13);
			this.Output.BackBrush = null;
			this.Output.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.Output.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy1;
			this.Output.CharHeight = 13;
			this.Output.CharWidth = 7;
			this.Output.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.Output.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
			this.Output.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Output.Font = new System.Drawing.Font("Courier New", 9F);
			this.Output.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.Output.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.ChangedRange;
			this.Output.IsReadLineMode = false;
			this.Output.IsReplaceMode = false;
			this.Output.Location = new System.Drawing.Point(0, 0);
			this.Output.Name = "Output";
			this.Output.Paddings = new System.Windows.Forms.Padding(0);
			this.Output.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.Output.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("Output.ServiceColors")));
			this.Output.ShowLineNumbers = false;
			this.Output.Size = new System.Drawing.Size(598, 88);
			this.Output.TabIndex = 1;
			this.Output.TextAreaBorderColor = System.Drawing.Color.SlateGray;
			this.Output.WordWrapMode = FastColoredTextBoxNS.WordWrapMode.WordWrapControlWidth;
			this.Output.Zoom = 100;
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
			this.splitContainer1.Panel2.Controls.Add(this.BottomMenu);
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
			this.splitContainer2.Size = new System.Drawing.Size(598, 210);
			this.splitContainer2.SplitterDistance = 402;
			this.splitContainer2.SplitterWidth = 1;
			this.splitContainer2.TabIndex = 1;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.Output);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bottomPanel.Location = new System.Drawing.Point(0, 0);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(598, 88);
			this.bottomPanel.TabIndex = 3;
			// 
			// BottomMenu
			// 
			this.BottomMenu.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.BottomMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BottomMenuOutput,
            this.BottomMenuLumenIntercative});
			this.BottomMenu.Location = new System.Drawing.Point(0, 88);
			this.BottomMenu.Name = "BottomMenu";
			this.BottomMenu.Size = new System.Drawing.Size(598, 24);
			this.BottomMenu.TabIndex = 2;
			this.BottomMenu.Text = "menuStrip1";
			// 
			// BottomMenuOutput
			// 
			this.BottomMenuOutput.Name = "BottomMenuOutput";
			this.BottomMenuOutput.Size = new System.Drawing.Size(57, 20);
			this.BottomMenuOutput.Text = "Output";
			this.BottomMenuOutput.Click += new System.EventHandler(this.OutputToolStripMenuItem_Click);
			// 
			// BottomMenuLumenIntercative
			// 
			this.BottomMenuLumenIntercative.Name = "BottomMenuLumenIntercative";
			this.BottomMenuLumenIntercative.Size = new System.Drawing.Size(74, 20);
			this.BottomMenuLumenIntercative.Text = "Interactive";
			this.BottomMenuLumenIntercative.Click += new System.EventHandler(this.ShowInteractive);
			// 
			// projectImages
			// 
			this.projectImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.projectImages.ImageSize = new System.Drawing.Size(16, 16);
			this.projectImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// TopMenu
			// 
			this.TopMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.TopMenu.Dock = System.Windows.Forms.DockStyle.None;
			this.TopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuFile,
            this.TopMenuEdit,
            this.TopMenuView,
            this.TopMenuRunner});
			this.TopMenu.Location = new System.Drawing.Point(29, 2);
			this.TopMenu.Name = "TopMenu";
			this.TopMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			this.TopMenu.Size = new System.Drawing.Size(156, 24);
			this.TopMenu.TabIndex = 1;
			this.TopMenu.Text = "menuStringip1";
			// 
			// TopMenuFile
			// 
			this.TopMenuFile.Checked = true;
			this.TopMenuFile.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.TopMenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuOpen,
            this.TopMenuCreate,
            this.TopMenuSave,
            this.TopMenuSaveAs});
			this.TopMenuFile.Name = "TopMenuFile";
			this.TopMenuFile.Size = new System.Drawing.Size(37, 20);
			this.TopMenuFile.Text = "File";
			// 
			// TopMenuOpen
			// 
			this.TopMenuOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuOpenFile,
            this.TopMenuOpenProject});
			this.TopMenuOpen.Margin = new System.Windows.Forms.Padding(0, -1, 0, 0);
			this.TopMenuOpen.Name = "TopMenuOpen";
			this.TopMenuOpen.Size = new System.Drawing.Size(112, 22);
			this.TopMenuOpen.Text = "Open";
			// 
			// TopMenuOpenFile
			// 
			this.TopMenuOpenFile.Margin = new System.Windows.Forms.Padding(-2, -1, 0, 0);
			this.TopMenuOpenFile.Name = "TopMenuOpenFile";
			this.TopMenuOpenFile.Size = new System.Drawing.Size(111, 22);
			this.TopMenuOpenFile.Text = "File";
			this.TopMenuOpenFile.Click += new System.EventHandler(this.OpenToolStringipMenuItem_Click);
			// 
			// TopMenuOpenProject
			// 
			this.TopMenuOpenProject.Margin = new System.Windows.Forms.Padding(-2, 0, 0, 0);
			this.TopMenuOpenProject.Name = "TopMenuOpenProject";
			this.TopMenuOpenProject.Size = new System.Drawing.Size(111, 22);
			this.TopMenuOpenProject.Text = "Project";
			this.TopMenuOpenProject.Click += new System.EventHandler(this.ProjectToolStringipMenuItem_Click);
			// 
			// TopMenuCreate
			// 
			this.TopMenuCreate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuCreateFile,
            this.TopMenuCreateProject});
			this.TopMenuCreate.Name = "TopMenuCreate";
			this.TopMenuCreate.Size = new System.Drawing.Size(112, 22);
			this.TopMenuCreate.Text = "Create";
			// 
			// TopMenuCreateFile
			// 
			this.TopMenuCreateFile.Margin = new System.Windows.Forms.Padding(-1, -1, 0, 0);
			this.TopMenuCreateFile.Name = "TopMenuCreateFile";
			this.TopMenuCreateFile.Size = new System.Drawing.Size(111, 22);
			this.TopMenuCreateFile.Text = "File";
			// 
			// TopMenuCreateProject
			// 
			this.TopMenuCreateProject.Margin = new System.Windows.Forms.Padding(-1, -1, 0, 0);
			this.TopMenuCreateProject.Name = "TopMenuCreateProject";
			this.TopMenuCreateProject.Size = new System.Drawing.Size(111, 22);
			this.TopMenuCreateProject.Text = "Project";
			this.TopMenuCreateProject.Click += new System.EventHandler(this.CreateProjectClick);
			// 
			// TopMenuSave
			// 
			this.TopMenuSave.Name = "TopMenuSave";
			this.TopMenuSave.Size = new System.Drawing.Size(112, 22);
			this.TopMenuSave.Text = "Save";
			this.TopMenuSave.Click += new System.EventHandler(this.SaveToolStringipMenuItem_Click);
			// 
			// TopMenuSaveAs
			// 
			this.TopMenuSaveAs.Name = "TopMenuSaveAs";
			this.TopMenuSaveAs.Size = new System.Drawing.Size(112, 22);
			this.TopMenuSaveAs.Text = "Save as";
			// 
			// TopMenuEdit
			// 
			this.TopMenuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuUndo,
            this.TopMenuRedo,
            this.TopMenuEditSplitter,
            this.TopMenuCopy,
            this.TopMenuPaste,
            this.TopMenuCut,
            this.TopMenuDelete});
			this.TopMenuEdit.Name = "TopMenuEdit";
			this.TopMenuEdit.Size = new System.Drawing.Size(39, 20);
			this.TopMenuEdit.Text = "Edit";
			this.TopMenuEdit.DropDownOpening += new System.EventHandler(this.TopMenuEdit_DropDownOpening);
			// 
			// TopMenuUndo
			// 
			this.TopMenuUndo.Margin = new System.Windows.Forms.Padding(0, -1, 0, 0);
			this.TopMenuUndo.Name = "TopMenuUndo";
			this.TopMenuUndo.Size = new System.Drawing.Size(107, 22);
			this.TopMenuUndo.Text = "Undo";
			this.TopMenuUndo.Click += new System.EventHandler(this.TopMenuUndo_Click);
			// 
			// TopMenuRedo
			// 
			this.TopMenuRedo.Name = "TopMenuRedo";
			this.TopMenuRedo.Size = new System.Drawing.Size(107, 22);
			this.TopMenuRedo.Text = "Redo";
			this.TopMenuRedo.Click += new System.EventHandler(this.TopMenuRedo_Click);
			// 
			// TopMenuEditSplitter
			// 
			this.TopMenuEditSplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.TopMenuEditSplitter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
			this.TopMenuEditSplitter.Name = "TopMenuEditSplitter";
			this.TopMenuEditSplitter.Size = new System.Drawing.Size(104, 6);
			// 
			// TopMenuCopy
			// 
			this.TopMenuCopy.Name = "TopMenuCopy";
			this.TopMenuCopy.Size = new System.Drawing.Size(107, 22);
			this.TopMenuCopy.Text = "Copy";
			this.TopMenuCopy.Click += new System.EventHandler(this.TopMenuCopy_Click);
			// 
			// TopMenuPaste
			// 
			this.TopMenuPaste.Name = "TopMenuPaste";
			this.TopMenuPaste.Size = new System.Drawing.Size(107, 22);
			this.TopMenuPaste.Text = "Paste";
			this.TopMenuPaste.Click += new System.EventHandler(this.TopMenuPaste_Click);
			// 
			// TopMenuCut
			// 
			this.TopMenuCut.Name = "TopMenuCut";
			this.TopMenuCut.Size = new System.Drawing.Size(107, 22);
			this.TopMenuCut.Text = "Cut";
			this.TopMenuCut.Click += new System.EventHandler(this.TopMenuCut_Click);
			// 
			// TopMenuDelete
			// 
			this.TopMenuDelete.Name = "TopMenuDelete";
			this.TopMenuDelete.Size = new System.Drawing.Size(107, 22);
			this.TopMenuDelete.Text = "Delete";
			this.TopMenuDelete.Click += new System.EventHandler(this.TopMenuDelete_Click);
			// 
			// TopMenuView
			// 
			this.TopMenuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TopMenuTools,
            this.TopMenuPanels,
            this.TopMenuSettings});
			this.TopMenuView.Name = "TopMenuView";
			this.TopMenuView.Size = new System.Drawing.Size(44, 20);
			this.TopMenuView.Text = "View";
			// 
			// TopMenuTools
			// 
			this.TopMenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.studioHelpToolStripMenuItem});
			this.TopMenuTools.Margin = new System.Windows.Forms.Padding(0, -1, 0, 0);
			this.TopMenuTools.Name = "TopMenuTools";
			this.TopMenuTools.Size = new System.Drawing.Size(116, 22);
			this.TopMenuTools.Text = "Tools";
			// 
			// studioHelpToolStripMenuItem
			// 
			this.studioHelpToolStripMenuItem.Name = "studioHelpToolStripMenuItem";
			this.studioHelpToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.studioHelpToolStripMenuItem.Text = "Studio Help";
			this.studioHelpToolStripMenuItem.Click += new System.EventHandler(this.StudioHelpToolStripMenuItem_Click);
			// 
			// TopMenuPanels
			// 
			this.TopMenuPanels.Name = "TopMenuPanels";
			this.TopMenuPanels.Size = new System.Drawing.Size(116, 22);
			this.TopMenuPanels.Text = "Panels";
			// 
			// TopMenuSettings
			// 
			this.TopMenuSettings.Name = "TopMenuSettings";
			this.TopMenuSettings.Size = new System.Drawing.Size(116, 22);
			this.TopMenuSettings.Text = "Settings";
			// 
			// TopMenuRunner
			// 
			this.TopMenuRunner.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.TopMenuRunner.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(112)))), ((int)(((byte)(112)))), ((int)(((byte)(112)))));
			this.TopMenuRunner.Image = global::Lumen.Studio.Properties.Resources.run1;
			this.TopMenuRunner.Name = "TopMenuRunner";
			this.TopMenuRunner.Size = new System.Drawing.Size(28, 20);
			this.TopMenuRunner.Click += new System.EventHandler(this.Run);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.White;
			this.imageList1.Images.SetKeyName(0, "function.png");
			this.imageList1.Images.SetKeyName(1, "enumMember.png");
			this.imageList1.Images.SetKeyName(2, "variable.png");
			this.imageList1.Images.SetKeyName(3, "module.png");
			this.imageList1.Images.SetKeyName(4, "fruit-apple-half.png");
			this.imageList1.Images.SetKeyName(5, "fruit-grape.png");
			// 
			// headerPanel
			// 
			this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
			this.headerPanel.Controls.Add(this.TopMenu);
			this.headerPanel.Controls.Add(this.TopMenuMaximize);
			this.headerPanel.Controls.Add(this.TopMenuClose);
			this.headerPanel.Controls.Add(this.panel2);
			this.headerPanel.Location = new System.Drawing.Point(1, 1);
			this.headerPanel.Name = "headerPanel";
			this.headerPanel.Size = new System.Drawing.Size(598, 24);
			this.headerPanel.TabIndex = 4;
			// 
			// TopMenuMaximize
			// 
			this.TopMenuMaximize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TopMenuMaximize.BackgroundImage = global::Lumen.Studio.Properties.Resources.maximize;
			this.TopMenuMaximize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.TopMenuMaximize.Location = new System.Drawing.Point(553, 4);
			this.TopMenuMaximize.Name = "TopMenuMaximize";
			this.TopMenuMaximize.Size = new System.Drawing.Size(16, 16);
			this.TopMenuMaximize.TabIndex = 2;
			this.TopMenuMaximize.Click += new System.EventHandler(this.MaximizeMinimize);
			// 
			// TopMenuClose
			// 
			this.TopMenuClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.TopMenuClose.BackgroundImage = global::Lumen.Studio.Properties.Resources.closew;
			this.TopMenuClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.TopMenuClose.Location = new System.Drawing.Point(575, 4);
			this.TopMenuClose.Name = "TopMenuClose";
			this.TopMenuClose.Size = new System.Drawing.Size(16, 16);
			this.TopMenuClose.TabIndex = 1;
			this.TopMenuClose.Click += new System.EventHandler(this.Exit);
			// 
			// panel2
			// 
			this.panel2.BackgroundImage = global::Lumen.Studio.Properties.Resources.logo1;
			this.panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.panel2.Location = new System.Drawing.Point(10, 4);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(16, 16);
			this.panel2.TabIndex = 0;
			// 
			// TextBoxContextMenu
			// 
			this.TextBoxContextMenu.Name = "TextBoxContextMenu";
			this.TextBoxContextMenu.Size = new System.Drawing.Size(61, 4);
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
			this.MainMenuStrip = this.TopMenu;
			this.MinimumSize = new System.Drawing.Size(600, 350);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Lumen Studio";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.FormLoad);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.bottomPanel.ResumeLayout(false);
			this.BottomMenu.ResumeLayout(false);
			this.BottomMenu.PerformLayout();
			this.TopMenu.ResumeLayout(false);
			this.TopMenu.PerformLayout();
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox textBox;
        public ConsoleEmulator Output;
        private System.Windows.Forms.SplitContainer splitContainer1;
        public System.Windows.Forms.MenuStrip TopMenu;
        private System.Windows.Forms.ToolStripMenuItem TopMenuRunner;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.ImageList imageList1;
        public System.Windows.Forms.ToolStripMenuItem TopMenuFile;
        public System.Windows.Forms.ToolStripMenuItem TopMenuOpen;
        public System.Windows.Forms.ToolStripMenuItem TopMenuSave;
        public System.Windows.Forms.ToolStripMenuItem TopMenuSaveAs;
        public System.Windows.Forms.ToolStripMenuItem TopMenuView;
        public System.Windows.Forms.ToolStripMenuItem TopMenuOpenFile;
        public System.Windows.Forms.ToolStripMenuItem TopMenuOpenProject;
		public System.Windows.Forms.ImageList projectImages;
        public System.Windows.Forms.ToolStripMenuItem TopMenuCreate;
        public System.Windows.Forms.ToolStripMenuItem TopMenuCreateFile;
        public System.Windows.Forms.ToolStripMenuItem TopMenuCreateProject;
        public System.Windows.Forms.ToolStripMenuItem TopMenuEdit;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel TopMenuClose;
        private System.Windows.Forms.Panel TopMenuMaximize;
        private System.Windows.Forms.Panel bottomPanel;
        public System.Windows.Forms.MenuStrip BottomMenu;
        private System.Windows.Forms.ToolStripMenuItem BottomMenuOutput;
        public System.Windows.Forms.ToolStripMenuItem BottomMenuLumenIntercative;
        public System.Windows.Forms.ToolStripMenuItem TopMenuTools;
        public System.Windows.Forms.ToolStripMenuItem TopMenuPanels;
        public System.Windows.Forms.ToolStripMenuItem TopMenuSettings;
        public System.Windows.Forms.ToolStripMenuItem TopMenuUndo;
        public System.Windows.Forms.ToolStripMenuItem TopMenuRedo;
        public System.Windows.Forms.ToolStripMenuItem TopMenuCopy;
        public System.Windows.Forms.ToolStripMenuItem TopMenuPaste;
        public System.Windows.Forms.ToolStripMenuItem TopMenuCut;
        public System.Windows.Forms.ToolStripMenuItem TopMenuDelete;
        public System.Windows.Forms.ContextMenuStrip TextBoxContextMenu;
        public System.Windows.Forms.ToolStripSeparator TopMenuEditSplitter;
		private System.Windows.Forms.ToolStripMenuItem studioHelpToolStripMenuItem;
	}
}

