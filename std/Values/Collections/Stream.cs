using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
    /// <summary> Предствляет экземпляр типа Kernel.Enumerator </summary>
    public sealed class Stream : Value, IEnumerable<Value> {
        public IEnumerable<Value> innerValue;

        public Stream(IEnumerable<Value> innerValue) {
            this.innerValue = innerValue;
        }

        public Value Clone() {
            return (Value)this.MemberwiseClone();
        }

        public Int32 CompareTo(Object obj) {
            throw new LumenException($"can not compare value of type 'Kernel.Enumerator' with value of type '{obj.GetType()}'");
        }

        public IEnumerator<Value> GetEnumerator() {
            return this.innerValue.GetEnumerator();
        }

        public IType Type => Prelude.Stream;

        IEnumerator IEnumerable.GetEnumerator() {
            return this.innerValue.GetEnumerator();
        }

        public String ToString(Scope e) {
            return "<Stream>";
        }

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString(null);
		}
	}
}
