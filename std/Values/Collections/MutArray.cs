using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public sealed class MutArray : BaseValueImpl {
		internal List<IValue> InternalValue { get; private set; }

		public override IType Type => Prelude.MutArray;

		public MutArray(List<IValue> value) {
			this.InternalValue = value;
		}

		public MutArray(IEnumerable<IValue> value) {
			this.InternalValue = value.ToList();
		}

		public MutArray(params IValue[] args) {
			this.InternalValue = args.ToList();
		}

		public override String ToString() {
			return "@[" + String.Join(", ", this.InternalValue) + "]";
		}

		public override Boolean Equals(Object obj) {
			if (obj is MutArray array) {
				List<IValue> list1 = this.InternalValue;
				List<IValue> list2 = array.InternalValue;

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
