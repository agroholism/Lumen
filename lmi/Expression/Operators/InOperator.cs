﻿using System;
using System.Collections.Generic;

using Lumen.Lang.Patterns;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	public class InOperator : Expression {
		public Expression expressionOne;
		public Expression expressionTwo;
		public Int32 line;
		public String fileName;

		public InOperator(Expression expressionOne, Expression expressionTwo, Int32 line, String file) {
			this.expressionOne = expressionOne;
			this.expressionTwo = expressionTwo;
			this.line = line;
			this.fileName = file;
		}

		public IValue Eval(Scope e) {
			if (this.expressionOne is IdExpression ide && ide.id == "_") {

				if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
					return new UserFun(
						new List<IPattern> { new NamePattern("x"), new NamePattern("y") },
						new InOperator(new IdExpression("x", ide.file, ide.line), new IdExpression("y", ide2.file, ide2.line), this.line, this.fileName));
				}
				else {
					return new UserFun(new List<IPattern> { new NamePattern("x") },
						new InOperator(new IdExpression("x", ide.file, ide.line), new ValueLiteral(this.expressionTwo.Eval(e)), this.line, this.fileName));
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				return new UserFun(new List<IPattern> { new NamePattern("x") },
					new InOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.file, _ide.line), this.line, this.fileName));
			}

			IValue operandOne = this.expressionOne.Eval(e);
			IValue operandTwo = this.expressionTwo.Eval(e);

			List<Expression> exps = new List<Expression> {
				new ValueLiteral(operandOne),
				new ValueLiteral(operandTwo),
			};

			Scope s = new Scope(e);

			return new Applicate(new DotOperator(new ValueLiteral(operandTwo.Type), "contains", this.fileName, this.line), exps, this.fileName, this.line).Eval(s);
		}

		public IEnumerable<IValue> EvalWithYield(Scope e) {
			if (this.expressionOne is IdExpression ide && ide.id == "_") {
				if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
					yield return new GeneratorExpressionTerminalResult(new UserFun(
						new List<IPattern> { new NamePattern("x"), new NamePattern("y") },
						new InOperator(new IdExpression("x", ide.file, ide.line), new IdExpression("y", ide2.file, ide2.line), this.line, this.fileName)));
				}
				else {
					yield return new GeneratorExpressionTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") },
						new InOperator(new IdExpression("x", ide.file, ide.line), new ValueLiteral(this.expressionTwo.Eval(e)), this.line, this.fileName)));
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				yield return new GeneratorExpressionTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") },
					new InOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.file, _ide.line), this.line, this.fileName)));
			}

			IEnumerable<IValue> ops = this.expressionOne.EvalWithYield(e);
			IValue operandOne = Const.UNIT;

			foreach (IValue i in ops) {
				if (i is GeneratorExpressionTerminalResult cgv1) {
					operandOne = cgv1.Value;
				}
				else {
					yield return i;
				}
			}

			IEnumerable<IValue> ops2 = this.expressionTwo.EvalWithYield(e);
			IValue operandTwo = Const.UNIT;
			foreach (IValue i in ops2) {
				if (i is GeneratorExpressionTerminalResult cgv2) {
					operandTwo = cgv2.Value;
				}
				else {
					yield return i;
				}
			}


			List<Expression> exps = new List<Expression> {
				new ValueLiteral(operandOne),
				new ValueLiteral(operandTwo),
			};

			Scope s = new Scope(e);
			IType type = operandTwo is SingletonConstructor sc ? sc : operandTwo.Type;

			yield return new GeneratorExpressionTerminalResult(new Applicate(new DotOperator(new ValueLiteral(type), "contains", this.fileName, this.line), exps, this.fileName, this.line).Eval(s)); 
		}

		public Expression Closure(ClosureManager manager) {
			return new InOperator(this.expressionOne.Closure(manager), this.expressionTwo.Closure(manager), this.line, this.fileName);
		}

		public override String ToString() {
			return this.expressionOne.ToString() + " in " + this.expressionTwo.ToString();
		}
	}
}