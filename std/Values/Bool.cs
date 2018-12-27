using System;

namespace Lumen.Lang.Std {
	public struct Bool : Value {
		internal Boolean value;
		public Record Type => StandartModule.Boolean;

		public Bool(Boolean value) {
			this.value = value;
		}

		public static implicit operator Bool(Boolean value) {
			return new Bool(value);
		}

		public Int32 CompareTo(Object obj) {
			if (obj is Bool b) {
				Boolean value = this.value;
				Boolean other = b.value;

				if (value == other) {
					return 0;
				}
				else if (value == false && other == true) {
					return -1;
				}
				else {
					return 1;
				}
			}

			throw new Exception("expected value of type 'Kernel.Boolean'", stack: null);
		}

		public Value Clone() {
			return new Bool(this.value);
		}

		public String ToString(Scope e) {
			return this.value ? "true" : "false";
		}

		public override String ToString() {
			return this.ToString(null);
		}
	}
}
