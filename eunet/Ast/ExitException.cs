#nullable enable

using System;

namespace Argent.Xenon.Ast {
	public class ExitException : Exception {
		public Int32 Line { get; private set; }
		public String File { get; private set; }

		public ExitException(String? message, Int32 line, String file) : base(message ?? "program was broken") {
			this.Line = line;
			this.File = file;
		}
	}
}