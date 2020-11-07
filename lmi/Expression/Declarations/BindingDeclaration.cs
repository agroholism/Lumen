using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	public class BindingDeclaration : Expression {
        public IPattern pattern;
        public Expression assignableExpression;
        public String fileName;
        public Int32 lineName;

        public BindingDeclaration(IPattern variableName, Expression assignableExpression, String fileName, Int32 lineName) {
            this.pattern = variableName;
            this.assignableExpression = assignableExpression;
            this.lineName = lineName;
            this.fileName = fileName;
        }

        public Value Eval(Scope scope) {
            Value assignableValue = this.assignableExpression.Eval(scope);

			MatchResult matchResult = this.pattern.Match(assignableValue, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line: this.lineName, fileName: this.fileName) {
					Note = matchResult.Note
				};
			}

            return assignableValue;
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerable<Value> assignableExpressionEvaluationResults = 
				this.assignableExpression.EvalWithYield(scope);

			Value assignableValue = Const.UNIT;
			foreach (Value result in assignableExpressionEvaluationResults) {
				if (result is GeneratorExpressionTerminalResult cgv) {
					assignableValue = cgv.Value;
					break;
				}

				yield return result;
			}

			MatchResult matchResult = this.pattern.Match(assignableValue, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line: this.lineName, fileName: this.fileName) {
					Note = matchResult.Note
				};
			}

			yield return new GeneratorExpressionTerminalResult(assignableValue);
		}

		public Expression Closure(ClosureManager manager) {
			// Right expression - first
			Expression closuredAssignableExpression = this.assignableExpression?.Closure(manager);
			return new BindingDeclaration(this.pattern.Closure(manager) as IPattern, closuredAssignableExpression, this.fileName, this.lineName);
		}

		public override String ToString() {
			return $"let {this.pattern} = {this.assignableExpression}";
		}
	}
}