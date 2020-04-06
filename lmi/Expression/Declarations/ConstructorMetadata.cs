using System;
using System.Collections.Generic;

namespace Lumen.Lmi {
	internal class ConstructorMetadata {
		public String Name { get; private set; }
		public List<String> Parameters { get; private set; }

		public ConstructorMetadata(String name, List<String> parameters) {
			this.Name = name;
			this.Parameters = parameters;
		}

		public override String ToString() {
			return $"{this.Name} {String.Join(" ", this.Parameters)}";
		}
	}
}