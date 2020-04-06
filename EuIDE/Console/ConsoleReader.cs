using System;
using System.IO;

namespace EuIDE {
    class ConsoleReader : TextReader {
        private readonly ConsoleEmulator tb;

        public ConsoleReader(ConsoleEmulator tb) {
            this.tb = tb;
        }

        public override String ReadLine() {
            return this.tb.ReadLine();
        }
    }
}
