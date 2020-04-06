using System;

namespace ldoc {
	internal class ParameterMetadata {
		public String Name { get; private set; }
		public Boolean IsMutable { get; private set; }

		public ParameterMetadata(String name, Boolean isMutable) {
			this.Name = name;
			this.IsMutable = isMutable;
		}
	}
}
// 1229 -> 1143