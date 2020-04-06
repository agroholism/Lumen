using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

using FastColoredTextBoxNS;

namespace EuIDE {
	public class ConsoleEmulator : FastColoredTextBox {
		private volatile Boolean isReadLineMode;
		private volatile Boolean isUpdating;
		private Place StartReadPlace { get; set; }

		/// <summary>
		/// Control is waiting for line entering. 
		/// </summary>
		public Boolean IsReadLineMode {
			get { return this.isReadLineMode; }
			set { this.isReadLineMode = value; }
		}

		/// <summary>
		/// Append line to end of text.
		/// </summary>
		public void WriteLine(String text) {
			this.IsReadLineMode = false;
			this.isUpdating = true;
			Place end = this.Range.End;

			try {
				this.InvokeNeded(() => {
					this.AppendText(text + Environment.NewLine);
					this.GoEnd();
				});
			}
			finally {
				this.isUpdating = false;
				this.InvokeNeded(() => this.ClearUndo());
			}
		}

		/// <summary>
		/// Append line to end of text.
		/// </summary>
		public void Write(String text) {
			this.IsReadLineMode = false;
			this.isUpdating = true;
			try {
				this.InvokeNeded(() => this.AppendText(text));
			}
			finally {
				this.isUpdating = false;
				this.InvokeNeded(() => this.ClearUndo());
			}
		}

		public void Write(String text, Boolean b = true) {
			try {
				this.InvokeNeded(() => this.AppendText(text));
			}
			finally {
				this.isReadLineMode = b;
				this.isUpdating = b;
				this.InvokeNeded(() => this.ClearUndo());
			}
		}

		public event EventHandler OnReadLine;

		/// <summary>
		/// Wait for line entering.
		/// Set IsReadLineMode to false for break of waiting.
		/// </summary>
		/// <returns></returns>
		public String ReadLine() {
			this.GoEnd();
			this.StartReadPlace = this.Range.End;
			this.IsReadLineMode = true;
			try {
				while (this.IsReadLineMode) {
					Application.DoEvents();
					Thread.Sleep(5);
				}
			}
			finally {
				this.IsReadLineMode = false;
				this.ClearUndo();
			}

			String res = new Range(this, this.StartReadPlace, this.Range.End).Text.TrimEnd('\r', '\n');

			OnReadLine?.Invoke(res, new EventArgs { });

			return res;
		}

		public override void OnTextChanging(ref String text) {
			if (!this.IsReadLineMode && !this.isUpdating) {
				text = ""; //cancel changing
				return;
			}

			if (this.IsReadLineMode) {
				if (this.Selection.Start < this.StartReadPlace || this.Selection.End < this.StartReadPlace) {
					this.GoEnd();//move caret to entering position
				}

				if (this.Selection.Start == this.StartReadPlace || this.Selection.End == this.StartReadPlace) {
					if (text == "\b") //backspace
					{
						text = ""; //cancel deleting of last char of readonly text
						return;
					}
				}

				if (text != null && text.Contains('\n')) {
					text = text.Substring(0, text.IndexOf('\n') + 1);
					this.IsReadLineMode = false;
				}
			}

			base.OnTextChanging(ref text);
		}

		public override void Clear() {
			Boolean oldIsReadMode = this.isReadLineMode;

			this.isReadLineMode = false;
			this.isUpdating = true;

			base.Clear();

			this.isUpdating = false;
			this.isReadLineMode = oldIsReadMode;

			this.StartReadPlace = Place.Empty;
		}
	}
}
