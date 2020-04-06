using System;

namespace Lumen.Lang {
	public class State : BaseValueImpl {
		public Value Value { get; set; }

		public override IType Type => Prelude.Ref;

		public State(Value value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[State {this.Value}]";
		}
	}

	public class ArrayRef : BaseValueImpl {
		public Array Value { get; set; }
		public Int32 Index { get; set; }

		public override IType Type => Prelude.Ref;

		public ArrayRef(Array value, Int32 index) {
			this.Value = value;
			this.Index = index;
		}

		public override String ToString() {
			return $"(State {this.Value})";
		}
	}
}
