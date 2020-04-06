using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Argent.Xenon.Runtime;
using XnObject = Argent.Xenon.Runtime.XnObject;

namespace Argent.Xenon.Ast {
	interface Expression {
		XnObject Eval(Scope scope);
		Expression Closure(ClosureManager manager);

		IEnumerable<XnObject> EvalWithYield(Scope scope);
	}
}
