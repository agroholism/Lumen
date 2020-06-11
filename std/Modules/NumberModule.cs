using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	internal sealed class NumberModule : Module {
		internal NumberModule() {
			this.Name = "Number";

			IEnumerable<Value> Range(Double from, Double to) {
				yield return new Number(from);
				if (from < to) {
					while (from < to) {
						from += 1;
						yield return new Number(from);
					}
				}
				else if (from >= to) {
					while (from > to) {
						from -= 1;
						yield return new Number(from);
					}
				}
			}

			#region operators

			// self...other
			// Number -> Number -> Stream
			this.SetMember(Op.RANGE_INCLUSIVE, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				/*if (other == Const.UNIT) {
					return new Stream(Range(scope["self"].ToDouble(scope), Double.PositiveInfinity));
				}*/

				return new Stream(Range(scope["self"].ToDouble(scope), other.ToDouble(scope)));
			}) {
				Arguments = Const.SelfOther
			});

			// self..other
			// Number -> Number -> Stream
			this.SetMember(Op.RANGE_EXCLUSIVE, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				/*if (other == Const.UNIT) {
					return new Stream(Range(scope["self"].ToDouble(scope), Double.PositiveInfinity));
				}*/

				return new Stream(Range(scope["self"].ToDouble(scope), other.ToDouble(scope) - 1));
			}) {
				Arguments = Const.SelfOther
			});

			// self + other
			// Number -> Number -> Number
			this.SetMember(Op.PLUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					return new Number(scope["self"].ToDouble(scope));
				}

				return new Number(scope["self"].ToDouble(scope) + other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// self - other
			// Number -> Number -> Number
			this.SetMember(Op.MINUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					return new Number(-scope["self"].ToDouble(scope));
				}

				return new Number(scope["self"].ToDouble(scope) - other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// self / other
			// Number -> Number -> Number
			this.SetMember(Op.SLASH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// self * other
			// Number -> Number -> Number
			this.SetMember(Op.STAR, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) * other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// self ^ other
			// Number -> Number -> Number
			this.SetMember(Op.POW, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(Math.Pow(scope["self"].ToDouble(scope), other.ToDouble(scope)));
			}) {
				Arguments = Const.SelfOther
			});

			#endregion

			// Number -> Number -> Number
			// Ord class implementation
			this.SetMember("compare", new LambdaFun((scope, args) => {
				return new Number(scope["x"].CompareTo(scope["y"]));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// Number -> Number -> Number
			this.SetMember("div", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Number)Math.Truncate(scope["self"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// Number -> Number -> Number
			this.SetMember("rem", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) % other.ToDouble(scope));
			}) {
				Arguments = Const.SelfOther
			});

			// Number -> Number -> Number
			this.SetMember("mod", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Double numOne = scope["self"].ToDouble(scope);
				Double numTwo = other.ToDouble(scope);

				return new Number((numOne % numTwo + numTwo) % numTwo);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("binaryNot", new LambdaFun((scope, args) => {
				Int32 value = scope["self"].ToInt(scope);

				return new Number(~value);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("binaryAnd", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne & numTwo);
			}) {
				Arguments = Const.SelfOther
			});
			this.SetMember("binaryOr", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne | numTwo);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("binaryXor", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne ^ numTwo);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("binaryLsh", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne << numTwo);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("binaryRsh", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne >> numTwo);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("times", new LambdaFun((scope, args) => {
				Double n = scope["self"].ToDouble(scope);
				Fun action = scope["action"] as Fun;

				for (Double x = 0; x < n; x += 1) {
					action.Run(new Scope(scope) {
						["self"] = action
					}, new Number(x));
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					Const.Self[0],
					new NamePattern("action")
				}
			});

			// make more
			this.SetMember("round", new LambdaFun((scope, args) => {
				Double num = scope["self"].ToDouble(scope);
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
					Const.Self[0],
					new NamePattern("to")
				}
			});
			this.SetMember("abs", new LambdaFun((scope, args) => {
				return new Number(Math.Abs(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("sqrt", new LambdaFun((scope, args) => {
				return new Number(Math.Sqrt(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("acos", new LambdaFun((scope, args) => {
				return new Number(Math.Acos(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("asin", new LambdaFun((scope, args) => {
				return new Number(Math.Asin(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("atan", new LambdaFun((scope, args) => {
				return new Number(Math.Atan(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("cos", new LambdaFun((scope, args) => {
				return new Number(Math.Cos(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("cosh", new LambdaFun((scope, args) => {
				return new Number(Math.Cosh(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("exp", new LambdaFun((scope, args) => {
				return new Number(Math.Exp(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("log", new LambdaFun((scope, args) => {
				return new Number(Math.Log(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("log10", new LambdaFun((scope, args) => {
				return new Number(Math.Log10(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("sin", new LambdaFun((scope, args) => {
				return new Number(Math.Sin(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});
			this.SetMember("sinh", new LambdaFun((scope, args) => {
				return new Number(Math.Sinh(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});

			this.SetMember("tan", new LambdaFun((scope, args) => {
				return new Number(Math.Tan(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});

			this.SetMember("tanh", new LambdaFun((scope, args) => {
				return new Number(Math.Tanh(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});

			this.SetMember("isnan", new LambdaFun((scope, args) => {
				return new Bool(Double.IsNaN(scope["self"].ToDouble(scope)));
			}) {
				Arguments = Const.Self
			});

			this.SetMember("withBase", new LambdaFun((scope, args) => {
				Value num = scope["self"];
				Double basis = Converter.ToDouble(scope["base"], scope);

				if (basis == 10) {
					return new Text(num.ToString());
				}

				return new Text(Converter.FromTo(num.ToString(), "10", basis.ToString()));
			}) {
				Arguments = new List<IPattern> {
					Const.Self[0],
					new NamePattern("base")
				}
			});

			this.SetMember("parse", new LambdaFun((scope, args) => {
				String str = scope["self"].ToString();

				if(Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var result)) {
					return Helper.CreateSome(new Number(result));
				}

				return Prelude.None;
			}) {
				Arguments = new List<IPattern> {
					Const.Self[0]
				}
			});

			#region StandartConverters

			this.SetMember("toText", new LambdaFun((scope, args) => {
				Value num = scope["self"];
				return new Text(num.ToString());
			}) {
				Arguments = Const.Self
			});

			#endregion

			this.IncludeMixin(Prelude.Ord);
			this.NameIt();
		}

		private void NameIt() {
			foreach (KeyValuePair<String, Value> i in this.Members) {
				if (i.Value is Fun fun) {
					fun.Name = "Number." + i.Key;
				}
			}
		}
	}
}
