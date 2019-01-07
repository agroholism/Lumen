using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public sealed class TypeMetadata {
		public String Name { get; set; }
		public Record Base { get; set; }
		public List<FunctionArgument> Fields { get; set; }
	}
}
