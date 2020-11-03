using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	public class BinaryOperator : Expression {
		public Expression expressionOne;
		public Expression expressionTwo;
		public String operation;
		public Int32 line;
		public String fileName;

		public BinaryOperator(Expression expressionOne, Expression expressionTwo, String operation, Int32 line, String file) {
			this.expressionOne = expressionOne;
			this.expressionTwo = expressionTwo;
			this.operation = operation;
			this.line = line;
			this.fileName = file;
		}

		public Value Eval(Scope e) {
			if (this.expressionOne is IdExpression ide) {
				if (ide.id == "_") {
					if (this.expressionTwo == null) {
						return new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryOperator(new IdExpression("x", ide.line, ide.file), null, this.operation, this.line, this.fileName));
					}

					if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
						return new UserFun(new List<IPattern> { new NamePattern("x"), new NamePattern("y") }, new BinaryOperator(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), this.operation, this.line, this.fileName));
					}
					else {
						return new UserFun(new List<IPattern> { new NamePattern("x") },
							new BinaryOperator(new IdExpression("x", ide.line, ide.file), new ValueLiteral(this.expressionTwo.Eval(e)), this.operation, this.line, this.fileName));
					}
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				return new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.line, _ide.file), this.operation, this.line, this.fileName));
			}

			Value operandOne = this.expressionOne.Eval(e);

			if (this.operation == "and") {
				return !Converter.ToBoolean(operandOne) ? false : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e));
			}

			if (this.operation == "or") {
				return Converter.ToBoolean(operandOne) ? true : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e));
			}

			if (this.operation == "xor") {
				return (Bool)(Converter.ToBoolean(operandOne) ^ Converter.ToBoolean(this.expressionTwo.Eval(e)));
			}

			if (this.operation == Op.NOT) {
				return (Bool)!Converter.ToBoolean(operandOne);
			}

			Value operandTwo = this.expressionTwo != null ? this.expressionTwo.Eval(e) : Const.UNIT;

			List<Expression> exps = new List<Expression> { new ValueLiteral(operandOne) };
			if (this.expressionTwo != null) {
				exps.Add(new ValueLiteral(operandTwo));
			}
			else {
				exps.Add(new ValueLiteral(Const.UNIT));
			}

			Scope s = new Scope(e);
			IType type = operandOne is SingletonConstructor sc ? sc : operandOne.Type;

			return new Applicate(new DotOperator(new ValueLiteral(type), this.operation, this.fileName, this.line), exps, this.line, this.fileName).Eval(s);
		}

		public IEnumerable<Value> EvalWithYield(Scope e) {
			if (this.expressionOne is IdExpression ide) {
				if (ide.id == "_") {
					if (this.expressionTwo == null) {
						yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryOperator(new IdExpression("x", ide.line, ide.file), null, this.operation, this.line, this.fileName)));
					}

					if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
						yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x"), new NamePattern("y") }, new BinaryOperator(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), this.operation, this.line, this.fileName)));
					}
					else {
						yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") },
							new BinaryOperator(new IdExpression("x", ide.line, ide.file), new ValueLiteral(this.expressionTwo.Eval(e)), this.operation, this.line, this.fileName)));
					}
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				yield return new GeneratorTerminalResult(new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryOperator(new ValueLiteral(this.expressionOne.Eval(e)), new IdExpression("x", _ide.line, _ide.file), this.operation, this.line, this.fileName)));
			}

			IEnumerable<Value> ops = this.expressionOne.EvalWithYield(e);
			Value operandOne = Const.UNIT;

			foreach (var i in ops) {
				if (i is GeneratorTerminalResult cgv1) {
					operandOne = cgv1.Value;
				}
				else {
					yield return i;
				}
			}

			Value operandTwo = Const.UNIT;

			if (this.expressionTwo != null) {
				IEnumerable<Value> ops2 = this.expressionTwo.EvalWithYield(e);
				foreach (var i in ops2) {
					if (i is GeneratorTerminalResult cgv2) {
						operandTwo = cgv2.Value;
					}
					else {
						yield return i;
					}
				}
			}

			if (this.operation == "and") {
				yield return new GeneratorTerminalResult(!Converter.ToBoolean(operandOne) ? false : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e)));
			}

			if (this.operation == "or") {
				yield return new GeneratorTerminalResult(Converter.ToBoolean(operandOne) ? true : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e)));
			}

			if (this.operation == "xor") {
				yield return new GeneratorTerminalResult((Bool)(Converter.ToBoolean(operandOne) ^ Converter.ToBoolean(this.expressionTwo.Eval(e))));
			}

			if (this.operation == "@not") {
				yield return new GeneratorTerminalResult((Bool)!Converter.ToBoolean(operandOne));
			}

			List<Expression> exps = new List<Expression> { new ValueLiteral(operandOne) };
			if (this.expressionTwo != null) {
				exps.Add(new ValueLiteral(operandTwo));
			}
			else {
				exps.Add(new ValueLiteral(Const.UNIT));
			}

			Scope s = new Scope(e);
			IType type = operandOne is SingletonConstructor sc ? sc : operandOne.Type;

			yield return new GeneratorTerminalResult(new Applicate(new DotOperator(new ValueLiteral(type), this.operation, this.fileName, this.line), exps, this.line, this.fileName).Eval(s));
		}

		public Expression Closure(ClosureManager manager) {
			return new BinaryOperator(this.expressionOne.Closure(manager), this.expressionTwo?.Closure(manager), this.operation, this.line, this.fileName);
		}

		public override String ToString() {
			if (this.expressionTwo == null) {
				return this.operation.Replace("@", "") + this.expressionOne.ToString();
			}

			return this.expressionOne.ToString() + " " + this.operation + " " + this.expressionTwo.ToString();
		}
	}
}