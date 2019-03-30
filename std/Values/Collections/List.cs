using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
    public class List : Value {
        public LinkedList value;

        public IObject Type => Prelude.List;

        public List() {
            this.value = LinkedList.Empty;
        }

        public List(params Value[] elements) {
            this.value = LinkedList.Create(elements);
        }

        public List(List<Value> elements) {
            this.value = LinkedList.Create(elements);
        }

        public List(LinkedList value) {
            this.value = value;
        }

        public String ToString(Scope e) {
            return "[" + String.Join("; ", this.value) + "]";
        }

        public override Boolean Equals(Object obj) {
            if (obj is List) {
                LinkedList v1 = this.value;
                LinkedList v2 = ((List)obj).value;

                return v1.Equals(v2);
            }
            return false;
        }

        public Int32 CompareTo(Object obj) {
            if (obj is Value) {
                Scope e = new Scope(null);
                e.Set("this", this);
                //return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(e, (Value)obj), null);
            }
            throw new LumenException("notcomparable");
        }

        public override Int32 GetHashCode() {
            Int32 res = 0;

            foreach (Value i in this.value) {
                res |= i.GetHashCode();
            }

            return res;
        }

        public Value Clone() {
            List<Value> result = new List<Value>();
            foreach (Value i in this.value) {
                result.Add(i);
            }
            return new Array(result);
        }

        public override String ToString() {
            return this.ToString(null);
        }
    }
}
