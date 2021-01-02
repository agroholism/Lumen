using System;

namespace Lumen.Lang {
	public class Mut : BaseValueImpl {
		public Value Value { get; set; }

		public override IType Type => Prelude.Mut;

		public Mut(Value value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[Ref {this.Value}]";
		}
	}
}
