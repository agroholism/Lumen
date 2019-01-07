using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang.Std {
	public static class Checker {
		public static void OfType(Value value, Record type, Scope scope) {
			if(value.Type != type) {
				throw new Exception(Exceptions.TYPE_ERROR.F(type, value.Type), stack: scope);
			}
		}
	}
}
