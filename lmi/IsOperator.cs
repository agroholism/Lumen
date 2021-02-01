using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class IsOperator : Expression {
		private Expression expression;
		private IPattern pattern;
		private Int32 line;
		private String file;

		public IsOperator(Expression expression, IPattern pattern, Int32 line, String file) {
			this.expression = expression;
			this.pattern = pattern;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope scope) {
			Value testValue = this.expression.Eval(scope);

			return new Logical(this.pattern.Match(testValue, scope).IsSuccess);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerable<Value> evaluationResults = this.expression.EvalWithYield(scope);
			Value testValue = null;

			foreach (Value evaluationResult in evaluationResults) {
				if(evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					testValue = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			yield return new GeneratorExpressionTerminalResult(new Logical(this.pattern.Match(testValue, scope).IsSuccess));
		}

		public Expression Closure(ClosureManager manager) {
			return new IsOperator(this.expression.Closure(manager), this.pattern.Closure(manager), this.line, this.file);
		}
	}
}