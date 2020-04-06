using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Argent.Xenon.Runtime.Interop {
	public static class Utils {
		public static Object AsClr(this XnObject euObject) {
			if (euObject is Atom atom) {
				return atom.internalValue;
			}

			if (euObject is Text text) {
				return text.internalValue;
			}

			if (euObject is XnList seq) {
				return seq.internalValue.Select(AsClr).ToList();
			}

			if (euObject is ClrObject dyn) {
				return dyn.value;
			}

			return null;
		}

		public static XnObject AsEu(this Object obj) {
			return obj switch
			{
				ClrObject clrObject => clrObject.value.AsEu(),
				XnObject euObject => euObject,
				Boolean b => new Atom(b ? 1 : 0),
				Int32 i32 => new Atom(i32),
				Double dbl => new Atom(dbl),
				String str => new Text(str),
				Object x => new ClrObject(x)
			};
		}
	}
}
