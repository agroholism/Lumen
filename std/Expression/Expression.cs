using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public interface Expression {
        Value Eval(Scope e);
		IEnumerable<Value> EvalWithYield(Scope scope);

		Expression Closure(ClosureManager manager);
    }
}
