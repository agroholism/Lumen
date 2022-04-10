﻿using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	public class BindingDeclaration : Expression {
        public IPattern pattern;
        public Expression assignableExpression;
        public String fileName;
        public Int32 lineNumber;

        public BindingDeclaration(IPattern variableName, Expression assignableExpression, String fileName, Int32 lineName) {
            this.pattern = variableName;
            this.assignableExpression = assignableExpression;
            this.fileName = fileName;
			this.lineNumber = lineName;
		}

        public IValue Eval(Scope scope) {
            IValue assignableValue = this.assignableExpression.Eval(scope);

			MatchResult matchResult = this.pattern.Match(assignableValue, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line: this.lineNumber, fileName: this.fileName) {
					Note = matchResult.Note
				};
			}

            return assignableValue;
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> assignableExpressionEvaluationResults = 
				this.assignableExpression.EvalWithYield(scope);

			IValue assignableValue = Const.UNIT;
			foreach (IValue evaluationResult in assignableExpressionEvaluationResults) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					assignableValue = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			MatchResult matchResult = this.pattern.Match(assignableValue, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line: this.lineNumber, fileName: this.fileName) {
					Note = matchResult.Note
				};
			}

			yield return new GeneratorExpressionTerminalResult(assignableValue);
		}

		public Expression Closure(ClosureManager manager) {
			// Right expression - first
			Expression closuredAssignableExpression = this.assignableExpression?.Closure(manager);
			return new BindingDeclaration(this.pattern.Closure(manager) as IPattern, closuredAssignableExpression, this.fileName, this.lineNumber);
		}

		public override String ToString() {
			return $"let {this.pattern} = {this.assignableExpression}";
		}
	}
}