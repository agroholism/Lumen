using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public delegate Value HFun(Scope e, params Value[] args);

	public interface Fun : Value {
		Dictionary<String, Value> Attributes { get; set; }
		List<FunctionArgument> Arguments { get; set; }
		
		Value Run(Scope e, params Value[] args);
	}
}
