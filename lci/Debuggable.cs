using System;
using System.Collections.Generic;

namespace StandartLibrary {
	interface Debuggable : Value {
		Dictionary<String, Tuple<Value, Int32>> Inspect();
	}
}
