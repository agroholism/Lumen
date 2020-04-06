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

			this.SetMember(Op.UPLUS, new LambdaFun((scope, args) => {
				return scope["this"];
			}) {
				Arguments = Const.This
			});

			this.SetMember(Op.UMINUS, new LambdaFun((scope, args) => {
				if(scope["this"] is BigNumber big) {
					return new BigNumber(big.value.Negate());
				}

				return new Number(-scope["this"].ToDouble(scope));
			}) {
				Arguments = Const.This
			});

			this.SetMember(Op.UNARY_XOR, new LambdaFun((scope, args) => {
				Double value = scope["this"].ToDouble(scope);
				return new Stream(Range(0, value - 1));
			}) {
				Arguments = Const.This
			});

			this.SetMember(Op.BNOT, new LambdaFun((scope, args) => {
				Int32 value = scope["this"].ToInt(scope);

				return new Number(~value);
			}) {
				Arguments = Const.This
			});

			this.SetMember(Op.RANGE_INCLUSIVE, new LambdaFun((scope, args) => {
				Double other = scope["other"].ToDouble(scope);

				return new Stream(Range(scope["this"].ToDouble(scope), other));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.RANGE_EXCLUSIVE, new LambdaFun((scope, args) => {
				Double other = scope["other"].ToDouble(scope);

				return new Stream(Range(scope["this"].ToDouble(scope), other - 1));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("compare", new LambdaFun((scope, args) => {
				return new Number(scope["x"].CompareTo(scope["y"]));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember(Op.PLUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					if (scope["this"] is BigNumber big) {
						return new BigNumber(big.value);
					}

					return new Number(scope["this"].ToDouble(scope));
				}

				if (scope["this"] is BigNumber || other is BigNumber) {
					return new BigNumber(scope["this"].ToBigFloat(scope) + other.ToBigFloat(scope));
				}

				return new Number(scope["this"].ToDouble(scope) + other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.MINUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					if (scope["this"] is BigNumber big) {
						return new BigNumber(big.value.Negate());
					}

					return new Number(-scope["this"].ToDouble(scope));
				}

				if (scope["this"] is BigNumber || other is BigNumber) {
					return new BigNumber(scope["this"].ToBigFloat(scope) - other.ToBigFloat(scope));
				}

				return new Number(scope["this"].ToDouble(scope) - other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.SLASH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope["this"] is BigNumber || other is BigNumber) {
					return new BigNumber(scope["this"].ToBigFloat(scope) / other.ToBigFloat(scope));
				}

				return new Number(scope["this"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.STAR, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope["this"] is BigNumber || other is BigNumber) {
					return new BigNumber(scope["this"].ToBigFloat(scope) * other.ToBigFloat(scope));
				}

				return new Number(scope["this"].ToDouble(scope) * other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.POW, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope["this"] is BigNumber || other is BigNumber) {
					return new BigNumber(Math.Pow((Double)scope["this"].ToBigFloat(scope), other.ToDouble(scope)));
				}

				return new Number(Math.Pow(scope["this"].ToDouble(scope), other.ToDouble(scope)));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.LSH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["this"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne << numTwo);
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember(Op.RSH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["this"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne >> numTwo);
			}) {
				Arguments = Const.ThisOther
			});

			#endregion

			this.SetMember("div", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Number)Math.Truncate(scope["this"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("rem", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["this"].ToDouble(scope) % other.ToDouble(scope));
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("mod", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Double numOne = scope["this"].ToDouble(scope);
				Double numTwo = other.ToDouble(scope);

				return new Number((numOne % numTwo + numTwo) % numTwo);
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("binaryAnd", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["this"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne & numTwo);
			}) {
				Arguments = Const.ThisOther
			});
			this.SetMember("binaryOr", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["this"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne | numTwo);
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("binaryXor", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["this"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne ^ numTwo);
			}) {
				Arguments = Const.ThisOther
			});

			this.SetMember("inc", new LambdaFun((scope, args) => {
				return new Number(scope["this"].ToDouble(scope) + 1);
			}) {
				Arguments = Const.This
			});
			this.SetMember("dec", new LambdaFun((scope, args) => {
				return new Number(scope["this"].ToDouble(scope) - 1);
			}) {
				Arguments = Const.This
			});

			this.SetMember("times", new LambdaFun((scope, args) => {
				Double n = scope["this"].ToDouble(scope);
				Fun action = scope["action"] as Fun;

				for (Double x = 0; x < n; x += 1) {
					action.Run(new Scope(scope) {
						["self"] = action
					}, new Number(x));
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					Const.This[0],
					new NamePattern("action")
				}
			});
			this.SetMember("count", new LambdaFun((scope, args) => {
				Double n = scope["this"].ToDouble(scope);
				Fun func = scope["fn"] as Fun;

				IEnumerable<Value> Iterate() {
					for (Double x = 0; x < n; x += 1) {
						yield return func.Run(new Scope(scope) {
							["self"] = func
						}, new Number(x));
					}
				}

				return new Stream(Iterate());
			}) {
				Arguments = new List<IPattern> {
					Const.This[0],
						new NamePattern("fn")
					}
			});

			// make more
			this.SetMember("round", new LambdaFun((scope, args) => {
				Double num = scope["this"].ToDouble(scope);
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
			this.SetMember("abs", new LambdaFun((scope, args) => {
				return new Number(Math.Abs(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("sqrt", new LambdaFun((scope, args) => {
				if(scope["this"] is BigNumber big) {
					return new BigNumber(BigFloat.Sqrt(big.value));
				}
				return new Number(Math.Sqrt(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("acos", new LambdaFun((scope, args) => {
				return new Number(Math.Acos(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("asin", new LambdaFun((scope, args) => {
				return new Number(Math.Asin(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("atan", new LambdaFun((scope, args) => {
				return new Number(Math.Atan(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("cos", new LambdaFun((scope, args) => {
				return new Number(Math.Cos(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("cosh", new LambdaFun((scope, args) => {
				return new Number(Math.Cosh(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("exp", new LambdaFun((scope, args) => {
				return new Number(Math.Exp(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("log", new LambdaFun((scope, args) => {
				return new Number(Math.Log(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("log10", new LambdaFun((scope, args) => {
				return new Number(Math.Log10(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("sin", new LambdaFun((scope, args) => {
				return new Number(Math.Sin(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});
			this.SetMember("sinh", new LambdaFun((scope, args) => {
				return new Number(Math.Sinh(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});

			this.SetMember("tan", new LambdaFun((scope, args) => {
				return new Number(Math.Tan(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});

			this.SetMember("tanh", new LambdaFun((scope, args) => {
				return new Number(Math.Tanh(scope["this"].ToDouble(scope)));
			}) {
				Arguments = Const.This
			});

			this.SetMember("withBase", new LambdaFun((scope, args) => {
				Value num = scope["this"];
				Double basis = Converter.ToDouble(scope["base"], scope);

				if (basis == 10) {
					return new Text(num.ToString());
				}

				return new Text(Converter.FromTo(num.ToString(), "10", basis.ToString()));
			}) {
				Arguments = new List<IPattern> {
					Const.This[0],
					new NamePattern("base")
				}
			});

			this.SetMember("parse", new LambdaFun((scope, args) => {
				String str = scope["this"].ToString();

				if(Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out var result)) {
					return Helper.CreateSome(new Number(result));
				}

				return Prelude.None;
			}) {
				Arguments = new List<IPattern> {
					Const.This[0]
				}
			});

			#region StandartConverters

			this.SetMember("toText", new LambdaFun((scope, args) => {
				Value num = scope["this"];
				return new Text(num.ToString());
			}) {
				Arguments = new List<IPattern> {
					Const.This[0],
				}
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
