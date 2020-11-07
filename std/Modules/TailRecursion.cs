using System;

namespace Lumen.Lang {
	public class TailRecursion : Exception, Value {
		public Value[] newArguments;

		public TailRecursion(Value[] result) {
			this.newArguments = result;
		}

		public IType Type => throw new NotImplementedException();

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			throw new NotImplementedException();
		}
	}
}
