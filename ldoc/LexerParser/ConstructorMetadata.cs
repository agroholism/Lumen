using System;
using System.Collections.Generic;

namespace ldoc {
	internal class ConstructorMetadata {
		public String Name { get; private set; }
		public List<ParameterMetadata> Parameters { get; private set; }

		public ConstructorMetadata(String name, List<ParameterMetadata> parameters) {
			this.Name = name;
			this.Parameters = parameters;
		}
	}
}
// 1229 -> 1143