using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    public class BinaryExpression : Expression {
        public Expression expressionOne;
        public Expression expressionTwo;
        public String operation;
        public Int32 line;
        public String fileName;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public BinaryExpression(Expression expressionOne, Expression expressionTwo, String operation, Int32 line, String file) {
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
                        return new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryExpression(new IdExpression("x", ide.line, ide.file), null, this.operation, this.line, this.fileName));
                    }

                    if (this.expressionTwo is IdExpression ide2 && ide2.id == "_") {
                        return new UserFun(new List<IPattern> { new NamePattern("x"), new NamePattern("y") }, new BinaryExpression(new IdExpression("x", ide.line, ide.file), new IdExpression("y", ide2.line, ide2.file), this.operation, this.line, this.fileName));
                    } else {
                        return new UserFun(new List<IPattern> { new NamePattern("x") },
                            new BinaryExpression(new IdExpression("x", ide.line, ide.file), new ValueE(this.expressionTwo.Eval(e)), this.operation, this.line, this.fileName));
                    }
                }
            } else if (this.expressionTwo is IdExpression _ide && _ide.id == "_") {
                return new UserFun(new List<IPattern> { new NamePattern("x") }, new BinaryExpression(new ValueE(this.expressionOne.Eval(e)), new IdExpression("x", _ide.line, _ide.file), this.operation, this.line, this.fileName));
            }

            Value operandOne = this.expressionOne.Eval(e);

            if (this.operation == "and") {
                return !Converter.ToBoolean(operandOne) ? false : (Logical)Converter.ToBoolean(this.expressionTwo.Eval(e));
            }

            if (this.operation == "or") {
                return Converter.ToBoolean(operandOne) ? true : (Logical)Converter.ToBoolean(this.expressionTwo.Eval(e));
            }

            if (this.operation == "xor") {
                return (Logical)(Converter.ToBoolean(operandOne) ^ Converter.ToBoolean(this.expressionTwo.Eval(e)));
            }

            if (this.operation == "@not") {
                return (Logical)!Converter.ToBoolean(operandOne);
            }

			Value operandTwo = this.expressionTwo != null ? this.expressionTwo.Eval(e) : Const.UNIT;

			List<Expression> exps = new List<Expression> { new ValueE(operandOne) };
			if(this.expressionTwo != null) {
				exps.Add(new ValueE(operandTwo));
			}

			Scope s = new Scope(e);
			s.Bind("'T", operandOne.Type);
            IType type = operandOne.Type;
            return new Applicate(new DotExpression(new ValueE(type), this.operation, this.fileName, this.line), exps, this.line, this.fileName).Eval(s);
        }

        public Expression Closure(ClosureManager manager) {
            return new BinaryExpression(this.expressionOne.Closure(manager), this.expressionTwo?.Closure(manager), this.operation, this.line, this.fileName);
        }

        public override String ToString() {
            if (this.expressionTwo == null) {
                return this.operation.Replace("@", "") + this.expressionOne.ToString();
            }

            return "(" + this.expressionOne.ToString() + " " + this.operation + " " + this.expressionTwo.ToString() + ")";
        }
    }
}