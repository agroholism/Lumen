using System;
using System.Collections;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class InfinityRange : BaseValueImpl {
		public Double Step { get; private set; }

		public Boolean HasStart => false;
		public Boolean HasEnd => false;
		public Boolean IsInclusive => false;
		public Boolean IsDownToUp => this.Step >= 0;

		public override IType Type => Prelude.Stream;

		public InfinityRange() {
			this.Step = 1;
		}

		public InfinityRange(Double step) {
			this.Step = step;
		}

		public override String ToString() {
			return $"...";
		}
	}

	public class NumberRange : BaseValueImpl, IEnumerable<Value> {
		public Double? Start { get; private set; }
		private Double? end;
		public Double? End =>
			this.IsInclusive && this.end.HasValue
				? (this.IsDownToUp ? this.end.Value + 1 : this.end.Value - 1)
				: this.end;

		public Double Step { get; private set; }

		public Boolean HasStart => this.Start != null;
		public Boolean HasEnd => this.end != null;
		public Boolean IsInclusive { get; private set; }
		public Boolean IsDownToUp => (this.Start ?? 0) <= (this.end ?? Double.PositiveInfinity);

		public override IType Type => Prelude.Stream;

		public NumberRange Clone(Double step) {
			return new NumberRange(this.Start ?? Double.PositiveInfinity,
				this.end ?? Double.PositiveInfinity, step, this.IsInclusive);
		}

		public NumberRange(Double begin, Double end, Boolean isInclusive)
			: this(begin, end, begin <= end ? 1 : -1, isInclusive) { }

		public NumberRange(Double begin, Double end, Double step, Boolean isInclusive) {
			this.Start = Double.IsInfinity(begin) ? null : (Double?)begin;
			this.end = Double.IsInfinity(end) ? null : (Double?)end;
			this.Step = step;
			this.IsInclusive = isInclusive;

			if (this.IsDownToUp && step < 0) {
				// can not be
			}
			else if (step > 0) {
				// can not be
			}
		}

		public Double StartOr(Double or) {
			return this.Start ?? or;
		}

		public Double EndOr(Double or) {
			return this.end ?? or;
		}

		public IEnumerator<Value> GetEnumerator() {
			Double current = this.Start ?? 0;
			Double end = this.End ?? Double.PositiveInfinity;

			if (this.IsDownToUp) {
				while (current < end) {
					yield return new Number(current);
					current += this.Step;
				}
			} else {
				while (current > end) {
					yield return new Number(current);
					current += this.Step;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

		public override String ToString() {
			return $"({this.Start?.ToString() ?? ""}{(this.IsInclusive ? "..." : "..")}{this.end?.ToString() ?? ""}) / {this.Step}";
		}
	}

	internal sealed class NumberModule : Module {
		internal NumberModule() {
			this.Name = "Number";

			#region operators

			// self...other
			this.SetMember(Constants.RANGE_INCLUSIVE, new LambdaFun((scope, args) => {
				Value self = scope["self"];
				Value other = scope["other"];

				Double selfDouble = self == Const.UNIT ? Double.PositiveInfinity : self.ToDouble(scope);
				Double otherDouble = other == Const.UNIT ? Double.PositiveInfinity : other.ToDouble(scope);
	
				return new NumberRange(selfDouble, otherDouble, true);
			}) {
				Parameters = Const.SelfOther
			});

			// self..other
			this.SetMember(Constants.RANGE_EXCLUSIVE, new LambdaFun((scope, args) => {
				Value self = scope["self"];
				Value other = scope["other"];

				Double selfDouble = self == Const.UNIT ? Double.PositiveInfinity : self.ToDouble(scope);
				Double otherDouble = other == Const.UNIT ? Double.PositiveInfinity : other.ToDouble(scope);

				return new NumberRange(selfDouble, otherDouble, false);
			}) {
				Parameters = Const.SelfOther
			});

			// self + other
			// Number -> Number -> Number
			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					return new Number(scope["self"].ToDouble(scope));
				}

				return new Number(scope["self"].ToDouble(scope) + other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// self - other
			// Number -> Number -> Number
			this.SetMember(Constants.MINUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other == Const.UNIT) {
					return new Number(-scope["self"].ToDouble(scope));
				}

				return new Number(scope["self"].ToDouble(scope) - other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// self / other
			// Number -> Number -> Number
			this.SetMember(Constants.SLASH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// self * other
			// Number -> Number -> Number
			this.SetMember(Constants.STAR, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) * other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// self ^ other
			// Number -> Number -> Number
			this.SetMember(Constants.POW, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(Math.Pow(scope["self"].ToDouble(scope), other.ToDouble(scope)));
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember(Constants.MOD, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) % other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			#endregion

			// Number -> Number -> Number
			// Ord class implementation
			this.SetMember("compare", new LambdaFun((scope, args) => {
				return new Number(scope["x"].CompareTo(scope["y"]));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// Number -> Number -> Number
			this.SetMember("div", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return (Number)Math.Truncate(scope["self"].ToDouble(scope) / other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// Number -> Number -> Number
			this.SetMember("rem", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				return new Number(scope["self"].ToDouble(scope) % other.ToDouble(scope));
			}) {
				Parameters = Const.SelfOther
			});

			// Number -> Number -> Number
			this.SetMember("mod", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Double numOne = scope["self"].ToDouble(scope);
				Double numTwo = other.ToDouble(scope);

				return new Number((numOne % numTwo + numTwo) % numTwo);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("binaryNot", new LambdaFun((scope, args) => {
				Int32 value = scope["self"].ToInt(scope);

				return new Number(~value);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("binaryAnd", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne & numTwo);
			}) {
				Parameters = Const.SelfOther
			});
			this.SetMember("binaryOr", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne | numTwo);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("binaryXor", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne ^ numTwo);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("binaryLsh", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne << numTwo);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("binaryRsh", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				Int32 numOne = scope["self"].ToInt(scope);
				Int32 numTwo = other.ToInt(scope);

				return new Number(numOne >> numTwo);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("times", new LambdaFun((scope, args) => {
				Double n = scope["self"].ToDouble(scope);
				Fun action = scope["action"] as Fun;

				for (Double x = 0; x < n; x += 1) {
					action.Call(new Scope(scope) {
						["self"] = action
					}, new Number(x));
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					Const.Self[0],
					new NamePattern("action")
				}
			});

			// make more
			this.SetMember("round", new LambdaFun((scope, args) => {
				Double num = scope["self"].ToDouble(scope);
				Double to = scope["to"].ToDouble(scope);

				return to switch
				{
					1 => new Number(Math.Ceiling(num)),
					-1 => new Number(Math.Floor(num)),
					_ => new Number(Math.Round(num)),
				};
			}) {
				Parameters = new List<IPattern> {
					Const.Self[0],
					new NamePattern("to")
				}
			});
			this.SetMember("abs", new LambdaFun((scope, args) => {
				return new Number(Math.Abs(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("sqrt", new LambdaFun((scope, args) => {
				return new Number(Math.Sqrt(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("acos", new LambdaFun((scope, args) => {
				return new Number(Math.Acos(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("asin", new LambdaFun((scope, args) => {
				return new Number(Math.Asin(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("atan", new LambdaFun((scope, args) => {
				return new Number(Math.Atan(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("cos", new LambdaFun((scope, args) => {
				return new Number(Math.Cos(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("cosh", new LambdaFun((scope, args) => {
				return new Number(Math.Cosh(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("exp", new LambdaFun((scope, args) => {
				return new Number(Math.Exp(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("log", new LambdaFun((scope, args) => {
				return new Number(Math.Log(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("log10", new LambdaFun((scope, args) => {
				return new Number(Math.Log10(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("sin", new LambdaFun((scope, args) => {
				return new Number(Math.Sin(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});
			this.SetMember("sinh", new LambdaFun((scope, args) => {
				return new Number(Math.Sinh(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});

			this.SetMember("tan", new LambdaFun((scope, args) => {
				return new Number(Math.Tan(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});

			this.SetMember("tanh", new LambdaFun((scope, args) => {
				return new Number(Math.Tanh(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});

			this.SetMember("isnan", new LambdaFun((scope, args) => {
				return new Logical(Double.IsNaN(scope["self"].ToDouble(scope)));
			}) {
				Parameters = Const.Self
			});

			/*this.SetMember("withBase", new LambdaFun((scope, args) => {
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
			});*/

			this.SetMember("parse", new LambdaFun((scope, args) => {
				String str = scope["self"].ToString();

				if (Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out Double result)) {
					return Helper.CreateSome(new Number(result));
				}

				return Prelude.None;
			}) {
				Parameters = new List<IPattern> {
					Const.Self[0]
				}
			});

			this.SetMember("toText", new LambdaFun((scope, args) => {
				Value num = scope["self"];
				return new Text(num.ToString());
			}) {
				Parameters = Const.Self
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Parameters = Const.Self
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Number(0);
			}) {
				Parameters = new List<IPattern> { }
			});

			this.AppendImplementation(Prelude.Ord);
			this.AppendImplementation(Prelude.Clone);
			this.AppendImplementation(Prelude.Default);
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
