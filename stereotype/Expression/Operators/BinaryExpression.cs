using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class BinaryExpression : Expression {
		public Expression expressionOne;
		public Expression expressionTwo;
		public String operation;
		public Int32 line;
		public String file;

		public BinaryExpression(Expression expressionOne, Expression expressionTwo, String operation, Int32 line, String file) {
			this.expressionOne = expressionOne;
			this.expressionTwo = expressionTwo;
			this.operation = operation;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope e) {
			if (this.expressionOne is IdExpression ide) {
				if (ide.id == "_") {
					if (expressionTwo == null) {
						return new UserFun(new List<FunctionArgument> { new FunctionArgument("x") }, new BinaryExpression(new IdExpression("x", ide.line, ide.file), null, operation, line, file));
					}

					if (expressionTwo is IdExpression ide2 && ide2.id == "_")
						return new UserFun(new List<FunctionArgument> { new FunctionArgument("x"), new FunctionArgument("y") }, new BinaryExpression(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), operation, line, file));
					else
						return new UserFun(new List<FunctionArgument> { new FunctionArgument("x") }, new BinaryExpression(new IdExpression("x", ide.line, ide.file), expressionTwo, operation, line, file));
				}
			}
			else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
				return new UserFun(new List<FunctionArgument> { new FunctionArgument("x") }, new BinaryExpression(expressionOne, new IdExpression("x", _ide.line, _ide.file), operation, line, file));
			}

			Value operandOne = this.expressionOne.Eval(e);

			if (this.operation == "and") {
				return !Converter.ToBoolean(operandOne) ? false : (Bool)Converter.ToBoolean(expressionTwo.Eval(e));
			}

			if (this.operation == "or") {
				return Converter.ToBoolean(operandOne) ? true : (Bool)Converter.ToBoolean(expressionTwo.Eval(e));
			}

			if (this.operation == "xor") {
				return (Bool)(Converter.ToBoolean(operandOne) ^ Converter.ToBoolean(expressionTwo.Eval(e)));
			}

			if (this.operation == "@not") {
				return (Bool)!Converter.ToBoolean(operandOne);
			}

			Value operandTwo = this.expressionTwo != null ? this.expressionTwo.Eval(e) : Const.NULL;

			// SOLVE
			if (operandOne is Expando typ) {
				var a = new Scope();
				a.Set("this", operandOne);
				if (typ.IsExists(operation))
					return ((Fun)typ.Get(operation, AccessModifiers.PUBLIC, e)).Run(a, operandTwo);
				return ((Fun)((Expando)operandOne).Get(operation, AccessModifiers.PUBLIC, e)).Run(a, operandTwo);
			}

			Record type = operandOne.Type;
			if (type.AttributeExists(this.operation)) {
				try {
					return type.GetAttribute(this.operation, e).Run(new Scope(e) { ["this"] = operandOne }, operandTwo);
				}
				catch (Lumen.Lang.Std.Exception hex) {
					if (hex.line == -1) {
						hex.line = this.line;
					}

					if (hex.file == null) {
						hex.file = this.file;
					}

					throw hex;
				}
			}
			else {
				throw new Lumen.Lang.Std.Exception($"value of type {type.meta.Name} does hot have a operator {this.operation}", stack: e) {
					line = this.line,
					file = this.file
				};
			}
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new BinaryExpression(this.expressionOne.Closure(visible, thread), this.expressionTwo?.Closure(visible, thread), this.operation, this.line, this.file);
		}

		public Expression Optimize(Scope scope) {
			Expression opt1 = this.expressionOne.Optimize(scope);
			Expression opt2 = this.expressionTwo.Optimize(scope);

			/*if(scope is OptimizationScope os) {
				if(opt1 is IdExpression ide1 && os.whileConstants.Contains(ide1.id)) {
					if(opt2 is ValueE _ve1) {
						return new BinaryExpression(os.constsValues[ide1.id], opt2, this.operation, this.line, this.file).Optimize(scope);
					}
				}

			}
			*/
		/*	if (opt1 is ValueE ve1 && opt2 is ValueE ve2) {
				if (ve1.val is Num n1 && ve2.val is Num n2 && operation == "+") {
					return new ValueE((Double)n1 + n2);
				}
			}
			*/
			/*if (opt1 is StringE && opt2 is StringE se2) {
				se1 = (StringE)opt1;
				return new StringE(se1.text + se2.text);
			}

			if (opt1 is StringE && opt2 is ValueE) {
				se1 = (StringE)opt1;
				ve2 = (ValueE)opt2;
				if (ve2.val is Number) {
					return new StringE(se1.text + ve2.val);
				}
			}
			*/
			return new BinaryExpression(opt1, opt2, operation, line, file);
		}

		public override String ToString() {
			if (this.expressionTwo == null) {
				return this.operation.Replace("@", "") + this.expressionOne.ToString();
			}

			return "(" + this.expressionOne.ToString() + " " + this.operation + " " + this.expressionTwo.ToString() + ")";
		}
	}
}