using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    public class BinaryExpression : Expression {
        public Expression expressionOne;
        public Expression expressionTwo;
        public String operation;
        public Int32 line;
        public String fileName;

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
                return !Converter.ToBoolean(operandOne) ? false : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e));
            }

            if (this.operation == "or") {
                return Converter.ToBoolean(operandOne) ? true : (Bool)Converter.ToBoolean(this.expressionTwo.Eval(e));
            }

            if (this.operation == "xor") {
                return (Bool)(Converter.ToBoolean(operandOne) ^ Converter.ToBoolean(this.expressionTwo.Eval(e)));
            }

            if (this.operation == "@not") {
                return (Bool)!Converter.ToBoolean(operandOne);
            }

            Value operandTwo = this.expressionTwo != null ? this.expressionTwo.Eval(e) : Const.UNIT;

            // SOLVE
            if (operandOne is Instance typ) {
                return ((Fun)typ.GetField(this.operation, e)).Run(new Scope(e), operandOne, operandTwo);
            }

            IObject type = operandOne.Type;
            if (type.TryGetField(this.operation, out var prf) && prf is Fun fun) {
                try {
                    return fun.Run(new Scope(e), operandOne, operandTwo);
                } catch (LumenException lex) {
                    if (lex.file == null) {
                        lex.file = this.fileName;
                    }

                    if (lex.line == -1) {
                        lex.line = this.line;
                    }
                    lex.AddToCallStack(fun.Name, this.fileName, this.line);

                    throw;
                }
            } else {
                throw new LumenException($"value of type {type} does hot have a operator {this.operation}") {
                    line = this.line,
                    file = this.fileName
                };
            }
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return new BinaryExpression(this.expressionOne.Closure(visible, thread), this.expressionTwo?.Closure(visible, thread), this.operation, this.line, this.fileName);
        }

        public override String ToString() {
            if (this.expressionTwo == null) {
                return this.operation.Replace("@", "") + this.expressionOne.ToString();
            }

            return "(" + this.expressionOne.ToString() + " " + this.operation + " " + this.expressionTwo.ToString() + ")";
        }
    }
}