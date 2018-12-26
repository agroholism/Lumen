using System;

namespace Lumen.Lang.Std {
	[Serializable]
	class DateTime : Value {
		System.DateTime value;

		public DateTime(System.DateTime value) {
			this.value = value;
		}

		public KType Type => StandartModule.DateTime;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return this.value.ToString();
		}

		public override String ToString() {
			return this.ToString(null);
		}
	}
}
