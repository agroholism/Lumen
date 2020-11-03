using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using FastColoredTextBoxNS;
using Timer = System.Windows.Forms.Timer;

namespace Lumen.Studio {
	public sealed partial class MainForm : LumenStudioForm /*/ Form/**/ {
		/// <summary> Singleton realization  </summary>
		public static MainForm Instance { get; private set; }

		public static TextBoxManager MainTextBoxManager { get; set; }
		public static PictureBox MainPictureBox { get; } = new CopyableImageBox();

		public static Control ActiveWorkingAreaControl { get; private set; }
		public static Control ActiveBottomControl { get; private set; }

		public static DataGridView DebugGrid = new DebugGrid();

		private readonly String argument;

		internal MainForm(String[] args) : base() {
			Instance = this;

			this.InitializeComponent();

			this.AnimateForm();

			this.textBox.ContextMenuStrip = this.TextBoxContextMenu;

			MainTextBoxManager = new TextBoxManager(this.textBox) {
				Language = Settings.Languages[0]
			};

			this.InitializeDebugGrid();

			//*
			this.headerPanel.MouseDown += new MouseEventHandler(this.FormMouseDown);
			this.headerPanel.MouseMove += new MouseEventHandler(this.FormMouseMove);
			this.headerPanel.MouseUp += new MouseEventHandler(this.FormMouseUp);
			//*/

			//this.treeView1.BeforeSelect += (sender, e) => e.Cancel = true;

			InitializeConsole();
			this.splitContainer2.Panel2.Controls.Add(ProjectManager.ProjectViewier);
			this.splitContainer2.Panel1.Controls.Add(MainPictureBox);

			this.MainMenuStrip.Renderer = new LumenMenuRenderer();
			this.BottomMenu.Renderer = new LumenMenuRenderer();
			this.TextBoxContextMenu.Renderer = new LumenMenuRenderer();

			this.ApplyColorScheme();

			if (args.Length > 0) {
				this.argument = args[0];
			}

			/*LSSB c = new LSSB(this.textBox);
			c.Dock = DockStyle.Right;
			this.textBox.Dock = DockStyle.Fill;

			this.splitContainer2.Panel1.Controls.Add(c);*/

			ToolStripItem x = TextBoxContextMenu.Items.Add("Format");
			x.Click += (sender, e) => {
				var ast = Lmi.Interpriter.GetAST(this.textBox.Text, "");
				this.textBox.Text = String.Join(Environment.NewLine, ast);
			};

			this.MakeControlActiveInWorkingArea(this.textBox);
			this.MakeControlActiveInBottom(this.Output);

			Lmi.Interpriter.BasePath = Directory.GetCurrentDirectory();
		}

		private void InitializeConsole() {
			ConsoleWriter.Instance = new ConsoleWriter(this.Output);
			ConsoleReader.Instance = new ConsoleReader(this.Output);

			Console.SetOut(ConsoleWriter.Instance);
			Console.SetIn(ConsoleReader.Instance);
		}

		private void InitializeDebugGrid() {
			DebugGrid.CellDoubleClick += (sender, e) => {
				DataGridViewCell cell = DebugGrid.Rows[e.RowIndex].Cells[2];

				if (cell is DataCell dataCell) {
					this.history.Push(dataCell.__value);
					this.ValueUpdate(dataCell.__value);
				}
			};

			PictureBox backButton = new PictureBox {
				Visible = false,
				BackColor = Settings.BackgroundColor,
				// Image = Image.FromFile(Settings.ExecutablePath + $"\\settings\\themes\\icons\\standart\\back.png"),
				Size = new Size(14, 15)
			};

			backButton.Click += (sender, e) => {
				if(this.history.Count == 1) {
					return;
				}

				this.history.Pop();
				this.ValueUpdate(this.history.Peek());
			};

			DebugGrid.Controls.Add(backButton);
			this.bottomPanel.Controls.Add(DebugGrid);
			DebugGrid.Visible = false;
			ToolStripItem item = this.BottomMenu.Items.Add("Debug");
			item.Click += (sender, e) => {
				if (ActiveBottomControl == DebugGrid) {
					return;
				}

				ActiveBottomControl.Visible = false;
				DebugGrid.Size = new Size(this.bottomPanel.Size.Width + 2, this.bottomPanel.Size.Height + 1);
				DebugGrid.Location = new Point(-1, -1);
				DebugGrid.Visible = true;
				ActiveBottomControl = DebugGrid;
				backButton.Location = new Point(0, 1);
				backButton.Visible = true;
			};
		}

		private void AnimateForm() {
			this.Opacity = 0;

			Timer launchTimer = new Timer {
				Interval = 10,
				Enabled = true
			};

			launchTimer.Tick += (sender, e) => {
				this.Opacity += 0.1;
				if (this.Opacity >= 1) {
					launchTimer.Enabled = false;
				}
			};
		}

		#region ApplyTheme

		public void ApplyColorScheme() {
			this.LoadImages();
			//this.treeView1.HideSelection = false;

			this.Font = Settings.DefaultInterfaceFont;

			this.Output.Font = new Font("Consolas", 9f);
			this.Output.Dock = DockStyle.None;
			this.Output.Size = this.bottomPanel.Size;
			this.Output.Location = new Point(0, this.bottomPanel.Height);
			this.Output.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			ProjectManager.ProjectViewier.Font = Settings.DefaultInterfaceFont;

			this.splitContainer2.SplitterWidth = 1;

			this.headerPanel.BackColor = Settings.BackgroundColor;

			this.textBox.BackColor = Settings.BackgroundColor;
			this.textBox.ForeColor = Settings.ForegroundColor;
			this.textBox.IndentBackColor = Settings.BackgroundColor;
			this.textBox.LineNumberColor = Settings.ForegroundColor;
			this.textBox.PaddingBackColor = Settings.BackgroundColor;
			this.textBox.TextAreaBorderColor = Settings.BackgroundColor;
			this.textBox.ServiceLinesColor = Settings.BackgroundColor;
			ProjectManager.ProjectViewier.BackColor = Settings.BackgroundColor;
			ProjectManager.ProjectViewier.ForeColor = Settings.ForegroundColor;

			this.TopMenu.BackColor = Settings.BackgroundColor;
			this.BackColor = Settings.LinesColor;
			this.Output.BackColor = Settings.BackgroundColor;
			this.Output.ForeColor = Settings.ForegroundColor;
			this.splitContainer1.BackColor = Settings.LinesColor;
			this.splitContainer2.BackColor = Settings.LinesColor;

			this.BottomMenu.BackColor = Settings.BackgroundColor;
			this.BottomMenu.ForeColor = Settings.ForegroundColor;

			this.textBox.ContextMenuStrip.BackColor = Settings.BackgroundColor;
			this.textBox.ContextMenuStrip.ForeColor = Settings.ForegroundColor;
			this.textBox.ContextMenuStrip.Font = Settings.DefaultInterfaceFont;
			this.textBox.ContextMenuStrip.ShowImageMargin = false;

			this.textBox.ShowScrollBars = Settings.ScrollBars == Settings.VerticalScrollMode.ON ? true : false;

			if (MainTextBoxManager.Menu.Renderer is LumenMenuRenderer renderer) {
				renderer.ArrowColor = Settings.AutocompleteMenu.ArrowColor;
				renderer.BackColor = Settings.AutocompleteMenu.Background;
				renderer.BorderColor = Settings.AutocompleteMenu.BordersColor;
				renderer.ForeColor = Settings.AutocompleteMenu.ForegroundColor;
				renderer.SelectedBackColor = Settings.AutocompleteMenu.SelectedBackground;

				MainTextBoxManager.Menu.listView.HoveredColor = Settings.AutocompleteMenu.SelectedBackground;
				MainTextBoxManager.Menu.listView.SelectedColor = Settings.AutocompleteMenu.SelectedBackground;

				// MainTextBoxManager.Menu.listView.ToolTipBackgroundColor = Settings.AutocompleteMenu.ToolTipBackColor;
				// MainTextBoxManager.Menu.listView.ToolTipForegroundColor = Settings.AutocompleteMenu.ToolTipForeColor;
			}

			MainTextBoxManager.Menu.BackColor = Settings.AutocompleteMenu.Background;
			MainTextBoxManager.Menu.ForeColor = Settings.AutocompleteMenu.ForegroundColor;
			// MainTextBoxManager.Menu.BorderColor = Settings.AutocompleteMenu.BordersColor;

			if (this.TopMenu.Renderer is LumenMenuRenderer lumenMenu) {
				lumenMenu.ArrowColor = Settings.TopMenu.ArrowColor;
				lumenMenu.BackColor = Settings.TopMenu.Background;
				lumenMenu.BorderColor = Settings.TopMenu.BordersColor;
				lumenMenu.ForeColor = Settings.TopMenu.ForegroundColor;
				lumenMenu.SelectedBackColor = Settings.TopMenu.SelectedBackground;
				lumenMenu.ArrowSelectedColor = Settings.TopMenu.ArrowSelectedColor;
			}
		}

		private void LoadImages() {
			foreach (Object i in Settings.ProjectManagerImages) {
				if (i is Image img) {
					this.projectImages.Images.Add(img);
				}
				else if (i is Icon ico) {
					this.projectImages.Images.Add(ico);
				}
			}
		}

		#endregion

		Int32 previousLine = -1;
		public volatile Boolean WaitDebug;
		public volatile Boolean IsLaunched;
		public Thread WorkingThread { get; set; }
		Lang.Scope currentScope;
		Stack<Object> history;

		private void Run(Object sender, EventArgs e) {
			if (this.IsLaunched && !this.WaitDebug) {
				this.WorkingThread.Abort();
				this.IsLaunched = false;
				this.WaitDebug = false;
				this.ReBreakpointLine();
				this.TopMenuRunner.Image = Properties.Resources.run1;
				return;
			}

			if (this.WaitDebug) {
				this.WaitDebug = false;
				return;
			}

			this.Output.Clear();

			if (MainTextBoxManager.Language != null) {
				if (MainTextBoxManager.Language.Build != null) {
					ProjectManager.SaveFile();
					this.StartProcess(MainTextBoxManager.Language.Build.Replace("[FILE]", ProjectManager.CurrentFile));
				}
				else {
					ProjectManager.SaveFile();
					this.IsLaunched = true;
					this.TopMenuRunner.Image = Properties.Resources.stop;

					StringBuilder result = new StringBuilder();

					List<Int32> breakpoints = this.textBox.Bookmarks.Select(x => x.LineIndex).ToList();
					String[] sts = MainTextBoxManager.TextBox.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
					for (Int32 i = 0; i < sts.Length; i++) {
						if (breakpoints.Contains(i)) {
							String prefix = "";
							for (Int32 j = 0; j < sts.Length; j++) {
								if (sts[i][j] == ' ') {
									prefix += ' ';
								}
								else if (sts[i][j] == '\t') {
									prefix += '\t';
								}
								else {
									break;
								}
							}
							result.Append($"{prefix}_lsOnDebug {i + 1};");
						}
						result.Append(sts[i]).Append(Environment.NewLine);
					}

					File.WriteAllText("main.lm", result.ToString());

					Lang.Scope scope = new Lang.Scope();

					scope.Bind("_lsOnDebug", new Lang.LambdaFun((_scope, args) => {
						this.BreakpointOnLine(Lang.Converter.ToInt(_scope["line"], scope) - 1);
						this.Review(_scope.parent, "");
						return Lang.Const.UNIT;
					}) {
						Arguments = new List<Lang.Expressions.IPattern> {
							new Lang.Expressions.NamePattern("line")
						}
					});

					this.WorkingThread = new Thread(() => {
						Lmi.Interpriter.Start("main.lm", scope);
						this.InvokeNeded(() => {
							this.ReBreakpointLine();
							this.TopMenuRunner.Image = Properties.Resources.run1;
							this.IsLaunched = false;
						});
					});
					this.WorkingThread.Start();
				}
			}
		}

		private void Review(Lang.Scope e, String v) {
			this.history = new Stack<Object>();
			this.history.Push(e);
			DebugGrid.Rows.Clear();
			this.currentScope = e;

			DataGridViewRow row;
			if (e.parent != null) {
				row = new DataGridViewRow();
				row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
				row.Cells.Add(new DataGridViewTextBoxCell() { Value = "Extern scope" });
				row.Cells.Add(new DataCell(e.parent) { Value = "<...>" });
				DebugGrid.Rows.Add(row);
			}

			foreach (KeyValuePair<String, Lang.Value> i in e.variables) {
				row = new DataGridViewRow();
				row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
				row.Cells.Add(new DataGridViewTextBoxCell() { Value = i.Key });
				row.Cells.Add(new DataCell(i.Value) { Value = Rename(i.Value) });
				DebugGrid.Rows.Add(row);
			}

			this.TopMenuRunner.Image = Properties.Resources.next;
			this.WaitDebug = true;
			try {
				while (this.WaitDebug) {
					Application.DoEvents();
					Thread.Sleep(5);
				}
			}
			finally {
				this.WaitDebug = false;
			}
			this.TopMenuRunner.Image = Properties.Resources.stop;
			this.currentScope = null;
			DebugGrid.Rows.Clear();
		}

		private void ChangeNameColumn(String name, Int32 index) {
			DebugGrid.Columns[index].HeaderText = name;
		}

		private void ValueUpdate(Object value) {
			DebugGrid.Rows.Clear();

			this.ChangeNameColumn("Name", 1);
			if (value is Lang.Instance) {
				this.ObjectUpdate(value as Lang.Instance);
			}
			else if (value is Lang.Scope scope) {
				this.currentScope = scope;
				if (scope.parent != null) {
					DataGridViewRow row = new DataGridViewRow();
					row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
					row.Cells.Add(new DataGridViewTextBoxCell() { Value = "Extern scope" });
					row.Cells.Add(new DataCell(scope.parent) { Value = "<...>" });
					DebugGrid.Rows.Add(row);
				}

				foreach (KeyValuePair<String, Lang.Value> i in scope.variables) {
					DataGridViewRow row = new DataGridViewRow();
					row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
					row.Cells.Add(new DataGridViewTextBoxCell() { Value = i.Key });
					row.Cells.Add(new DataCell(i.Value) { Value = Rename(i.Value) });
					DebugGrid.Rows.Add(row);
				}
			}
			else if(value is Lang.List list) {
				this.ChangeNameColumn("Index", 1);
				Int32 index = 0;
				foreach (Lang.Value i in list.Value) {
					DataGridViewRow row = new DataGridViewRow();
					row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
					row.Cells.Add(new DataGridViewTextBoxCell() { Value = index });
					row.Cells.Add(new DataCell(i) { Value = Rename(i) });
					DebugGrid.Rows.Add(row);
					index++;
				}
			}
			else if (value is Lang.Array array) {
				this.ChangeNameColumn("Index", 1);
				Int32 index = 0;
				foreach (Lang.Value i in array.InternalValue) {
					DataGridViewRow row = new DataGridViewRow();
					row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
					row.Cells.Add(new DataGridViewTextBoxCell() { Value = index });
					row.Cells.Add(new DataCell(i) { Value = Rename(i) });
					DebugGrid.Rows.Add(row);
					index++;
				}
			}
			else if (value is Lang.Module module) {
				this.ModuleUpdate(module);
			}
		}

		private void ModuleUpdate(Lang.Module module) {
			DataGridViewRow row;
			foreach (var i in module.Members) {
				row = new DataGridViewRow();
				row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
				row.Cells.Add(new DataGridViewTextBoxCell { Value = i.Key });
				row.Cells.Add(new DataCell(i.Value) { Value = Rename(i.Value) });
				DebugGrid.Rows.Add(row);
			}
		}

		private void ObjectUpdate(Lang.Instance value) {
			this.ChangeNameColumn("Fields", 1);
			DataGridViewRow row;
			foreach (String field in (value.Type as Lang.Constructor).Fields) {
				Lang.Value valueOfField = value.GetField(field, null);
				row = new DataGridViewRow();
				row.Cells.Add(new DataGridViewImageCell() { Value = Settings.EmptyImage });
				row.Cells.Add(new DataGridViewTextBoxCell { Value = field });
				row.Cells.Add(new DataCell(valueOfField) { Value = Rename(valueOfField) });
				DebugGrid.Rows.Add(row);
			}
		}

		private void ReBreakpointLine() {
			if (this.previousLine != -1) {
				Range i = this.textBox.GetLine(this.previousLine);
				i.ClearStyle(Settings.Error);
				i.ClearStyle(Settings.Type,
					Settings.Function, Settings.String,
					Settings.Keyword, Settings.Comment);
				i.ClearStyle(Settings.ActiveBreakpoint);
				MainTextBoxManager.TextBox.Bookmarks.GetBookmark(this.previousLine).Color = Settings.TextBoxBookMarkColor;
				i.SetStyle(Settings.UnactiveBreakpoint);
				this.previousLine = -1;
			}
		}

		private void BreakpointOnLine(Int32 line) {
			if (line >= this.textBox.Lines.Count)
				return;
			Range i = this.textBox.GetLine(line);
			this.ReBreakpointLine();
			this.previousLine = line;
			i.ClearStyle(Settings.UnactiveBreakpoint);
			i.ClearStyle(Settings.Error);
			i.ClearStyle(Settings.Type,
				Settings.Function, Settings.String,
				Settings.Keyword, Settings.Comment);
			i.SetStyle(Settings.ActiveBreakpoint);

			MainTextBoxManager.TextBox.Bookmarks.GetBookmark(line).Color = Settings.TextBoxActiveBookMarkColor;

			this.textBox.DoRangeVisible(i);
		}

		public static String Rename(Lang.Value value) {
			if (value is Lang.List list) {
				if (list.Value.Count() < 10) {
					return list.ToString();
				}
				else {
					return $"List ({list.Value.Count()} elems)";
				}
			}

			if (value is Lang.Text ks) {
				String s = ks.ToString();
				if (s.Length < 15) {
					return s;
				}
				else {
					return $"Text ({s.Length} chars)";
				}
			}

			try {
				return value.ToString();
			}
			catch {
				return "";
			}
		}

		private void StartProcess(String name) {
			Tuple<String, String> nameAndArgs = this.GetNameAndArgs(name);

			Process proc = new Process() {
				StartInfo = new ProcessStartInfo {
					FileName = nameAndArgs.Item1,
					Arguments = nameAndArgs.Item2
				}
			};

			Task.Factory.StartNew(() => {
				proc.Start();
				proc.WaitForExit();
			});
		}

		private Tuple<String, String> GetNameAndArgs(String command) {
			String[] src = command.Split(' ');

			StringBuilder builder = new StringBuilder();
			for (Int32 i = 1; i < src.Length; i++) {
				builder.Append(src[i]).Append(" ");
			}

			return new Tuple<String, String>(src[0], builder.ToString());
		}

		private void RaiseError(Int32 line) {
			this.textBox.GetLine(line - 1).SetStyle(Settings.Error);
		}

		internal void RaiseError(IError result) {
			if (result.ErrorLine > 0) {
				Range range = this.textBox.GetLine(result.ErrorLine - 1);

				/*this.err = result.ErrorMessage;

				if (result.ErrorCharEnd == -1) {
					this.b = new Place(result.ErrorCharBegin, result.ErrorLine - 1);
					this.e = new Place(range.Length, result.ErrorLine - 1);
					range = this.textBox.GetRange(this.b.Value, this.e.Value);
				}
				else {
					this.b = new Place(result.ErrorCharBegin, result.ErrorLine - 1);
					this.e = new Place(result.ErrorCharEnd, result.ErrorLine - 1);
					range = this.textBox.GetRange(this.b.Value, this.e.Value);
				}
				*/
				range.SetStyle(Settings.Error);

				//this.errorTable.Rows.Add(result.ErrorType, result.ErrorMessage);
			}
		}

		private void TextBox_TextChanged(Object sender, TextChangedEventArgs e) {
			MainTextBoxManager?.OnTextChanged(e);
		}

		private void FormLoad(Object sender, EventArgs e) {
			foreach (Language i in Settings.Languages) {
				if (i.Actor != null && File.Exists(i.Actor)) {
					Lang.Scope s = new Lang.Scope();
					s.AddUsing(Lang.Prelude.Instance);
					/*  Interop.Interop op = new Interop.Interop();

                      s.Set("studio", Interop.Interop.Module);

                      s.Set(i.Name, new Lang.LambdaFun(null));*/

					Lmi.Interpriter.Start(i.Actor, s);

					i.Fn = s.Get(i.Name) as Lang.Module;
				}
			}

			Settings.Loader.LoadExtenstions();

			if (this.argument != null) {
				if (File.Exists(this.argument)) {
					ProjectManager.OpenFile(this.argument, true);
				}
				else {
					ProjectManager.OpenProject(this.argument);
					Settings.LastOpenedProject = null;
				}
			}
			else if (Settings.LastOpenedProject != null) {
				ProjectManager.OpenProject(Settings.LastOpenedProject);
			}
			else if (ProjectManager.CurrentFile == null) {
				if (!File.Exists("default.lm")) {
					File.Create("default.lm").Close();
				}

				ProjectManager.OpenFile("default.lm", true);
			}
		}

		public void CustomizeForLanguage(Language language) {
			MainTextBoxManager.Language = language;
			language.Customize();
		}

		public void UnshowAllAtWorkingArea() {
			foreach (Control control in this.splitContainer2.Panel1.Controls) {
				control.Visible = false;
			}
		}

		private void TextBox_ToolTipNeeded(Object sender, ToolTipNeededEventArgs e) {
			/*if (this.b.HasValue) {
				if (e.Place.iLine == this.b.Value.iLine) {
					if (e.Place.iChar >= this.b.Value.iChar && e.Place.iChar <= this.e.Value.iChar) {
						e.ToolTipTitle = this.err;
						e.ToolTipText = "     ";
					}
					return;
				}
			}*/

			if (this.WaitDebug) {
				Console.SetOut(new ConsoleWriter(new ConsoleEmulator()));
				if (this.textBox.SelectedText.Length > 0) {
					e.ToolTipText = Lmi.Interpriter.Eval(this.textBox.SelectedText, scope: this.currentScope).ToString();
					e.ToolTipTitle = this.textBox.SelectedText;
				}
				else if (e.HoveredWord != null) {
					e.ToolTipText = Lmi.Interpriter.Eval(e.HoveredWord, scope: this.currentScope).ToString();
					e.ToolTipTitle = e.HoveredWord;
				}
				Console.SetOut(ConsoleWriter.Instance);
			}

			/*foreach(KeyValuePair<(System.Int32 begin, System.Int32 end, System.Int32 line), IEntity> i in ScopeMetadata.symboltable) {
                if (e.Place.iLine + 1 == i.Key.line) {
                    if (e.Place.iChar >= i.Key.begin && e.Place.iChar <= i.Key.end) {
                        if(i.Value is FunctionMetadata fm) {
                            e.ToolTipTitle = BuildFunctionHeader(fm);
                        }
                        else if (i.Value is VariableMetadata vm) {
                            e.ToolTipTitle = Stringify(vm);
                        } else if (i.Value is LiteralMetadata lm) {
                            e.ToolTipTitle = lm.v;
                        }

                        e.ToolTipText = "     ";
                        return;
                    }
                }
            }*/
		}

		private void OpenToolStringipMenuItem_Click(Object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				ProjectManager.OpenFile(dialog.FileName, true);
			}
		}

		private void SaveToolStringipMenuItem_Click(Object sender, EventArgs e) {
			ProjectManager.SaveFile();
		}

		private void MainForm_FormClosed(Object sender, FormClosedEventArgs e) {
			if (ProjectManager.Project != null && ProjectManager.Project.Path != null) {
				XmlNode lastData = Settings.MainSettings.DocumentElement["LastData"];
				if (lastData != null) {
					lastData.Attributes["project"].Value = ProjectManager.Project.Path;
				}
				else {
					XmlAttribute attribute = Settings.MainSettings.CreateAttribute("project");
					attribute.Value = ProjectManager.Project.Path;
					lastData = Settings.MainSettings.CreateElement("LastData");
					lastData.Attributes.SetNamedItem(attribute);
					Settings.MainSettings.DocumentElement.AppendChild(lastData);
				}
				Settings.MainSettings.Save(Settings.ExecutablePath + "\\settings\\main.xml");
			}

			Application.Exit();
		}

		private void BottomPanelHideAll() {
			foreach (Control i in this.bottomPanel.Controls) {
				i.Visible = false;
			}
		}

		#region Project Manager 

		private void ProjectToolStringipMenuItem_Click(Object sender, EventArgs e) {
			FolderBrowserDialog ofd = new FolderBrowserDialog();

			if (ofd.ShowDialog() == DialogResult.OK) {
				ProjectManager.OpenProject(ofd.SelectedPath);
			}
		}

		internal void Fill(TreeNode node, String path) {
			if(String.IsNullOrEmpty(path)) {
				return;
			}

			foreach (String i in Directory.EnumerateDirectories(path)) {
				node.Nodes.Add(i, i.Replace(path + "\\", ""), 0, 0);
			}

			foreach (String i in Directory.EnumerateFiles(path)) {
				node.Nodes.Add(i, i.Replace(path + "\\", ""), this.GetImageIndex(i), this.GetImageIndex(i));
			}
		}

		private Int32 GetImageIndex(String i) {
			return Settings.Loader.GetImageIndex(Path.GetExtension(i));
		}

		public String GetName(String path) {
			String[] cons = path.Split('\\');

			return cons[cons.Length - 1];
		}

	/*	private void TreeView1_NodeMouseDoubleClick(Object sender, TreeNodeMouseClickEventArgs e) {
			this.treeView1.SelectedNode = null;

			if (e.Node.IsExpanded) {
				e.Node.Collapse(true);
				return;
			}

			String path = e.Node.Name;
			if (Directory.Exists(path)) {
				if (e.Node.Nodes.Count == 0) {
					this.Fill(e.Node, path);
				}

				e.Node.Expand();
				return;
			}

			if (ProjectManager.Project == null) {
				return;
			}

			ProjectManager.OpenFile(path, true);
		}*/

		#endregion

		private void TextBox_KeyDown(Object sender, KeyEventArgs e) {
			if (e.Control && e.KeyCode == Keys.S) {
				ProjectManager.SaveFile();
			}
		}

		private void CreateProjectClick(Object sender, EventArgs e) {
			new CreateProjectDialog().ShowDialog();
		}

		private void Exit(Object sender, EventArgs e) {
			if (this.WorkingThread != null && this.WorkingThread.IsAlive) {
				this.WorkingThread.Abort();
			}

			Application.Exit();
		}

		private void MaximizeMinimize(Object sender, EventArgs e) {
			if (this.WindowState == FormWindowState.Normal) {
				this.WindowState = FormWindowState.Maximized;
				this.isMaximized = true;
			}
			else {
				this.WindowState = FormWindowState.Normal;
				this.isMaximized = false;
			}
		}

		private void TextBox_MouseDoubleClick(Object sender, MouseEventArgs e) {
			if (e.X < this.textBox.LeftIndent) {
				Place place = this.textBox.PointToPlace(e.Location);
				if (e.Button == MouseButtons.Right) {
					if (this.textBox.Bookmarks.Contains(place.iLine)) {
						/*KlischeeFreeKlischeeStduio.KSDocSettings window = new KlischeeFreeKlischeeStduio.KSDocSettings();
                        window.ShowDialog();
                        Bookmark bookmark = this.textBox.Bookmarks.GetBookmark(place.iLine);
                        bookmark.Condition = window.condition;
                        bookmark.Action = window.action;*/
					}
					return;
				}

				if (this.textBox.Bookmarks.Contains(place.iLine)) {
					this.textBox.Bookmarks.Remove(place.iLine);
					this.textBox.GetLine(place.iLine).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.String, Settings.Keyword);
					MainTextBoxManager.TextBox.OnTextChanged(this.textBox.Range);
				}
				else {
					this.textBox.Bookmarks.Add(new Bookmark(MainTextBoxManager.TextBox, "", place.iLine/*, Settings.TextBoxBookMarkColor*/));
					this.textBox.GetLine(place.iLine).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.String, Settings.Keyword);
					this.textBox.GetLine(place.iLine).SetStyle(Settings.UnactiveBreakpoint);
				}
			}
		}

		public void MakeControlActiveInBottom(Control control) {
			if (ActiveBottomControl == control) {
				return;
			}

			if(ActiveBottomControl != null) {
				ActiveBottomControl.Visible = false;
			}

			control.Dock = DockStyle.Fill;
			control.Visible = true;
			ActiveBottomControl = control;
		}

		private void OutputToolStripMenuItem_Click(Object sender, EventArgs e) {
			this.MakeControlActiveInBottom(this.Output);
		}

		private void ShowInteractive(Object sender, EventArgs e) {
			//this.BottomPanelHideAll();
			if (this.interactive == null) {
				this.IntializeInteractive();
			}

			this.MakeControlActiveInBottom(this.interactive);
		}

		public void AddControlToWorkingArea(Control control) {
			this.splitContainer2.Panel1.Controls.Add(control);
		}

		public void RemoveControlFromWorkingArea(Control control) {
			this.splitContainer2.Panel1.Controls.Remove(control);
		}

		public void MakeControlActiveInWorkingArea(Control control) {
			if (!this.splitContainer2.Panel1.Contains(control)) {
				this.AddControlToWorkingArea(control);
			}

			this.UnshowAllAtWorkingArea();
			ActiveWorkingAreaControl = control;
			control.Visible = true;
		}

		TextBoxManager tbmng;
		ConsoleEmulator interactive;
		private void IntializeInteractive() {
			this.interactive = new ConsoleEmulator();
			this.bottomPanel.Controls.Add(this.interactive);
			this.interactive.Font = new Font("Consolas", 9f);
			this.interactive.ShowLineNumbers = false;
			this.interactive.BackColor = Settings.BackgroundColor;
			this.interactive.ForeColor = Settings.ForegroundColor;
			this.interactive.Size = this.bottomPanel.Size;
			this.interactive.Visible = true;

			this.interactive.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			this.tbmng = new TextBoxManager(this.interactive) {
				Language = Settings.Languages[0]
			};

			this.tbmng.Menu.Renderer = new LumenMenuRenderer();
			if (this.tbmng.Menu.Renderer is LumenMenuRenderer rendererq) {
				rendererq.ArrowColor = Settings.AutocompleteMenu.ArrowColor;
				rendererq.BackColor = Settings.AutocompleteMenu.Background;
				rendererq.BorderColor = Settings.AutocompleteMenu.BordersColor;
				rendererq.ForeColor = Settings.AutocompleteMenu.ForegroundColor;
				rendererq.SelectedBackColor = Settings.AutocompleteMenu.SelectedBackground;

				this.tbmng.Menu.listView.HoveredColor = Settings.AutocompleteMenu.SelectedBackground;
				this.tbmng.Menu.listView.SelectedColor = Settings.AutocompleteMenu.SelectedBackground;

				// this.tbmng.Menu.listView.ToolTipBackgroundColor = Settings.AutocompleteMenu.ToolTipBackColor;
				// this.tbmng.Menu.listView.ToolTipForegroundColor = Settings.AutocompleteMenu.ToolTipForeColor;
			}

			this.tbmng.Menu.BackColor = Settings.AutocompleteMenu.Background;
			this.tbmng.Menu.ForeColor = Settings.AutocompleteMenu.ForegroundColor;
			// this.tbmng.Menu.BorderColor = Settings.AutocompleteMenu.BordersColor;

			this.interactive.TextChanged += (sender, e) => {
				Settings.Languages[0].OnTextChanged(this.tbmng, e.ChangedRange);
			};

			Task.Factory.StartNew(() => {
				Lang.Scope mainScope = new Lang.Scope();

				ConsoleWriter localWriter = new ConsoleWriter(this.interactive);
				ConsoleReader localReader = new ConsoleReader(this.interactive);

				while (true) {
					this.interactive.Write("> ");
					String command = this.interactive.ReadLine();
					while (!command.TrimEnd().EndsWith(";;")) {
						this.interactive.Write(". ");
						command += Environment.NewLine + this.interactive.ReadLine();
					}

					Console.SetIn(localReader);
					Console.SetOut(localWriter);
					Lang.Value result = Lmi.Interpriter.Eval(command.TrimEnd(new System.Char[] { ' ', '\t', '\r', '\n', ';' }), "interactive", mainScope);
					mainScope.Bind("ans", result);
					Console.WriteLine($"\t{result} :: {result.Type}");
					Console.SetIn(ConsoleReader.Instance);
					Console.SetOut(ConsoleWriter.Instance);
				}
			});
		}

		/* Edit submenu methods */
		private void MakeShowIEditable() {
			this.TopMenuUndo.Visible = true;
			this.TopMenuRedo.Visible = true;
			this.TopMenuEditSplitter.Visible = true;
			this.TopMenuCopy.Visible = true;
			this.TopMenuPaste.Visible = true;
			this.TopMenuCut.Visible = true;
			this.TopMenuDelete.Visible = true;
		}

		private void TopMenuEdit_DropDownOpening(Object sender, EventArgs e) {
			this.MakeShowIEditable();

			if (ActiveWorkingAreaControl is FastColoredTextBox) {
				return;
			}

			if (!(ActiveWorkingAreaControl is IUndoable)) {
				this.TopMenuUndo.Visible = false;
			}

			if (!(ActiveWorkingAreaControl is IRedoable)) {
				this.TopMenuRedo.Visible = false;
			}

			if (!(this.TopMenuUndo.Visible && this.TopMenuRedo.Visible)) {
				this.TopMenuEditSplitter.Visible = false;
			}

			if (!(ActiveWorkingAreaControl is ICopyable)) {
				this.TopMenuCopy.Visible = false;
			}

			if (!(ActiveWorkingAreaControl is IPasteable)) {
				this.TopMenuPaste.Visible = false;
			}

			if (!(ActiveWorkingAreaControl is ICutable)) {
				this.TopMenuCut.Visible = false;
			}

			if (!(ActiveWorkingAreaControl is IDeleteable)) {
				this.TopMenuDelete.Visible = false;
			}
		}

		private void TopMenuUndo_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.Undo();
			}
			else if (ActiveWorkingAreaControl is IUndoable undonable) {
				undonable.Undo();
			}
		}

		private void TopMenuRedo_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.Redo();
			}
			else if (ActiveWorkingAreaControl is IRedoable redoable) {
				redoable.Redo();
			}
		}

		private void TopMenuCopy_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.Copy();
			}
			else if (ActiveWorkingAreaControl is ICopyable copyable) {
				copyable.Copy();
			}
		}

		private void TopMenuPaste_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.Paste();
			}
			else if (ActiveWorkingAreaControl is IPasteable pasteable) {
				pasteable.Paste();
			}
		}

		private void TopMenuCut_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.Cut();
			}
			else if (ActiveWorkingAreaControl is ICutable cutable) {
				cutable.Cut();
			}
		}

		private void TopMenuDelete_Click(Object sender, EventArgs e) {
			if (ActiveWorkingAreaControl is FastColoredTextBox textBox) {
				textBox.SelectedText = "";
			}
			else if (ActiveWorkingAreaControl is IDeleteable deleteable) {
				deleteable.Delete();
			}
		}

		private void MainForm_Shown(Object sender, EventArgs e) {

		}

		private void TextBox_SelectionChanged(Object sender, EventArgs e) {
			MainTextBoxManager.Language.OnSelectedChanged(MainTextBoxManager);
		}

		private void StudioHelpToolStripMenuItem_Click(Object sender, EventArgs e) {

		}
	}

	public class DebugGrid : GridView {
		DataGridViewTextBoxColumn imageDebugColumn = new DataGridViewTextBoxColumn {
			AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
			Frozen = true,
			HeaderText = "",
			ReadOnly = true,
			Width = 16
		};

		DataGridViewTextBoxColumn nameDebugColumn = new DataGridViewTextBoxColumn {
			AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
			Frozen = true,
			HeaderText = "Name",
			ReadOnly = true,
		};

		DataGridViewTextBoxColumn valueDebugColumn = new DataGridViewTextBoxColumn {
			AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
			HeaderText = "Value",
			ReadOnly = true
		};

		public DebugGrid() {
			this.Columns.Add(this.imageDebugColumn);
			this.Columns.Add(this.nameDebugColumn);
			this.Columns.Add(this.valueDebugColumn);
			this.ScrollBars = ScrollBars.Both;
		}
	}

	public class LumenDebugger {

	}

	class DataCell : DataGridViewTextBoxCell {
		public Object __value;

		public DataCell(Object val) {
			this.__value = val;
		}
	}

	public class GridView : DataGridView {
		public static DataGridViewCellStyle CellStyle { get; set; } = new DataGridViewCellStyle {
			Alignment = DataGridViewContentAlignment.MiddleCenter,
			Font = Settings.DefaultInterfaceFont,
			ForeColor = Settings.ForegroundColor,
			BackColor = Settings.BackgroundColor,
			SelectionBackColor = Settings.BackgroundColor,
			SelectionForeColor = Settings.ForegroundColor,
		};

		public GridView() {
			this.BackgroundColor = Settings.BackgroundColor;
			this.GridColor = Settings.LinesColor;
			this.DataError += delegate { };
			this.BackColor = Settings.BackgroundColor;
			this.EnableHeadersVisualStyles = false;
			this.ColumnHeadersDefaultCellStyle = CellStyle;
			this.AlternatingRowsDefaultCellStyle = CellStyle;
			this.RowsDefaultCellStyle = CellStyle;

			this.AllowUserToAddRows = false;
			this.AllowUserToDeleteRows = false;
			this.AllowUserToResizeRows = false;

			this.Dock = DockStyle.Fill;
			this.Anchor = (((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right);
			this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			this.BorderStyle = BorderStyle.None;
			this.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.Cursor = Cursors.Arrow;
			this.Location = new Point(0, 0);
			this.MultiSelect = false;
			this.RightToLeft = RightToLeft.No;
			this.RowHeadersVisible = false;
			this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
			this.RowTemplate.ReadOnly = true;
			this.RowTemplate.Resizable = DataGridViewTriState.True;
			this.ShowCellErrors = false;
			this.ShowEditingIcon = false;
			this.ShowRowErrors = false;
			this.Size = new Size(449, 126);
			this.TabIndex = 0;
		}
	}
}