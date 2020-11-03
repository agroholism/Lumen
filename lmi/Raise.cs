using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Raise : Expression {
		private Expression expression;
		private Int32 line;
		private String file;

		public Raise(Expression expression, String file, Int32 line) {
			this.expression = expression;
			this.file = file;
			this.line = line;
		}

		public Expression Closure(ClosureManager manager) {
			return new Raise(this.expression.Closure(manager), this.file, this.line);
		}

		public Value Eval(Scope scope) {
			Value value = this.expression.Eval(scope);

			LumenException exception = value.ToException(scope);
			exception.SetDataIfAbsent(null, this.file, this.line);
			throw exception;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return this.Eval(scope);
		}
	}
}