using System;

namespace Argent.Xenon.Runtime.Interop {
	public class ClrObject : XnObject {
		public Object value;

		public KsTypeable Type => new ClrType(this.value.GetType());

		public ClrObject(Object value) {
			this.value = value;
		}

		public override Boolean Equals(Object obj) {
			return this.value.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return this.value.GetHashCode();
		}

		public override String ToString() {
			return this.value.ToString();
		}
	}
}
