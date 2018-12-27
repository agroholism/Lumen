using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public struct FunctionArgument {
		public String name;
		public Object defaultValue;
		public Dictionary<String, Value> Attributes { get; set; }

		public FunctionArgument(String name, Object defaultValue = null) {
			this.name = name;
			this.defaultValue = defaultValue;
			this.Attributes = new Dictionary<String, Value>();
		}
	}
}
