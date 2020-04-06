using System;
using System.Linq;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class XnList : XnObject {
		public List<XnObject> internalValue;

		public XnList() {
			this.internalValue = new List<XnObject>();
		}

		public XnList(List<XnObject> values) {
			this.internalValue = values;
		}

		public KsTypeable Type => XnStd.ListType;

		public override Boolean Equals(Object obj) {
			if(obj is XnList sequence) {
				if(sequence.internalValue.Count != this.internalValue.Count) {
					return false;
				}

				for(Int32 i = 0; i < sequence.internalValue.Count; i++) {
					if(!this.internalValue[i].Equals(sequence.internalValue[i])) {
						return false;
					}
				}

				return true;
			}
			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return 0;
		}

		public override String ToString() {
			return "{ " + String.Join(", ", this.internalValue) +" }";
		}	}

	public class XnImmutList : ImmutObject {
		public List<XnObject> internalValue;

		public XnImmutList() {
			this.internalValue = new List<XnObject>();
		}

		public XnImmutList(List<XnObject> values) {
			this.internalValue = values;
		}

		public KsTypeable Type => XnStd.ListType;

		public override Boolean Equals(Object obj) {
			if (obj is XnList sequence) {
				if (sequence.internalValue.Count != this.internalValue.Count) {
					return false;
				}

				for (Int32 i = 0; i < sequence.internalValue.Count; i++) {
					if (!this.internalValue[i].Equals(sequence.internalValue[i])) {
						return false;
					}
				}

				return true;
			}
			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return 0;
		}

		public override String ToString() {
			return "{ " + String.Join(", ", this.internalValue) + " }";
		}
	}
}
