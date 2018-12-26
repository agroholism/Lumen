using System;
using System.Collections.Generic;

namespace StandartLibrary.Expressions {
	public interface Expression {
		//KType Validate(Scope scope);

		Value Eval(Scope e);

		Expression Closure(List<String> visible, Scope scope);

		Expression Optimize(Scope scope);
	}
}
