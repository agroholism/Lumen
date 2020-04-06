using System;

namespace Argent.Xenon.Runtime.Interop {
	public class ClrType : KsType {
		public Type type;

		public ClrType(Type type) {
			this.type = type;
		}
	}
}
