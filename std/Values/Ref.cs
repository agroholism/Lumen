using System;

namespace Lumen.Lang {
	public class Ref : BaseValueImpl {
		public Value Value { get; set; }

		public override IType Type => Prelude.Ref;

		public Ref(Value value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[Ref {this.Value}]";
		}
	}
}
