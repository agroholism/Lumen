using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Assert : Expression {
		private Expression expression;
		private string file;
		private int line;

		public Assert(Expression expression, string file, int line) {
			this.expression = expression;
			this.file = file;
			this.line = line;
		}

		public Expression Closure(ClosureManager manager) {
			return new Assert(this.expression.Closure(manager), this.file, this.line);
		}

		public IValue Eval(Scope scope) {
			var result = this.expression.Eval(scope);

			if (!result.ToBoolean()) {
				var exception = Prelude.AssertError.MakeExceptionInstance(new Text(
					$"assert is broken {this.expression}"
				)).ToException();

				String currentFunctionName = null;

				if (scope.IsExistsInThisScope("rec") && scope["rec"] is Fun currentFunction) {
					currentFunctionName = currentFunction.Name;
				}

				exception.SetLastCallDataIfAbsent(currentFunctionName, this.file, this.line);

				throw exception;
			}

			return result;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}
	}
}