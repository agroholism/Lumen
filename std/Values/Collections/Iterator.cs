using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
    /// <summary> Предствляет экземпляр типа Kernel.Enumerator </summary>
    public sealed class Enumerator : Value, IEnumerable<Value> {
        internal IEnumerable<Value> innerValue;

        public Enumerator(IEnumerable<Value> innerValue) {
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

        public IObject Type => Prelude.Sequence;

        IEnumerator IEnumerable.GetEnumerator() {
            return this.innerValue.GetEnumerator();
        }

        public String ToString(Scope e) {
            return "";
        }
    }
}
