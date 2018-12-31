using System;
using System.Text;
using System.IO;

using FastColoredTextBoxNS;

namespace LumenPad {
	class ConsoleWriter : TextWriter {
		public override Encoding Encoding => Encoding.Unicode;

		public static ConsoleWriter Instance;

		private readonly ConsoleEmulator tb;

		public ConsoleWriter(ConsoleEmulator tb) {
			this.tb = tb;
		}

		public override void Write(String value) {
			this.tb.Write(value);
		}

		public override void WriteLine(String value) {
			this.tb.WriteLine(value);
		}
	}
}
