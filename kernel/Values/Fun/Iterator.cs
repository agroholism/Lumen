using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	/// <summary> Предствляет экземпляр типа Kernel.Enumerator </summary>
	public sealed class Enumerator : Value, IEnumerable<Value> {
		internal IEnumerable<Value> innerValue;

		public Enumerator(IEnumerable<Value> innerValue) {
			this.innerValue = innerValue;
		}

		public Value Clone() {
			return (Value)MemberwiseClone();
		}

		public Int32 CompareTo(Object obj) {
			throw new Exception($"can not compare value of type 'Kernel.Enumerator' with value of type '{obj.GetType()}'", stack: null);
		}

		public IEnumerator<Value> GetEnumerator() {
			return innerValue.GetEnumerator();
		}

		public KType Type => StandartModule.Enumerator;

		IEnumerator IEnumerable.GetEnumerator() {
			return innerValue.GetEnumerator();
		}

		public String ToString(Scope e) {
			return "";
		}
	}
}
