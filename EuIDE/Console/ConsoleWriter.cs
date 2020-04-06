using System;
using System.Text;
using System.IO;

namespace EuIDE {
    class ConsoleWriter : TextWriter {
        public override Encoding Encoding => Encoding.Unicode;

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
