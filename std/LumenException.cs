using System;
using System.Collections.Generic;
using System.Text;

namespace Lumen.Lang {
    public class LumenException : Exception {
        public Int32 line;
        public String file;
        public String functionName;
        public String Note { get; set; }
        private List<Tuple<String, String, Int32>> callStack;

        public LumenException(String message, String functionName=null, Int32 line=-1, String fileName=null) : base(message) {
            this.callStack = new List<Tuple<String, String, Int32>>();
            this.line = line;
            this.file = fileName;
            this.functionName = functionName;
        }

        public void AddToCallStack(String functionName, String fileName, Int32 line) {
            this.callStack.Add(new Tuple<String, String, Int32>(functionName, fileName, line));
        }

        public override String ToString() {
            StringBuilder result = new StringBuilder();
            result.Append($"Exception: {this.Message} {Environment.NewLine}\tin {this.functionName} at {this.file}:{this.line}");

            if (this.callStack.Count > 0) {
                foreach (Tuple<String, String, Int32> i in this.callStack) {
                    if (i.Item1 == null) {
                        result.Append(Environment.NewLine).Append($"\tin <main> at {i.Item2}:{i.Item3}");
                    } else {
                        result.Append(Environment.NewLine).Append($"\tin {i.Item1} at {i.Item2}:{i.Item3}");
                    }
                }
            }

            return result.ToString();
        }
    }
}
