using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class InternWithBindingDeclaration : Expression {
		private Expression bindingDeclaration;

		private Value evaluationResult;

		public InternWithBindingDeclaration(Expression bindingDeclaration) {
			this.bindingDeclaration = bindingDeclaration;
		}

		public Expression Closure(ClosureManager manager) {
			return
					new InternWithBindingDeclaration(this.bindingDeclaration.Closure(manager));
		}

		public Value Eval(Scope scope) {
			if (this.evaluationResult == null) {
				this.evaluationResult = this.bindingDeclaration.Eval(scope);
				return this.evaluationResult;
			}

			if (this.bindingDeclaration is BindingDeclaration bindingDeclaration) {
				MatchResult matchResult = bindingDeclaration.pattern.Match(this.evaluationResult, scope);
				if (!matchResult.IsSuccess) {
					throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line: bindingDeclaration.lineNumber, fileName: bindingDeclaration.fileName) {
						Note = matchResult.Note
					};
				}
			}
			else if (this.bindingDeclaration is FunctionDeclaration functionDeclaration) {
				scope.Bind(functionDeclaration.name, evaluationResult);
			}

			return evaluationResult;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}
	}
}