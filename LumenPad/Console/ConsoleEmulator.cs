using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace LumenPad {
	public class ConsoleEmulator : FastColoredTextBox {
		private volatile bool isReadLineMode;
		private volatile bool isUpdating;
		private Place StartReadPlace { get; set; }

		/// <summary>
		/// Control is waiting for line entering. 
		/// </summary>
		public bool IsReadLineMode {
			get { return isReadLineMode; }
			set { isReadLineMode = value; }
		}

		/// <summary>
		/// Append line to end of text.
		/// </summary>
		public void WriteLine(string text) {
			this.IsReadLineMode = false;
			this.isUpdating = true;
			try {
				this.InvokeNeded(() => {
					AppendText(text);
					GoEnd();
				});
			}
			finally {
				this.isUpdating = false;
				this.InvokeNeded(() => ClearUndo());
			}
		}

		/// <summary>
		/// Append line to end of text.
		/// </summary>
		public void Write(string text) {
			IsReadLineMode = false;
			isUpdating = true;
			try {
				this.InvokeNeded(() => AppendText(text));
			}
			finally {
				isUpdating = false;
				this.InvokeNeded(() => ClearUndo());
			}
		}

		public void Write(string text, Boolean b = true) {
			try {
				this.InvokeNeded(() => AppendText(text));
			}
			finally {
				isReadLineMode = b;
				isUpdating = b;
				this.InvokeNeded(() => ClearUndo());
			}
		}

		public event EventHandler OnReadLine;

		/// <summary>
		/// Wait for line entering.
		/// Set IsReadLineMode to false for break of waiting.
		/// </summary>
		/// <returns></returns>
		public string ReadLine() {
			GoEnd();
			StartReadPlace = Range.End;
			IsReadLineMode = true;
			try {
				while (IsReadLineMode) {
					Application.DoEvents();
					Thread.Sleep(5);
				}
			}
			finally {
				IsReadLineMode = false;
				ClearUndo();
			}

			var res = new Range(this, StartReadPlace, Range.End).Text.TrimEnd('\r', '\n');

			OnReadLine?.Invoke(res, new EventArgs { });

			return res;
		}

		public override void OnTextChanging(ref String text) {
			if (!IsReadLineMode && !isUpdating) {
				text = ""; //cancel changing
				return;
			}

			if (IsReadLineMode) {
				if (Selection.Start < StartReadPlace || Selection.End < StartReadPlace)
					GoEnd();//move caret to entering position

				if (Selection.Start == StartReadPlace || Selection.End == StartReadPlace)
					if (text == "\b") //backspace
					{
						text = ""; //cancel deleting of last char of readonly text
						return;
					}

				if (text != null && text.Contains('\n')) {
					text = text.Substring(0, text.IndexOf('\n') + 1);
					IsReadLineMode = false;
				}
			}

			base.OnTextChanging(ref text);
		}

		public override void Clear() {
			var oldIsReadMode = isReadLineMode;

			isReadLineMode = false;
			isUpdating = true;

			base.Clear();

			isUpdating = false;
			isReadLineMode = oldIsReadMode;

			StartReadPlace = Place.Empty;
		}
	}
}
