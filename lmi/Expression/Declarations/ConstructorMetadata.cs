using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ConstructorMetadata {
		public String Name { get; private set; }
		public Dictionary<String, List<Expression>> Parameters { get; private set; }

		public ConstructorMetadata(String name, Dictionary<String, List<Expression>> parameters) {
			this.Name = name;
			this.Parameters = parameters;
		}

		public override String ToString() {
			return $"{this.Name} {String.Join(" ", this.Parameters)}";
		}
	}
}