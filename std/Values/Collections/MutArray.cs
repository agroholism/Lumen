using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public sealed class MutArray : BaseValueImpl {
		internal List<Value> InternalValue { get; private set; }

		public override IType Type => Prelude.MutArray;

		public MutArray(List<Value> value) {
			this.InternalValue = value;
		}

		public MutArray(IEnumerable<Value> value) {
			this.InternalValue = value.ToList();
		}

		public MutArray(params Value[] args) {
			this.InternalValue = args.ToList();
		}

		public override String ToString() {
			return "@[" + String.Join(", ", this.InternalValue) + "]";
		}

		public override Boolean Equals(Object obj) {
			if (obj is MutArray array) {
				List<Value> list1 = this.InternalValue;
				List<Value> list2 = array.InternalValue;

				if (list1.Count != list2.Count) {
					return false;
				}

				for (Int32 i = 0; i < list1.Count; i++) {
					if (!list1[i].Equals(list2[i])) {
						return false;
					}
				}

				return true;
			}

			return false;
		}

		public override Int32 GetHashCode() {
			return -15211345;
		}
	}
}
