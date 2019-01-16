using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public partial class MainForm : Form {
		/// <summary> Singleton realization  </summary>
		public static MainForm Instance { get; private set; }

		private static TaskFactory Factory { get; } = new TaskFactory();

		public static TextBoxManager MainTextBoxManager { get; set; }

		public static Project Project { get; set; }
		public static String CurrentFile { get; set; }

		public String err;
		public Place? b;
		public Place? e;

		public static Boolean AllowRegistrationChangas = true;
		public Boolean IsCmdInitialized { get; private set; } = false;

		public MainForm(String[] args) {
			InitializeComponent();

			Instance = this;

			MainTextBoxManager = new TextBoxManager(this.textBox) {
				Language = Settings.Languages[0]
			};

			this.splitContainer2.SplitterWidth = 1;
			this.splitContainer3.SplitterWidth = 1;

			ConsoleWriter.Instance = new ConsoleWriter(this.output);
			ConsoleReader.Instance = new ConsoleReader(this.output);

			Console.SetOut(ConsoleWriter.Instance);
			Console.SetIn(ConsoleReader.Instance);

			this.textBox.BackColor = Settings.BackgroundColor;
			this.textBox.ForeColor = Settings.ForegroundColor;
			this.textBox.IndentBackColor = Settings.BackgroundColor;
			this.textBox.LineNumberColor = Settings.ForegroundColor;
			this.textBox.PaddingBackColor = Settings.BackgroundColor;
			this.textBox.TextAreaBorderColor = Settings.BackgroundColor;
			this.textBox.ServiceLinesColor = Settings.BackgroundColor;
			this.treeView1.BackColor = Settings.BackgroundColor;
			this.treeView1.ForeColor = Settings.ForegroundColor;

			this.menuStrip1.BackColor = Settings.BackgroundColor;
			this.BackColor = Settings.LinesColor;
			this.output.BackColor = Settings.BackgroundColor;
			this.output.ForeColor = Settings.ForegroundColor;
			this.errorTable.ForeColor = Settings.ForegroundColor;
			this.errorTable.GridColor = Settings.LinesColor;
			this.errorTable.BackgroundColor = Settings.BackgroundColor;
			this.splitContainer1.BackColor = Settings.LinesColor;
			this.splitContainer2.BackColor = Settings.LinesColor;
			this.splitContainer3.BackColor = Settings.LinesColor;
			//this.textBox.Font = new Font("Courier New", 10);

			ColorizeTopMenu();

			if (args.Length > 0) {
				OpenFile(args[0]);
			}
		}

		internal void HighlightUnsavedFile() {
			TreeNode[] nodes = this.treeView1.Nodes.Find(CurrentFile, true);

			if (nodes.Length >= 1) {
				TreeNode node = nodes[0];
				node.ForeColor = Color.DarkRed;
			}
		}

		private void ColorizeTopMenu() {
			foreach (Object i in this.menuStrip1.Items) {
				if (i is ToolStripMenuItem item) {
					ColorizeMenuItem(item);
				}
			}
		}

		private void ColorizeMenuItem(ToolStripMenuItem item) {
			item.BackColor = Settings.BackgroundColor;
			item.ForeColor = Settings.ForegroundColor;

			if (item.HasDropDownItems) {
				foreach (Object i in item.DropDownItems) {
					if (i is ToolStripMenuItem mi) {
						ColorizeMenuItem(mi);
					}
				}
			}
		}

		private void Run(Object sender, EventArgs e) {
			this.output.Clear();
			this.errorTable.Rows.Clear();

			if (MainTextBoxManager.Language != null) {
				if (MainTextBoxManager.Language.RunCommand != null) {
					SavePreviousFile();
					StartProcess(MainTextBoxManager.Language.RunCommand.Replace("[FILE]", CurrentFile));
				}
				else {
					SavePreviousFile();
					Stereotype.Interpriter.Start(CurrentFile);
				}
			}
		}

		private void StartProcess(String name) {
			Tuple<String, String> nameAndArgs = GetNameAndArgs(name);

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

				this.errorTable.Rows.Add(result.ErrorType, result.ErrorMessage);
			}
		}

		private void TextBox_TextChanged(Object sender, TextChangedEventArgs e) {
			this.err = null;
			this.b = null;
			this.errorTable.Rows.Clear();

			MainTextBoxManager?.OnTextChanged(e);
		}

		private void Form1_Load(Object sender, EventArgs e) {
			foreach (Language i in Settings.Languages) {
				if (i.Actor != null) {
					Lang.Std.Scope s = new Lang.Std.Scope();
					s.AddUsing(Lang.Std.StandartModule.__Kernel__);
					Interop.Interop op = new Interop.Interop();

					s.Set("studio", Interop.Interop.Module);

					s.Set(i.Name, new Lang.Std.LambdaFun(null));

					Stereotype.Interpriter.Start(i.Actor, s);

					i.Fn = s.Get(i.Name) as Lang.Std.Fun;
				}
			}

			if (CurrentFile == null) {
				if (!File.Exists("default.lm")) {
					File.Create("default.lm").Close();
				}

				OpenFile("default.lm");
			}
		}

		#region File System

		private void OpenFile(String path) {
			if (File.Exists(path)) {
				if (path.EndsWith(".png")) {
					OpenImage(path);
					return;
				}

				SavePreviousFile();

				ProcessExtenstion(Path.GetExtension(path));
				AllowRegistrationChangas = false;

				MainTextBoxManager.TextBox.Text = File.ReadAllText(path);

				AllowRegistrationChangas = true;

				CurrentFile = path;

				if (Project != null) {
					this.Text = path.Replace(Project.Path + "\\", "") + " - Lumen Studio";
				}
			}
		}

		private void FileOpenWithoutSave(String path) {
			if (File.Exists(path)) {
				if (path.EndsWith(".png")) {
					OpenImage(path);
					return;
				}

				ProcessExtenstion(Path.GetExtension(path));

				AllowRegistrationChangas = false;

				MainTextBoxManager.TextBox.Text = File.ReadAllText(path);

				AllowRegistrationChangas = true;

				CurrentFile = path;

				this.Text = path.Replace(Project.Path + "\\", "") + " - Lumen Studio";
			}
		}

		private void ProcessExtenstion(String extension) {
			foreach (Language language in Settings.Languages) {
				if (language.Extensions.Contains(extension)) {
					CustomizeForLanguage(language);
					return;
				}
			}
		}

		private void CustomizeForLanguage(Language language) {
			MainTextBoxManager.Language = language;
		}

		private void SavePreviousFile() {
			if (CurrentFile != null) {
				File.WriteAllText(CurrentFile, MainTextBoxManager.TextBox.Text);
				MainTextBoxManager.ChangesSaved = true;
				TreeNode[] nodes = this.treeView1.Nodes.Find(CurrentFile, true);

				if (nodes.Length == 1) {
					TreeNode node = nodes[0];
					node.ForeColor = Settings.ForegroundColor;
				}
			}
		}

		public void DeleteRecursive(String dir) {
			foreach (String d in Directory.EnumerateDirectories(dir)) {
				DeleteRecursive(d);
			}

			foreach (String f in Directory.EnumerateFiles(dir)) {
				File.Delete(f);
			}

			Directory.Delete(dir);
		}

		public void FileWriteWithCreating(String path) {
			String[] s = path.Split('\\');

			for (Int32 i = 1; i < s.Length - 1; i++) {
				String p = "";
				for (Int32 j = 0; j <= i; j++) {
					p += s[j] + "\\";
				}

				if (!Directory.Exists(p)) {
					Directory.CreateDirectory(p);
				}
			}

			File.WriteAllText(path, MainTextBoxManager.TextBox.Text);
		}

		#endregion

		private void OpenImage(String path) {
			this.textBox.Visible = false;

			PictureBox z = new PictureBox {
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.None,
				ImageLocation = path,
				WaitOnLoad = false
			};

			this.splitContainer2.Panel1.Controls.Add(z);
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

		private void OpenToolStripMenuItem_Click(Object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				OpenFile(dialog.FileName);
			}
		}

		private void SaveToolStripMenuItem_Click(Object sender, EventArgs e) {
			SavePreviousFile();
		}

		private void MainForm_FormClosed(Object sender, FormClosedEventArgs e) {
			if (Directory.Exists(".tmp")) {
				DeleteRecursive(".tmp");
			}

			Application.Exit();
		}

		#region Cmd 

		private void CmdButtonClick(Object sender, EventArgs e) {
			if (!this.IsCmdInitialized) {
				CmdInit();
			}

			CmdInterface.DoInterfaceVisible();
		}

		private static class CmdInterface {
			public static FastColoredTextBox CmdOutput { get; private set; }
			public static ConsoleEmulator CmdInput { get; private set; }
			public static Process GlobalCmdProcess { get; private set; }
			private static volatile Boolean isWriteMode;

			public static void InitializeGui() {
				CmdInput = new ConsoleEmulator {
					Dock = DockStyle.Bottom,
					BackColor = Settings.BackgroundColor,
					ForeColor = Settings.ForegroundColor,
					ShowLineNumbers = false,
					Height = 20,
					Font = new Font("Consolas", 10f),
					ShowScrollBars = false
				};

				CmdOutput = new FastColoredTextBox {
					ForeColor = Settings.BackgroundColor,
					BackColor = Settings.ForegroundColor,
					Dock = DockStyle.Fill,
					ShowLineNumbers = false,
					Font = new Font("Consolas", 10f),
					PreferredLineWidth = 10
				};

				Instance.splitContainer3.Panel2.Controls.Add(CmdInterface.CmdInput);
				Instance.splitContainer3.Panel2.Controls.Add(CmdInterface.CmdOutput);
			}

			public static void InitializeProcess() {
				GlobalCmdProcess = new Process {
					StartInfo = new ProcessStartInfo {
						Arguments = "",
						CreateNoWindow = true,
						UseShellExecute = false,
						FileName = Settings.CmdPath,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						StandardOutputEncoding = Encoding.GetEncoding(Settings.CmdCodePage),
						StandardErrorEncoding = Encoding.GetEncoding(Settings.CmdCodePage)
					}
				};

				GlobalCmdProcess.OutputDataReceived += (sender, e) => {
					if (e.Data == null) {
						return;
					}

					isWriteMode = true;
					CmdOutput.AppendText(Convert(e.Data) + Environment.NewLine);
					isWriteMode = false;
				};

				GlobalCmdProcess.ErrorDataReceived += (sender, e) => {
					if (e.Data == null) {
						return;
					}

					isWriteMode = true;
					CmdOutput.AppendText(Convert(e.Data) + Environment.NewLine);
					isWriteMode = false;
				};
			}

			private static String Convert(String str) {
				Byte[] bytes = Encoding.Convert(
					GlobalCmdProcess.StartInfo.StandardOutputEncoding,
					Encoding.Unicode,
					GlobalCmdProcess.StartInfo.StandardOutputEncoding.GetBytes(str));

				return Encoding.Unicode.GetString(bytes);
			}

			public static void RunProcess() {
				GlobalCmdProcess.Start();

				GlobalCmdProcess.StandardInput.WriteLine("chcp " + Settings.CmdCodePage);

				GlobalCmdProcess.BeginOutputReadLine();

				Boolean IsFirstCommand = true;
				while (!GlobalCmdProcess.WaitForExit(300)) {
					if (!isWriteMode) {
						if (IsFirstCommand) {
							CmdOutput.Clear();
							IsFirstCommand = false;
						}
						GlobalCmdProcess.StandardInput.WriteLine(CmdInput.ReadLine());
						CmdInput.Clear();
					}
				}
			}

			public static void DoInterfaceVisible() {
				Instance.BottomPanelHideAll();

				CmdOutput.Visible = true;
				CmdInput.Visible = true;
			}
		}

		private void BottomPanelHideAll() {
			foreach (Control i in this.splitContainer3.Panel2.Controls) {
				i.Visible = false;
			}
		}

		private void CmdInit() {
			CmdInterface.InitializeGui();

			BottomPanelHideAll();

			CmdInterface.InitializeProcess();

			Factory.StartNew(CmdInterface.RunProcess);

			this.IsCmdInitialized = true;
		}

		#endregion

		#region Project Manager 

		private void ProjectToolStripMenuItem_Click(Object sender, EventArgs e) {
			FolderBrowserDialog ofd = new FolderBrowserDialog();

			if (ofd.ShowDialog() == DialogResult.OK) {
				OpenProject(ofd.SelectedPath);
			}
		}

		private void OpenProject(String selectedPath) {
			Project = new Project {
				Name = GetName(selectedPath),
				Path = selectedPath
			};

			Fill(this.treeView1.Nodes.Add(Project.Name), Path.GetFullPath(selectedPath));
		}

		private void Fill(TreeNode node, String path) {
			foreach (String i in Directory.EnumerateDirectories(path)) {
				TreeNode n = node.Nodes.Add(i, i.Replace(path + "\\", ""), 0);
				Fill(n, i);
			}

			foreach (String i in Directory.EnumerateFiles(path)) {
				node.Nodes.Add(i, i.Replace(path + "\\", ""), GetImageIndex(i));
			}
		}

		private Int32 GetImageIndex(String i) {
			if (i.EndsWith(".html")) {
				return 2;
			}

			if (i.EndsWith(".py")) {
				return 3;
			}

			if (i.EndsWith(".lm")) {
				return 4;
			}

			if (i.EndsWith(".css")) {
				return 5;
			}
			if (i.EndsWith(".png")) {
				return 6;
			}


			/*
			if (i.EndsWith(".txt")) {
				return 4;
			}

			if (i.EndsWith(".cs")) {
				return 2;
			}
			*/
			return 1;
		}

		private String GetName(String path) {
			String[] cons = path.Split('\\');

			return cons[cons.Length - 1];
		}

		private void TreeView1_NodeMouseDoubleClick(Object sender, TreeNodeMouseClickEventArgs e) {
			if (Project == null) {
				return;
			}

			if (!Directory.Exists(".tmp")) {
				Directory.CreateDirectory(".tmp");
			}

			if (CurrentFile != null) {
				if (!MainTextBoxManager.ChangesSaved) {
					FileWriteWithCreating(".tmp" + CurrentFile.Replace(Project.Path, ""));
				}
			}

			String path = e.Node.Name;

			if (File.Exists(".tmp\\" + path.Replace(Project.Path, ""))) {
				FileOpenWithoutSave(".tmp\\" + path.Replace(Project.Path, ""));
				CurrentFile = path;
			}
			else {
				FileOpenWithoutSave(path);
			}
		}

		#endregion
	}
}
