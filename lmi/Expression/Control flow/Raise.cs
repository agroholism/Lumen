using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Raise : Expression {
		private Expression expression;
		private Expression from;
		private Int32 line;
		private String file;

		public Raise(Expression expression, Expression from, String file, Int32 line) {
			this.expression = expression;
			this.from = from;
			this.file = file;
			this.line = line;
		}

		public Expression Closure(ClosureManager manager) {
			return new Raise(this.expression.Closure(manager), this.from?.Closure(manager), this.file, this.line);
		}

		public IValue Eval(Scope scope) {
			LumenException from = this.from?.Eval(scope)?.ToException(); // if not?

			// raise have no arguments
			if(this.expression == UnitLiteral.Instance) {
				LumenException catchedException =
					scope[Constants.LAST_EXCEPTION_SPECIAL_NAME].ToException();
				catchedException.Cause = from;
				throw catchedException;
			}

			IValue value = this.expression.Eval(scope);

			LumenException exception = value.ToException();

			String currentFunctionName = null;

			if (scope.IsExistsInThisScope("rec") && scope["rec"] is Fun currentFunction) {
				currentFunctionName = currentFunction.Name;
			}

			exception.SetLastCallDataIfAbsent(currentFunctionName, this.file, this.line);
			exception.Cause = from;
			throw exception;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			yield return this.Eval(scope);
		}
	}
}