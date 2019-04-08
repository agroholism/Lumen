using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    internal sealed class NumberModule : Module {
        internal NumberModule() {
            this.name = "prelude.Number";

            IEnumerable<Value> Range(Number from, Number to) {
                yield return from;
                if (from < to) {
                    while (from < to) {
                        from += 1;
                        yield return from;
                    }
                } else if (from >= to) {
                    while (from > to) {
                        from -= 1;
                        yield return from;
                    }
                }
            }

            #region operators

            this.SetField(Op.UPLUS, new LambdaFun((scope, args) => {
                return scope.This;
            }) {
                Arguments = Const.This
            });
            this.SetField(Op.UMINUS, new LambdaFun((scope, args) => {
                return -(Number)scope.This;
            }) {
                Arguments = Const.This
            });
            this.SetField(Op.UNARY_XOR, new LambdaFun((scope, args) => {
                Number value = scope.This as Number;
                return new Enumerator(Range(0, value - 1));
            }) {
                Arguments = Const.This
            });
            this.SetField(Op.BNOT, new LambdaFun((scope, args) => {
                Int32 value = scope.This.ToInt(scope);

                return new Number(~value);
            }) {
                Arguments = Const.This
            });

            this.SetField(Op.RANGE_INCLUSIVE, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return new Enumerator(Range(scope.This.ToNum(scope), other.ToNum(scope)));
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.RANGE_EXCLUSIVE, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return new Enumerator(Range(scope.This.ToNum(scope), other.ToNum(scope) - 1));
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("compare", new LambdaFun((scope, args) => {
                return new Number(scope["x"].CompareTo(scope["y"]));
            }) {
                Arguments = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

            this.SetField(Op.PLUS, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return scope.This.ToNum(scope) + other.ToNum(scope);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.MINUS, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return scope.This.ToNum(scope) - other.ToNum(scope);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.SLASH, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return scope.This.ToNum(scope) / other.ToNum(scope);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.STAR, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return scope.This.ToNum(scope) * other.ToNum(scope);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.POW, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return (Number)Math.Pow(scope.This.ToDouble(scope), other.ToDouble(scope));
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.LSH, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    Int32 numOne = scope.This.ToInt(scope);
                    Int32 numTwo = other.ToInt(scope);

                    return new Number(numOne << numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.RSH, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    Int32 numOne = scope.This.ToInt(scope);
                    Int32 numTwo = other.ToInt(scope);

                    return new Number(numOne >> numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            #endregion

            this.SetField("div", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    return (Number)Math.Truncate(scope.This.ToDouble(scope) / other.ToDouble(scope));
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("rem", new LambdaFun((scope, args) => {
                Value other = scope.This;

                if (other is Number) {
                    return new Number(scope["other"].ToDouble(scope) % other.ToDouble(scope));
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("mod", new LambdaFun((scope, args) => {
                Value other = scope["this"];

                if (other is Number) {
                    Double numOne = scope["other"].ToDouble(scope);
                    Double numTwo = other.ToDouble(scope);

                    return new Number((numOne % numTwo + numTwo) % numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("binaryAnd", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    Int32 numOne = scope.This.ToInt(scope);
                    Int32 numTwo = other.ToInt(scope);

                    return new Number(numOne & numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("binaryOr", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    Int32 numOne = scope.This.ToInt(scope);
                    Int32 numTwo = other.ToInt(scope);

                    return new Number(numOne | numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });
            this.SetField("binaryXor", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                if (other is Number) {
                    Int32 numOne = scope.This.ToInt(scope);
                    Int32 numTwo = other.ToInt(scope);

                    return new Number(numOne ^ numTwo);
                }

                throw new LumenException(Exceptions.TYPE_ERROR.F(this, other.Type));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("inc", new LambdaFun((scope, args) => {
                return new Number(scope.This.ToDouble(scope) + 1);
            }) {
                Arguments = Const.This
            });
            this.SetField("dec", new LambdaFun((scope, args) => {
                return new Number(scope.This.ToDouble(scope) - 1);
            }) {
                Arguments = Const.This
            });

            this.SetField("times", new LambdaFun((scope, args) => {
                Number n = (Number)scope.This;
                Fun action = scope["action"] as Fun;

                for (Number x = 0; x < n; x += 1) {
                    action.Run(new Scope(scope) {
                        ["self"] = action
                    }, x);
                }

                return Const.UNIT;
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                    new NamePattern("action")
                }
            });
            this.SetField("count", new LambdaFun((scope, args) => {
                Number n = (Number)scope.This;
                Fun func = scope["fn"] as Fun;

                IEnumerable<Value> Iterate() {
                    for (Number x = 0; x < n; x += 1) {
                        yield return func.Run(new Scope(scope) {
                            ["self"] = func
                        }, x);
                    }
                }

                return new Enumerator(Iterate());
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                        new NamePattern("fn")
                    }
            });

            // make more
            this.SetField("round", new LambdaFun((scope, args) => {
                Double num = scope.This.ToDouble(scope);
                Double to = scope["to"].ToDouble(scope);

                switch (to) {
                    case 1:
                        return new Number(Math.Ceiling(num));
                    case -1:
                        return new Number(Math.Floor(num));
                    default:
                        return new Number(Math.Round(num));
                }
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                    new NamePattern("to")
                }
            });
            this.SetField("abs", new LambdaFun((scope, args) => {
                return new Number(Math.Abs(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("sqrt", new LambdaFun((scope, args) => {
                return new Number(Math.Sqrt(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("acos", new LambdaFun((scope, args) => {
                return new Number(Math.Acos(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("asin", new LambdaFun((scope, args) => {
                return new Number(Math.Asin(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("atan", new LambdaFun((scope, args) => {
                return new Number(Math.Atan(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("cos", new LambdaFun((scope, args) => {
                return new Number(Math.Cos(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("cosh", new LambdaFun((scope, args) => {
                return new Number(Math.Cosh(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("exp", new LambdaFun((scope, args) => {
                return new Number(Math.Exp(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("log", new LambdaFun((scope, args) => {
                return new Number(Math.Log(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("log10", new LambdaFun((scope, args) => {
                return new Number(Math.Log10(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("sin", new LambdaFun((scope, args) => {
                return new Number(Math.Sin(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("sinh", new LambdaFun((scope, args) => {
                return new Number(Math.Sinh(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("tan", new LambdaFun((scope, args) => {
                return new Number(Math.Tan(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });
            this.SetField("tanh", new LambdaFun((scope, args) => {
                return new Number(Math.Tanh(scope.This.ToDouble(scope)));
            }) {
                Arguments = Const.This
            });

            this.SetField("withBase", new LambdaFun((scope, args) => {
                Value num = scope.This;
                Double basis = Converter.ToDouble(scope["base"], scope);

                if (basis == 10) {
                    return new Text(num.ToString(scope));
                }

                return new Text(Converter.FromTo(num.ToString(scope), "10", basis.ToString()));
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                    new NamePattern("base")
                }
            });

            #region StandartConverters

            this.SetField("String", new LambdaFun((scope, args) => {
                Value num = scope.This;
                return new Text(num.ToString(scope));
            }) {
                Arguments = new List<IPattern> {
                    Const.This[0],
                }
            });

            #endregion

            this.Derive(Prelude.Ord);
            this.NameIt();
        }

        private void NameIt() {
            foreach(KeyValuePair<String, Value> i in this.variables) {
                if(i.Value is Fun fun) {
                    fun.Name = "prelude.Number." + i.Key;
                }
            }
        }
    }
}
