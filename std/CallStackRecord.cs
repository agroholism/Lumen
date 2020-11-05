using System;

namespace Lumen.Lang {
	internal class CallStackRecord {
		internal String FunctionName { get; set; }
		internal String FileName { get; set; }
		internal Int32 LineNumber { get; set; }

		internal CallStackRecord(String functionName, String fileName, Int32 lineNumber) {
			this.FunctionName = functionName;
			this.FileName = fileName;
			this.LineNumber = lineNumber;
		}
	}
}
