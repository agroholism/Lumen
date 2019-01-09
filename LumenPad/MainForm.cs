using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

using FastColoredTextBoxNS;

namespace Lumen.Studio {
	public partial class MainForm : Form {
		/// <summary> Singleton realization  </summary>
		public static MainForm Instance { get; private set; }

		public static TextBoxManager MainTextBoxManager { get; set; }

		public static Project Project { get; set; }
		public static String CurrentFile { get; set; }

		public String err;
		public Place? b;
		public Place? e;

		public MainForm(String[] args) {
			InitializeComponent();

			Instance = this;

			MainTextBoxManager = new TextBoxManager(this.textBox) {
				Language = Settings.Languages[0]
			};

			this.fastColoredTextBox1.ShowScrollBars = false;
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

			this.menuStrip1.BackColor = Settings.BackgroundColor;
			this.BackColor = Settings.LinesColor;
			this.splitContainer2.Panel2Collapsed = true;
			//this.splitContainer3.Panel2Collapsed = true;
			this.output.BackColor = Settings.BackgroundColor;
			this.output.ForeColor = Settings.ForegroundColor;
			this.errorTable.ForeColor = Settings.ForegroundColor;
			this.errorTable.GridColor = Settings.LinesColor;
			this.errorTable.BackgroundColor = Settings.BackgroundColor;
			this.splitContainer1.BackColor = Settings.LinesColor;
			this.splitContainer2.BackColor = Settings.LinesColor;
			this.splitContainer3.BackColor = Settings.LinesColor;
			//this.textBox.Font = new Font("Courier New", 10);

			ColorizeMenu();

			if (args.Length > 0) {
				LoadFile(args[0]);
			}
		}

		private void ColorizeMenu() {
			foreach (Object i in this.menuStrip1.Items) {
				if (i is ToolStripMenuItem item) {
					ColorizeItem(item);
				}
			}
		}

		private void ColorizeItem(ToolStripMenuItem item) {
			item.BackColor = Settings.BackgroundColor;
			item.ForeColor = Settings.ForegroundColor;

			if (item.HasDropDownItems) {
				foreach (Object i in item.DropDownItems) {
					if (i is ToolStripMenuItem mi) {
						ColorizeItem(mi);
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
					/*TcpClient cli = new TcpClient("127.0.0.1", 4444);

					var z = cli.GetStream();
					while (z.CanRead) {
						byte[] bytes = new byte[1024];
						z.Read(bytes, 0, 1024);
						output.Write(Encoding.UTF8.GetString(bytes, 0, 1024));
					}*/

					return;
				}
			}

			/*if(MainTextBoxManager.Language.Name != "Lumen") {

				return;
			}
			*/
			//
			// Project?.Type?.Build(Project);
			//File.WriteAllText("main.txt", MainTextBoxManager.TextBox.Text);
			//Stereotype.Interpriter.Start("main.txt");
		}

		TaskFactory tf = new TaskFactory();

		private void StartProcess(String name) {
			Tuple<String, String> na = GetNameAndArgs(name);

			Process proc = new Process() {
				StartInfo = new ProcessStartInfo {
					FileName = na.Item1,
					Arguments = na.Item2
				}
			};

			this.tf.StartNew(() => {
				proc.Start();
				proc.WaitForExit();
			});
		}

		private Tuple<String, String> GetNameAndArgs(String command) {
			String[] src = command.Split(' ');

			StringBuilder builder = new StringBuilder();
			for (Int32 i = 1; i < src.Length; i++) {
				builder.Append(src[i]);
			}

			return new Tuple<String, String>(src[0], builder.ToString());
		}

		internal void RaiseError(IRunResult result) {
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
			e = null;
			this.errorTable.Rows.Clear();

			MainTextBoxManager?.OnTextChanged();
		}

		private void SetDynamicHightliting(FastColoredTextBox textBox) {
			foreach (Range x in textBox.GetRanges(@"\b(type)\s+(?<range>[\w_]+?)\b")) {
				textBox.Range.SetStyle(Settings.Type, @"\b" + x.Text + @"\b");
			}
		}

		private void Form1_Load(Object sender, EventArgs e) {
			foreach(var i in Settings.Languages) {
				if(i.Actor != null) {
					Lang.Std.Scope s = new Lang.Std.Scope();
					s.AddUsing(Lang.Std.StandartModule.__Kernel__);
					var op = new Interop.Interop();

					s.Set("studio", Interop.Interop.Module);

					s.Set(i.Name, new Lang.Std.LambdaFun(null));

					Stereotype.Interpriter.Start(i.Actor, s);

					i.Fn = s.Get(i.Name) as Lang.Std.Fun;
				}
			}

			if (CurrentFile == null) {
				LoadFile(Settings.DefaultFileName);
			}
		}

		private void LoadFile(String path) {
			if (File.Exists(path)) {
				SavePreviousFile();

				ProcessExtenstion(Path.GetExtension(path));

				MainTextBoxManager.TextBox.Text = File.ReadAllText(path);

				this.Text = path + " - Lumen Studio";

				CurrentFile = path;
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
			}
		}

		private void FastColoredTextBox1_TextChanged(Object sender, TextChangedEventArgs e) {

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

		private void SplitContainer3_SplitterMoved(Object sender, SplitterEventArgs e) {

		}

		private void HTMLToolStripMenuItem_Click(Object sender, EventArgs e) {
			/*new HTMLParser(this.textBox.Text).Run();
			Stereotype.Interpriter.Start("out.lm");*/
		}

		private void openToolStripMenuItem_Click(Object sender, EventArgs e) {
			OpenFileDialog dialog = new OpenFileDialog();
			if (dialog.ShowDialog() == DialogResult.OK) {
				LoadFile(dialog.FileName);
			}
		}

		private void saveToolStripMenuItem_Click(Object sender, EventArgs e) {
			SavePreviousFile();
		}

		private void pyToolStripMenuItem_Click(Object sender, EventArgs e) {
			this.output.Clear();

			ProcessStartInfo pi = new ProcessStartInfo("py") {
				Arguments = $"-m flake8 {CurrentFile}",
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardInput = true
			};

			Process p = Process.Start(pi);

			output.Write(p.StandardOutput.ReadToEnd());
		}

		private void MainForm_FormClosed(Object sender, FormClosedEventArgs e) {
			Application.Exit();
		}

		public Boolean IsCmdInitialized { get; private set; } = false;

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
						if(IsFirstCommand) {
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

			this.tf.StartNew(CmdInterface.RunProcess);

			this.IsCmdInitialized = true;
		}
	}
}
