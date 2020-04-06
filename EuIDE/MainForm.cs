using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace EuIDE {
	public partial class MainForm : Form {
		private TextBoxManager mainManager;
		private ConsoleEmulator output;

		Thread thread;

		public MainForm() {
			this.InitializeComponent();

			this.menu.Renderer = new MenuRenderer {
				ArrowColor = Color.Black,
				ArrowSelectedColor = Color.WhiteSmoke,
				BackColor = Color.WhiteSmoke,
				BorderColor = Color.WhiteSmoke,
				ForeColor = Color.Black,
				SelectedBackColor = Color.AliceBlue
			};

			this.mainManager = new TextBoxManager(this.textBox);

			this.output = new ConsoleEmulator {
				Dock = DockStyle.Fill,
				BackColor = Color.WhiteSmoke,
				ShowLineNumbers = false
			};
			this.splitContainer.Panel2.Controls.Add(output);

			Console.SetOut(new ConsoleWriter(this.output));
			Console.SetIn(new ConsoleReader(this.output));

			this.textBox.Text = File.ReadAllText("main.eu");

			Argent.Xenon.Interpreter.Handle += (ex) => {
				if (ex.line != -1) {
					this.InvokeNeded(() => {
						this.RaiseError(ex.line);
						this.output.WriteLine(ex.ToString());
					});
				}
			};

			Argent.Xenon.Interpreter.OnExit += (ex) => {
				if (ex.Line != -1) {
					this.InvokeNeded(() => {
						this.RaiseError(ex.Line);
						this.output.WriteLine(ex.Message);
					});
				}
			};
		}

		private void RunToolStripMenuItem1_Click(Object sender, EventArgs e) {
			this.output.Clear();

			File.WriteAllText("main.eu", this.textBox.Text);
			this.thread = new Thread(() => {
				Argent.Xenon.Interpreter.Run(this.textBox.Text, "main.eu");
			});

			this.thread.Start();
		}

		private void RaiseError(Int32 line) {
			this.textBox.GetLine(line - 1).SetStyle(Params.Error);
		}
	}
}
