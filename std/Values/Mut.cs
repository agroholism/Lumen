using System;

namespace Lumen.Lang {
	public class Mut : BaseValueImpl {
		public IValue Value { get; set; }

		public override IType Type => Prelude.Mut;

		public Mut(IValue value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"[Ref {this.Value}]";
		}
	}
}
