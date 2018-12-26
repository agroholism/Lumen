using StandartLibrary;
using System;
using System.Collections.Generic;

namespace StandartLibrary {
	public class Null : Value {
		public Value Clone() {
			return this;
		}

		public KType Type => StandartModule.Null;

		public override String ToString() {
			return this.ToString(null);
		}

		public string ToString(Scope e) {
			return "";
		}

		public int CompareTo(object obj) {
			if (obj is Value) {
				Scope e = new Scope(null);
				e.Set("this", this);
				//return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(e, (Value)obj), null);
			}
			throw new Exception("notcomparable");
		}
	}
}
