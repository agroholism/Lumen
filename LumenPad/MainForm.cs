using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

using FastColoredTextBoxNS;

namespace LumenPad {
	public partial class MainForm : Form {
		public static MainForm Instance { get; private set; }

		public static TextBoxManager MainTextBoxManager { get; set; }

		public static Project Project { get; set; }

		public String err;
		public Place? b;
		public Place? e;

		public MainForm() {
			InitializeComponent();

			Instance = this;

			MainTextBoxManager = new TextBoxManager(this.textBox) {
				Language = KlischeeLanguage.Instance
			};

			this.fastColoredTextBox1.ShowScrollBars = false;
			this.splitContainer2.SplitterWidth = 1;
			this.splitContainer3.SplitterWidth = 1;

			ConsoleWriter.Instance = new ConsoleWriter(this.output);
			ConsoleReader.Instance = new ConsoleReader(this.output);

			Console.SetOut(ConsoleWriter.Instance);
			Console.SetIn(ConsoleReader.Instance);
		}

		private void Run(Object sender, EventArgs e) {
			this.output.Clear();
			this.errorTable.Rows.Clear();
			//
			// Project?.Type?.Build(Project);
			File.WriteAllText("main.txt", MainTextBoxManager.TextBox.Text);
			Stereotype.Interpriter.Start("main.txt");

			/*Process p = new Process {
				StartInfo = new ProcessStartInfo {
					Arguments = "main.txt",
					//CreateNoWindow = true,
				//	UseShellExecute = false,
					FileName = "C:\\Ruby24-x64\\bin\\ruby",
				//	RedirectStandardInput = true,
				//	RedirectStandardOutput = true,
					
				}
			};*/

			/*p.OutputDataReceived += (subsender, ex) => {
				Console.Write(ex.Data);
			};*/

			//p.Start();

			
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

		private void textBox_TextChanged(Object sender, TextChangedEventArgs e) {
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
			if (File.Exists("main.txt")) {
				this.textBox.Text = File.ReadAllText("main.txt");
			}
		}

		private void fastColoredTextBox1_TextChanged(Object sender, TextChangedEventArgs e) {

		}

		private void textBox_ToolTipNeeded(Object sender, ToolTipNeededEventArgs e) {
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

		private void splitContainer3_SplitterMoved(Object sender, SplitterEventArgs e) {

		}
	}
}
