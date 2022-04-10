using System;

namespace Lumen.Lang {
	public sealed class Mut : BaseValueImpl {
		internal IValue Value { get; set; }

		public override IType Type => Prelude.Mut;

		public Mut(IValue value) {
			this.Value = value;
		}

		public override String ToString() {
			return $"(Mut {this.Value})";
		}
	}
}
