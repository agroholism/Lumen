using System;
using System.Collections.Generic;
using System.Text;

namespace Lumen.Lang {
	public class LumenException : Exception {
		private class CallRecord {
			public String FunctionName { get; set; }
			public String FileName { get; set; }
			public Int32 LineNumber { get; set; }

			public CallRecord(String functionName, String fileName, Int32 lineNumber) {
				this.FunctionName = functionName;
				this.FileName = fileName;
				this.LineNumber = lineNumber;
			}
		}

		public Int32 Line => this.callStack.Count > 0 ? this.callStack[0].LineNumber : -1;

		public String Note { get; set; }
		public Value LumenObject { get; set; }
		private List<CallRecord> callStack;

		public LumenException(String message) : base(message) {
			this.callStack = new List<CallRecord>();
			this.AddToCallStack(null, null, -1);
		}

		public LumenException(String message, Int32 line = -1, String fileName = null) : this(message) {
			this.AddToCallStack(null, fileName, line);
		}

		public void AddToCallStack(String functionName, String fileName, Int32 line) {
			this.callStack.Add(new CallRecord(functionName, fileName, line));
		}

		public Boolean SetDataIfAbsent(String functionName, String fileName, Int32 line) {
			CallRecord lastCall = this.callStack[this.callStack.Count - 1];

			lastCall.FileName ??= fileName;
			lastCall.LineNumber = lastCall.LineNumber == -1 ? line : lastCall.LineNumber;

			if (lastCall.FunctionName == null) {
				lastCall.FunctionName = functionName;
				return true;
			}

			return false;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder()
				.Append(
				$"{this.LumenObject?.Type?.ToString() ?? "Exception"}: {this.Message} {Environment.NewLine}");

			for (Int32 i = this.callStack.Count - 1; i >= 0; i--) {
				CallRecord call = this.callStack[i];

				result
				.Append(Environment.NewLine)
				.Append($"\tin {call.FunctionName ?? "<main>"} at {call.FileName}:{call.LineNumber}");
			}

			return result.ToString();
		}
	}
}
