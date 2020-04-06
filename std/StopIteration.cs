using System;

namespace Lumen.Lang {
	public class StopIteration : Value {
		public IType Type => throw new NotImplementedException();

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			throw new NotImplementedException();
		}
	}
}
