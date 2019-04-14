using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public sealed partial class MainForm : LumenStudioForm /*/ Form/**/ {
		/// <summary> Singleton realization  </summary>
		public static MainForm Instance { get; private set; }

		private static TaskFactory Factory { get; } = new TaskFactory();
		public static TextBoxManager MainTextBoxManager { get; set; }
		public static PictureBox MainPictureBox { get; } = new CopyableImageBox();

		public static Control ActiveWorkingAreaControl { get; private set; }
		public static Control ActiveBottomControl { get; private set; }

		public String err;
		public Place? b;
		public Place? e;

		private readonly String argument;

		internal MainForm(String[] args) : base() {
			this.InitializeComponent();
			this.StartPosition = FormStartPosition.CenterScreen;
			Instance = this;

			this.Opacity = 0;

			Timer t = new Timer {
				Interval = 10,
				Enabled = true
			};

			t.Tick += (sender, e) => {
				this.Opacity += 0.1;
				if(this.Opacity >= 1) {
					t.Enabled = false;
				}
			};

			this.textBox.ContextMenuStrip = this.TextBoxContextMenu;
			MainTextBoxManager = new TextBoxManager(this.textBox) {
				Language = Settings.Languages[0]
			};

			// Make it into extension
			ToolStripItem xmlize = new ToolStripButton("Xmlize");
			xmlize.Click += (sender, e) => {
				this.textBox.SelectedText = this.textBox.SelectedText.Replace("&", "&amp;")
				.Replace("<", "&lt;").Replace(">", "&gt;")
				.Replace("\"", "&quot;");
			};
			ToolStripItem dexmlize = new ToolStripButton("Dexmlize");
			dexmlize.Click += (sender, e) => {
				this.textBox.SelectedText = this.textBox.SelectedText
				.Replace("&lt;", "<").Replace("&gt;", ">")
				.Replace("&quot;", "\"").Replace("&amp;", "&");
			};
			this.TextBoxContextMenu.Items.Add(xmlize);
			this.TextBoxContextMenu.Items.Add(dexmlize);

			// Solve a problem
			this.TextBoxContextMenu.Opening += (sender, e) => {
				if (!this.textBox.Selection.IsEmpty) {
					xmlize.Visible = true;
					dexmlize.Visible = true;
				}
			};
			this.TextBoxContextMenu.Closed += (sender, e) => {
				xmlize.Visible = false;
				dexmlize.Visible = false;
			};

			//*
			this.headerPanel.MouseDown += new MouseEventHandler(this.FormMouseDown);
			this.headerPanel.MouseMove += new MouseEventHandler(this.FormMouseMove);
			this.headerPanel.MouseUp += new MouseEventHandler(this.FormMouseUp);
			//*/

			this.treeView1.BeforeSelect += (sender, e) => e.Cancel = true;

			ConsoleWriter.Instance = new ConsoleWriter(this.Output);
			ConsoleReader.Instance = new ConsoleReader(this.Output);

			Console.SetOut(ConsoleWriter.Instance);
			Console.SetIn(ConsoleReader.Instance);

			this.splitContainer2.Panel1.Controls.Add(MainPictureBox);

			ActiveBottomControl = this.Output;
			this.MainMenuStrip.Renderer = new LumenMenuRenderer();
			this.BottomMenu.Renderer = new LumenMenuRenderer();

			this.ApplyColorScheme();

			if (args.Length > 0) {
				this.argument = args[0];
			}

			this.Output.Location = new Point(0, this.Output.Width);

			this.MakeControlActiveInWorkingArea(this.textBox);
			this.Output.Location = new Point(0, 0);
		}


		#region ApplyTheme
		public void ApplyColorScheme() {
			LoadImages();
			this.treeView1.HideSelection = false;

			this.Font = Settings.DefaultInterfaceFont;

			this.Output.Font = new Font("Consolas", 9f);
			this.Output.Dock = DockStyle.None;
			this.Output.Size = this.bottomPanel.Size;
			this.Output.Location = new Point(0, this.bottomPanel.Height);
			this.Output.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

			this.treeView1.Font = Settings.DefaultInterfaceFont;

			this.splitContainer2.SplitterWidth = 1;

			this.headerPanel.BackColor = Settings.BackgroundColor;

			this.textBox.BackColor = Settings.BackgroundColor;
			this.textBox.ForeColor = Settings.ForegroundColor;
			this.textBox.IndentBackColor = Settings.BackgroundColor;
			this.textBox.LineNumberColor = Settings.ForegroundColor;
			this.textBox.PaddingBackColor = Settings.BackgroundColor;
			this.textBox.TextAreaBorderColor = Settings.BackgroundColor;
			this.textBox.ServiceLinesColor = Settings.BackgroundColor;
			this.treeView1.BackColor = Settings.BackgroundColor;
			this.treeView1.ForeColor = Settings.ForegroundColor;

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

			this.ColorizeTopMenu();
			this.ColorizeBottomMenu();
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

		private void ColorizeTopMenu() {
			foreach (Object i in this.TopMenu.Items) {
				if (i is ToolStripMenuItem item) {
					this.ColorizeMenuItem(item);
				}
			}
		}

		private void ColorizeBottomMenu() {
			foreach (Object i in this.BottomMenu.Items) {
				if (i is ToolStripMenuItem item) {
					this.ColorizeMenuItem(item);
				}
			}
		}

		private void ColorizeMenuItem(ToolStripMenuItem item) {
			item.BackColor = Settings.BackgroundColor;
			item.ForeColor = Settings.ForegroundColor;

			if (item.HasDropDownItems) {
				foreach (Object i in item.DropDownItems) {
					if (i is ToolStripMenuItem mi) {
						this.ColorizeMenuItem(mi);
					}
				}
			}
		}
		#endregion

		private void Run(Object sender, EventArgs e) {
			this.Output.Clear();

			if (MainTextBoxManager.Language != null) {
				if (MainTextBoxManager.Language.Build != null) {
					ProjectManager.SaveFile();
					this.StartProcess(MainTextBoxManager.Language.Build.Replace("[FILE]", ProjectManager.CurrentFile));
				}
				else {
					ProjectManager.SaveFile();
					Task.Factory.StartNew(() => Light.Interpriter.Start(ProjectManager.CurrentFile, new Lang.Scope()));
				}
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

			Factory.StartNew(() => {
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

		internal void RaiseError(IError result) {
			if (result.ErrorLine > 0) {
				Range range = this.textBox.GetLine(result.ErrorLine - 1);

				this.err = result.ErrorMessage;

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

				range.SetStyle(Settings.Error);

				//this.errorTable.Rows.Add(result.ErrorType, result.ErrorMessage);
			}
		}

		private void TextBox_TextChanged(Object sender, TextChangedEventArgs e) {
			this.err = null;
			this.b = null;
			// this.errorTable.Rows.Clear();

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

					Light.Interpriter.Start(i.Actor, s);

					i.Fn = s.Get(i.Name) as Lang.Module;
				}
			}

			if (argument != null) {
				if (File.Exists(argument)) {
					ProjectManager.OpenFile(argument, true);
				}
				else {
					ProjectManager.OpenProject(argument);
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
		}

		public void UnshowAllAtWorkingArea() {
			foreach (Control control in this.splitContainer2.Panel1.Controls) {
				control.Visible = false;
			}
		}

		private void TextBox_ToolTipNeeded(Object sender, ToolTipNeededEventArgs e) {
			if (this.b.HasValue) {
				if (e.Place.iLine == this.b.Value.iLine) {
					if (e.Place.iChar >= this.b.Value.iChar && e.Place.iChar <= this.e.Value.iChar) {
						e.ToolTipTitle = this.err;
						e.ToolTipText = "     ";
					}
					return;
				}
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
			foreach (String i in Directory.EnumerateDirectories(path)) {
				TreeNode n = node.Nodes.Add(i, i.Replace(path + "\\", ""), 0, 0);
				this.Fill(n, i);
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

		private void TreeView1_NodeMouseDoubleClick(Object sender, TreeNodeMouseClickEventArgs e) {
			String path = e.Node.Name;
			this.treeView1.SelectedNode = null;

			if (ProjectManager.Project == null) {
				return;
			}

			ProjectManager.OpenFile(path, true);
		}

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
					this.textBox.Bookmarks.Add(new Bookmark(MainTextBoxManager.TextBox, "", place.iLine, Settings.TextBoxBookMarkColor));
					this.textBox.GetLine(place.iLine).ClearStyle(Settings.UnactiveBreakpoint, Settings.Type, Settings.String, Settings.Keyword);
					this.textBox.GetLine(place.iLine).SetStyle(Settings.UnactiveBreakpoint);
				}
			}
		}

		private Boolean direction = true;

		public void MakeControlActiveInBottom(Control control) {
			if(ActiveBottomControl == control) {
				return;
			}

			if(direction) {
				control.Location = new Point(0, -this.bottomPanel.Height);
			}
			else {
				control.Location = new Point(0, this.bottomPanel.Height);
			}

			Int32 index = this.bottomPanel.Height / 10;

			Timer t = new Timer {
				Interval = 1,
				Enabled = true
			};

			t.Tick += (sender, e) => {
				// down
				if (direction && control.Location.Y < 0) {
					// move down
					control.Location = new Point(0, control.Location.Y + index > 0 ? 0 : control.Location.Y + index);
					ActiveBottomControl.Location = new Point(0, ActiveBottomControl.Location.Y + index);
				} else if (!direction && control.Location.Y > 0) {
					control.Location = new Point(0, control.Location.Y - index < 0 ? 0 : control.Location.Y - index);
					ActiveBottomControl.Location = new Point(0, ActiveBottomControl.Location.Y - index);
				}
				else {
					t.Enabled = false;
					ActiveBottomControl = control;
					direction = !direction;
				}
			};
		}

		private void OutputToolStripMenuItem_Click(Object sender, EventArgs e) {
			MakeControlActiveInBottom(Output);
		}

		private void ShowInteractive(Object sender, EventArgs e) {
			//this.BottomPanelHideAll();
			if (this.interactive == null) {
				this.IntializeInteractive();
			}

			MakeControlActiveInBottom(interactive);
		}

		public void AddControlToWorkingArea(Control control) {
			this.splitContainer2.Panel1.Controls.Add(control);
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

			this.interactive.TextChanged += (sender, e) => {
				Settings.Languages[0].OnTextChanged(this.tbmng, e.ChangedRange);
			};

			Task.Factory.StartNew(() => {
				Lang.Scope mainScope = new Lang.Scope();

				ConsoleWriter localWriter = new ConsoleWriter(this.interactive);
				ConsoleReader localReader = new ConsoleReader(this.interactive);

				while (true) {
					this.interactive.Write(">>> ");
					String command = this.interactive.ReadLine();
					while (!command.TrimEnd().EndsWith(";;")) {
						this.interactive.Write("... ");
						command += Environment.NewLine + this.interactive.ReadLine();
					}

					Console.SetIn(localReader);
					Console.SetOut(localWriter);
					Lang.Value result = Light.Interpriter.Eval(command.TrimEnd(new System.Char[] { ' ', '\t', '\r', '\n', ';' }), "interactive", mainScope);
					Console.WriteLine($"//-> {result} :: {result.Type}");
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
	}
}