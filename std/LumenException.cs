using System;
using System.Collections.Generic;
using System.Text;

namespace Lumen.Lang {
	public class LumenException : Exception, Value {
		public IType Type { get; }

		public Value[] items;
		private List<CallStackRecord> callStack;

		public String Note { get; set; }
		public LumenException Cause { get; set; }

		public Int32 Line => this.callStack.Count > 0 ? this.callStack[0].LineNumber : -1;

		public LumenException(String message) : base(message) {
			this.Type = Prelude.Error;
			this.items = new[] { new Text(message) };
			this.callStack = new List<CallStackRecord> {
				new CallStackRecord(null, null,-1)
			};
		}

		public LumenException(String message, Int32 line = -1, String fileName = null) : this(message) {
			this.SetLastCallDataIfAbsent(null, fileName, line);
		}

		public LumenException(Constructor type, String message, Int32 line = -1, String fileName = null) : base(message) {
			this.items = new Value[type.Fields.Count];
			this.Type = type;
			this.callStack = new List<CallStackRecord> {
				new CallStackRecord(null, fileName, line)
			};
		}

		public void AddToCallStack(String functionName, String fileName, Int32 line) {
			this.callStack.Add(new CallStackRecord(functionName, fileName, line));
		}

		public Boolean SetLastCallDataIfAbsent(String functionName = null, String fileName = null, Int32 lineNumber = -1) {
			CallStackRecord lastCall =
				this.callStack[this.callStack.Count - 1];

			lastCall.FileName ??= fileName;
			lastCall.LineNumber = lastCall.LineNumber == -1 ? lineNumber : lastCall.LineNumber;

			if (lastCall.FunctionName == null) {
				lastCall.FunctionName = functionName;
				return true;
			}

			return false;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder()
				.Append($"{this.Type}: ")
				.Append(this.Message)
				.Append(' ')
				.Append(Environment.NewLine);

			for (Int32 i = this.callStack.Count - 1; i >= 0; i--) {
				CallStackRecord call = this.callStack[i];

				if(call.FileName == null && call.FunctionName == null && call.LineNumber == -1) {
					continue;
				}

				result
				.Append(Environment.NewLine)
				.Append("\t");

				if (call.FileName != null) {
					result.Append($"at \"{call.FileName}\" ");
				}

				if (call.LineNumber >= 0) {
					result.Append($"on {call.LineNumber} ");
				}

				if (call.FunctionName != null) {
					result.Append($"in {call.FunctionName}");
				}
			}

			if (this.Note != null) {
				result
					.Append(Environment.NewLine)
					.Append(Environment.NewLine)
					.Append(this.Note);
			}

			if (this.HelpLink != null) {
				result
					.Append(Environment.NewLine)
					.Append(Environment.NewLine)
					.Append("For more information please visit ")
					.Append(this.HelpLink);
			}

			if (this.Cause != null) {
				result
					.Append(Environment.NewLine)
					.Append(Environment.NewLine)
					.Append($"The above exception was the direct cause of the following exception:")
					.Append(Environment.NewLine)
					.Append(Environment.NewLine)
					.Append(this.Cause.ToString());
			}

			return result.ToString();
		}

		public Boolean TryGetField(String name, out Value result) {
			Int32 index = (this.Type as ExceptionConstructor).Fields.IndexOf(name);

			if (index != -1) {
				result = this.items[index];
				return true;
			}

			result = null;
			return false;
		}

		public Value GetField(String name) {
			return this.TryGetField(name, out Value result)
				? result
				: throw new LumenException(Exceptions.INSTANCE_OF_DOES_NOT_CONTAINS_FIELD.F(this.Type, name));
		}

		public override Boolean Equals(Object obj) {
			if (obj is Instance otherInstance
				&& otherInstance.Type == this.Type
				&& otherInstance.Items.Length == this.items.Length) {

				for (Int32 i = 0; i < this.items.Length; i++) {
					if (!this.items[i].Equals(otherInstance.Items[i])) {
						return false;
					}
				}

				return true;
			}

			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return base.GetHashCode();
		}

		public virtual Int32 CompareTo(Object obj) {
			if (obj is Value value) {
				if (this.Type.HasImplementation(Prelude.Ord)
					&& this.Type.GetMember("compare", null).TryConvertToFunction(out Fun comparator)) {
					return comparator.Call(new Scope(), this, value).ToInt(null);
				}
			}

			throw new LumenException("its not ord");
		}

		public virtual String ToString(String format, IFormatProvider formatProvider) {
			if (this.Type.HasImplementation(Prelude.Format) &&
				this.Type.GetMember("format", null).TryConvertToFunction(out Fun function)) {
				function.Call(new Scope(), this, new Text(format ?? "")).ToString();
			}

			return this.ToString();
		}
	}
}
