using System;
using System.Collections.Generic;

using Lumen.Lang.Std;

namespace Lumen.Lang.Expressions {
	public interface Expression {
		Value Eval(Scope e);

		Expression Closure(List<String> visible, Scope scope);

		Expression Optimize(Scope scope);
	}
}
