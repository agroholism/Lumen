using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ConsOperator : Expression {
		private Expression leftExpression;
		private Expression rightExpression;
		private String fileName;
		private Int32 lineNumber;

		public ConsOperator(Expression result, Expression right, String fileName, Int32 lineNumber) {
			this.leftExpression = result;
			this.rightExpression = right;
			this.fileName = fileName;
			this.lineNumber = lineNumber;
		}

		public Value Eval(Scope e) {
			if (this.leftExpression is IdExpression ide) {
				if (ide.id == "_") {
					if (this.rightExpression is IdExpression ide2 && ide2.id == "_") {
						return new UserFun(
							new List<IPattern> { new NamePattern("x"), new NamePattern("y") }, 
							new ConsOperator(
								new IdExpression("x", ide.file, ide.line), 
								new IdExpression("y", ide2.file, ide2.line), 
								this.fileName, this.lineNumber));
					}
					else {
						return new UserFun(
							new List<IPattern> { new NamePattern("x") },
							new ConsOperator(
								new IdExpression("x", ide.file, ide.line), 
								new ValueLiteral(this.rightExpression.Eval(e)), 
								this.fileName, this.lineNumber));
					}
				}
			}
			else if (this.rightExpression is IdExpression _ide && _ide.id == "_") {
				return new UserFun(
					new List<IPattern> { new NamePattern("x") }, 
					new ConsOperator(
						new ValueLiteral(this.leftExpression.Eval(e)), 
						new IdExpression("x", _ide.file, _ide.line), 
						this.fileName, this.lineNumber));
			}

			Value rightOperand = this.rightExpression.Eval(e);
			Value leftOperand = this.leftExpression.Eval(e);

			if (rightOperand is List list) {
				return new List(new LinkedList(leftOperand, list.Value));
			}

			IType type = rightOperand is SingletonConstructor sc ? sc : rightOperand.Type;

			return new Applicate(new DotOperator(new ValueLiteral(type), "::", this.fileName, this.lineNumber), new List<Expression> { new ValueLiteral(leftOperand), new ValueLiteral(rightOperand) }, this.fileName, this.lineNumber).Eval(e);
		}

		public IEnumerable<Value> EvalWithYield(Scope e) {
			if (this.leftExpression is IdExpression ide) {
				if (ide.id == "_") {
					if (this.rightExpression is IdExpression ide2 && ide2.id == "_") {
						yield return new GeneratorExpressionTerminalResult(new UserFun(
							new List<IPattern> { new NamePattern("x"), new NamePattern("y") },
							new ConsOperator(
								new IdExpression("x", ide.file, ide.line),
								new IdExpression("y", ide2.file, ide2.line),
								this.fileName, this.lineNumber)));
					}
					else {
						yield return new GeneratorExpressionTerminalResult(new UserFun(
							new List<IPattern> { new NamePattern("x") },
							new ConsOperator(
								new IdExpression("x", ide.file, ide.line),
								new ValueLiteral(this.rightExpression.Eval(e)),
								this.fileName, this.lineNumber)));
					}
				}
			}
			else if (this.rightExpression is IdExpression _ide && _ide.id == "_") {
				yield return new GeneratorExpressionTerminalResult(new UserFun(
					new List<IPattern> { new NamePattern("x") },
					new ConsOperator(
						new ValueLiteral(this.leftExpression.Eval(e)),
						new IdExpression("x", _ide.file, _ide.line),
						this.fileName, this.lineNumber)));
			}

			Value rightOperand = null;
			Value leftOperand = null;

			IEnumerable<Value> rightExpressionEvaluationResults = this.rightExpression.EvalWithYield(e);
			IEnumerable<Value> leftExpressionEvaluationResults = this.leftExpression.EvalWithYield(e);

			foreach (Value result in rightExpressionEvaluationResults) {
				if (result is GeneratorExpressionTerminalResult cgv1) {
					rightOperand = cgv1.Value;
				}
				else {
					yield return result;
				}
			}

			foreach (Value result in leftExpressionEvaluationResults) {
				if (result is GeneratorExpressionTerminalResult cgv1) {
					leftOperand = cgv1.Value;
				}
				else {
					yield return result;
				}
			}


			if (rightOperand is List list) {
				yield return new GeneratorExpressionTerminalResult(new List(new LinkedList(leftOperand, list.Value)));
			}

			IType type = rightOperand is SingletonConstructor sc ? sc : rightOperand.Type;

			yield return new GeneratorExpressionTerminalResult(new Applicate(new DotOperator(new ValueLiteral(type), "::", this.fileName, this.lineNumber), new List<Expression> { new ValueLiteral(leftOperand), new ValueLiteral(rightOperand) }, this.fileName, this.lineNumber).Eval(e));
		}

		public Expression Closure(ClosureManager manager) {
			return new ConsOperator(this.leftExpression.Closure(manager), this.rightExpression.Closure(manager), this.fileName, this.lineNumber);
		}

		public override String ToString() {
			return $"{this.leftExpression} :: {this.rightExpression}";
		}
	}
}