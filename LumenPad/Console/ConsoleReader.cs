using System;
using System.IO;

namespace LumenPad {
	class ConsoleReader : TextReader {
		public static ConsoleReader Instance;

		private readonly ConsoleEmulator tb;

		public ConsoleReader(ConsoleEmulator tb) {
			this.tb = tb;
		}

		public override String ReadLine() {
			return this.tb.ReadLine();
		}
	}
}
