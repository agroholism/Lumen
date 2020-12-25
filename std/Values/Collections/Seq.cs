using System;
using System.Linq;
using System.Collections.Generic;

namespace Lumen.Lang {
	public sealed class Seq : Value {
		internal IEnumerable<Value> InternalValue { get; private set; }

		public static Seq Empty => new Seq(Enumerable.Empty<Value>());

		public Seq(IEnumerable<Value> innerValue) {
			this.InternalValue = innerValue;
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}

		public Int32 CompareTo(Object obj) {
			throw new LumenException($"can not compare value of type 'Kernel.Enumerator' with value of type '{obj.GetType()}'");
		}

		public IType Type => Prelude.Seq;

		public override String ToString() {
			return $"[Stream #{this.GetHashCode()}]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}
	}
}
