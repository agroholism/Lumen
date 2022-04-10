using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public interface Expression {
		IValue Eval(Scope scope);

		IEnumerable<IValue> EvalWithYield(Scope scope);

		Expression Closure(ClosureManager manager);
	}
}
