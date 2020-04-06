using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	/// <summary> List value </summary>
	public class List : BaseValueImpl {
		public LinkedList value;

        public override IType Type => Prelude.List;

        public List() {
            this.value = LinkedList.Empty;
        }

        public List(params Value[] elements) {
            this.value = LinkedList.Create(elements);
        }

        public List(IEnumerable<Value> elements) {
            this.value = LinkedList.Create(elements);
        }

		internal List(LinkedList value) {
            this.value = value;
        }

        public override String ToString() {
            return "[" + String.Join(", ", this.value) + "]";
        }

        public override Boolean Equals(Object obj) {
            if (obj is List list) {
                return this.value.Equals(list.value);
            }

            return false;
        }

        public override Value Clone() {
            List<Value> result = new List<Value>();
            foreach (Value i in this.value) {
                result.Add(i);
            }
            return new Array(result);
        }

		public override Int32 GetHashCode() {
			return -1584136870;
		}
	}
}
