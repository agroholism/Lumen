using System;
using System.Collections.Generic;

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

		public Value Eval(Scope e) {
			if (this.expressionOne is IdExpression ide && ide.id == "_") {

				if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
					return new UserFun(
						new List<IPattern> { new NamePattern("x"), new NamePattern("y") },
						new InOperator(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), this.line, this.fileName));
				}
				else {
					return new UserFun(new List<IPattern> { new NamePattern("x") },
						new InOperator(new IdExpression("x", ide.line, ide.file), new ValueLiteral(this.expressionTwo.Eval(e)), this.line, this.fileName));
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				return new UserFun(new List<IPattern> { new NamePattern("x") },
					new InOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.line, _ide.file), this.line, this.fileName));
			}

			Value operandOne = this.expressionOne.Eval(e);
			Value operandTwo = this.expressionTwo.Eval(e);

			List<Expression> exps = new List<Expression> {
				new ValueLiteral(operandOne),
				new ValueLiteral(operandTwo),
			};

			Scope s = new Scope(e);
			IType type = operandTwo is SingletonConstructor sc ? sc : operandTwo.Type;

			return new Applicate(new DotOperator(new ValueLiteral(type), "contains", this.fileName, this.line), exps, this.line, this.fileName).Eval(s);
		}

		public IEnumerable<Value> EvalWithYield(Scope e) {
			if (this.expressionOne is IdExpression ide && ide.id == "_") {
				if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
					yield return new GeneratorTerminalResult(new UserFun(
						new List<IPattern> { new NamePattern("x"), new NamePattern("y") },
						new InOperator(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), this.line, this.fileName)));
				}
				else {
					yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") },
						new InOperator(new IdExpression("x", ide.line, ide.file), new ValueLiteral(this.expressionTwo.Eval(e)), this.line, this.fileName)));
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") },
					new InOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.line, _ide.file), this.line, this.fileName)));
			}

			IEnumerable<Value> ops = this.expressionOne.EvalWithYield(e);
			Value operandOne = Const.UNIT;

			foreach (Value i in ops) {
				if (i is GeneratorTerminalResult cgv1) {
					operandOne = cgv1.Value;
				}
				else {
					yield return i;
				}
			}

			IEnumerable<Value> ops2 = this.expressionTwo.EvalWithYield(e);
			Value operandTwo = Const.UNIT;
			foreach (var i in ops2) {
				if (i is GeneratorTerminalResult cgv2) {
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

			yield return new GeneratorTerminalResult(new Applicate(new DotOperator(new ValueLiteral(type), "contains", this.fileName, this.line), exps, this.line, this.fileName).Eval(s)); 
		}

		public Expression Closure(ClosureManager manager) {
			return new InOperator(this.expressionOne.Closure(manager), this.expressionTwo.Closure(manager), this.line, this.fileName);
		}

		public override String ToString() {
			return this.expressionOne.ToString() + " in " + this.expressionTwo.ToString();
		}
	}
}