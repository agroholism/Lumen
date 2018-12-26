using System;

namespace Lumen.Lang.Std {
	public struct FunctionArgument {
		public String name;
		public Object defaultValue;

		public FunctionArgument(String name, Object defaultValue = null) {
			this.name = name;
			this.defaultValue = defaultValue;
		}
	}
}
