using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public sealed class Stream : Value {
		internal IEnumerable<Value> InternalValue { get; private set; }

		public Stream(IEnumerable<Value> innerValue) {
			this.InternalValue = innerValue;
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}

		public Int32 CompareTo(Object obj) {
			throw new LumenException($"can not compare value of type 'Kernel.Enumerator' with value of type '{obj.GetType()}'");
		}

		public IType Type => Prelude.Stream;

		public override String ToString() {
			return $"[Stream #{this.GetHashCode()}]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}
	}
}
