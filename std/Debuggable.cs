using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	interface Debuggable : Value {
		Dictionary<String, Tuple<Value, Int32>> Inspect();
	}
}
