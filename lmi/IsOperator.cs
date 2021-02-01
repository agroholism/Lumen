using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class IsOperator : Expression {
		private Expression expression;
		private Expression type;
		private Int32 line;
		private String file;

		public IsOperator(Expression expression, Expression type, Int32 line, String file) {
			this.expression = expression;
			this.type = type;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope scope) {
			Value testValue = this.expression.Eval(scope);
			IType type = this.type.Eval(scope) as IType;

			if(type == null) {
				throw new LumenException("is operator expects a type", this.line, this.file);
			}

			return new Logical(type.IsParentOf(testValue));
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

			IType type = this.type.Eval(scope) as IType;

			if (type == null) {
				throw new LumenException("is operator expects a type", this.line, this.file);
			}

			yield return new GeneratorExpressionTerminalResult(new Logical(type.IsParentOf(testValue)));
		}

		public Expression Closure(ClosureManager manager) {
			return new IsOperator(this.expression.Closure(manager), this.type.Closure(manager), this.line, this.file);
		}
	}
}