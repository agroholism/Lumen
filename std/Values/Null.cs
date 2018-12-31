using Lumen;
using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Null : Value {
		public Value Clone() {
			return this;
		}

		public Record Type => StandartModule.Null;

		public override String ToString() {
			return this.ToString(null);
		}

		public String ToString(Scope e) {
			return "";
		}

		public Int32 CompareTo(Object obj) {
			if (obj is Value) {
				Scope e = new Scope(null);
				e.Set("this", this);
				//return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(e, (Value)obj), null);
			}
			throw new Exception("notcomparable");
		}
	}
}
