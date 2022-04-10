using System;

namespace Lumen.Lang {
	public class TailRecursion : Exception, IValue {
		public IValue[] newArguments;

		public TailRecursion(IValue[] result) {
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
