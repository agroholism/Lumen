using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public interface Expression {
		Value Eval(Scope scope);

		IEnumerable<Value> EvalWithYield(Scope scope);

		Expression Closure(ClosureManager manager);
	}
}
