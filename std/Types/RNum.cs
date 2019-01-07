using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	internal sealed class RNum : Record {
		internal RNum() {
			this.meta = new TypeMetadata {
				Name = "std.num"
			};

			IEnumerable<Value> Range(Num from, Num to) {
				yield return from;
				if (from < to) {
					while (from < to) {
						from += 1;
						yield return from;
					}
				}
				else if (from >= to) {
					while (from > to) {
						from -= 1;
						yield return from;
					}
				}
			}

			IEnumerable<Value> BigRange(BigNum from, BigNum to) {
				yield return from;
				if (from < to) {
					while (from < to) {
						from += 1;
						yield return from;
					}
				}
				else if (from >= to) {
					while (from > to) {
						from -= 1;
						yield return from;
					}
				}
			}

			#region operators

			SetAttribute(Op.BXOR, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Num value = scope.This as Num;
				return new Enumerator(Range(0, value - 1));
			}));

			SetAttribute(Op.DOTI, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Enumerator(Range(scope.This.ToNum(scope), other.ToNum(scope)));
				}

				if (other is BigNum) {
					return new Enumerator(BigRange(scope.This.ToBigNum(scope), other.ToBigNum(scope)));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.DOTE, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Enumerator(Range(scope.This.ToNum(scope), other.ToNum(scope) - 1));
				}

				if (other is BigNum) {
					return new Enumerator(BigRange(scope.This.ToBigNum(scope), other.ToBigNum(scope) - 1));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.SHIP, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Num(scope.This.ToNum(scope).CompareTo(other.ToNum(scope)));
				}

				if (other is BigNum) {
					return new Num(scope.This.ToBigNum(scope).CompareTo(other.ToBigNum(scope)));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.LTEQ, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) <= other.ToNum(scope));
				}

				if (other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) <= other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.GTEQ, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) >= other.ToNum(scope));
				}

				if (other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) >= other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.GT, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) > other.ToNum(scope));
				}

				if (other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) > other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.LT, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) < other.ToNum(scope));
				}

				if (other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) < other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.EQL, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) == other.ToNum(scope));
				}

				if (other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) == other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.NOT_EQL, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (other is Num) {
					return new Bool(scope.This.ToNum(scope) != other.ToNum(scope));
				}

				if (scope.This is BigNum || other is BigNum) {
					return new Bool(scope.This.ToBigNum(scope) != other.ToBigNum(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});

			SetAttribute(Op.PLUS, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (scope.This is BigNum || other is BigNum) {
					return scope.This.ToBigNum(scope) + other.ToBigNum(scope);
				}

				if (other is Num) {
					return scope.This.ToNum(scope) + other.ToNum(scope);
				}

				if (other is Str) {
					return new Str(scope.This.ToString(scope) + other.ToString(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") { Attributes = new Dictionary<String, Value> { ["type"] = this } } }
			});
			SetAttribute(Op.UPLUS, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				return scope.This;
			}));
			SetAttribute(Op.MINUS, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope.This is BigNum || other is BigNum) {
					return scope.This.ToBigNum(scope) - other.ToBigNum(scope);
				}

				if (other is Num) {
					return scope.This.ToNum(scope) - other.ToNum(scope);
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute(Op.UMINUS, new LambdaFun((scope, args) => {
				if (scope.This is BigNum) {
					return -scope.This.ToBigNum(scope);
				}

				return -(Num)scope.This;
			}));
			/*	SetAttribute("@~", new LambdaFun((scope, args) => {
					return new Num(~(Int32)Converter.ToBigFloat(scope.This, scope));
				}));*/
			SetAttribute(Op.SLASH, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope.This is BigNum || other is BigNum) {
					return scope.This.ToBigNum(scope) / other.ToBigNum(scope);
				}

				if (other is Num) {
					return scope.This.ToNum(scope) / other.ToNum(scope);
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute(Op.STAR, new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (scope.This is BigNum || other is BigNum) {
					return scope.This.ToBigNum(scope) * other.ToBigNum(scope);
				}

				if (other is Num) {
					return scope.This.ToNum(scope) * other.ToNum(scope);
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute(Op.POW, new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value other = scope["other"];

				if (scope.This is BigNum || other is BigNum) {
					return new BigNum(BigFloat.Pow(scope.This.ToBigFloat(scope), (Int32)other.ToDouble(scope)));
				}

				if (other is Num) {
					return (Num)Math.Pow(scope.This.ToDouble(scope), other.ToDouble(scope));
				}

				throw new Exception(Exceptions.TYPE_ERROR.F(this, other.Type), stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			/*SetAttribute("//", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (numberOne - (numberOne % numberTwo)) / numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("-")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("-", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute("%", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = numberOne % numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("%")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("%", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute("<<", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (Int32)numberOne << (Int32)numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("<<")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("<<", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute(">>", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (Int32)numberOne >> (Int32)numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists(">>")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute(">>", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute("&", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (Int32)numberOne & (Int32)numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("&")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("&", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute("|", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (Int32)numberOne | (Int32)numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("|")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("|", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});
			SetAttribute("^", new LambdaFun((scope, args) => {
				Value other = scope["other"];

				if (other is Num) {
					Double numberOne = Converter.ToDouble(scope.This, scope);
					Double numberTwo = Converter.ToDouble(other, scope);

					if (numberOne == Double.NaN || numberTwo == Double.NaN) {
						return (Num)Double.NaN;
					}

					if (numberOne == Double.PositiveInfinity || numberTwo == Double.PositiveInfinity) {
						return (Num)Double.PositiveInfinity;
					}

					if (numberOne == Double.NegativeInfinity || numberTwo == Double.NegativeInfinity) {
						return (Num)Double.NegativeInfinity;
					}

					Double result = (Int32)numberOne ^ (Int32)numberTwo;

					return (Num)result;
				}

				if (other.Type.AttributeExists("^")) {
					args[0] = scope.This;
					scope.This = other;
					return other.Type.GetAttribute("^", scope).Run(scope, args);
				}

				throw new Exception($"expected value of type '{this}', passed value of type '{other.Type}'", stack: scope);
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("other") }
			});*/
			#endregion

			SetAttribute("times", new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Num n = (Num)scope.This;
				Value action = scope["action"];

				for (Num x = 0; x < n; x += 1) {
					StandartModule.Call(action, new Scope(scope), x);
				}

				return Const.VOID;
			}) {
				Arguments = new List<FunctionArgument> {
					new FunctionArgument("action")
				}
			});
			SetAttribute("count", new LambdaFun((scope, args) => {
				Num n = (Num)scope.This;
				Fun func = scope["func"] as Fun;

				IEnumerable<Value> Iterate() {
					for (Num x = 0; x < n; x += 1) {
						yield return func.Run(new Scope(scope), x);
					}
				}

				return new Enumerator(Iterate());
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("func")
					}
			});

			SetAttribute("sin", new LambdaFun((e, args) => {
				return new Num(Math.Sin(Converter.ToDouble(e.This, e)));
			}, "Kernel.Math.sin") {
			});

			/*SetAttribute("round", new LambdaFun((scope, args) => {
				Double num = Converter.ToBigFloat(scope.This, scope);
				String to = scope["to"].ToString(scope);

				switch (to) {
					case "up":
						return new Num(Math.Ceiling(num));
					case "down":
						return new Num(Math.Floor(num));
					default:
						return new Num(Math.Round(num));
				}

			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("to", (KString)"even")
					}
			});
			*/
			/*SetAttribute("abs", new LambdaFun((scope, args) => {
				return new Num(((Num)scope.This).value.Abs());
			}));
			*/
			/*	SetAttribute("ceil", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Num)Math.Ceiling(num);
				}));
				SetAttribute("floor", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Num)Math.Floor(num);
				}));
				SetAttribute("round", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);
					Int32 z = (Int32)Converter.ToDouble(scope["z"], scope);

					return (Num)Math.Round(num, z);
				}) {
					Arguments = new List<FunctionArgument> {
							new FunctionArgument("z", (Num)0)                    }
				});

				SetAttribute("sqrt", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Num)Math.Sqrt(num);
				}));

				SetAttribute("get_int?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num % 1 == 0);
				}));

				SetAttribute("get_odd?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num % 2 != 0);
				}));
				SetAttribute("get_even?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num % 2 == 0);
				}));

				SetAttribute("get_positive?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num > 0);
				}));
				SetAttribute("get_negative?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num < 0);
				}));
				SetAttribute("get_zero?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num == 0);
				}));
				SetAttribute("get_nonzero?", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return (Bool)(num != 0);
				}));

				SetAttribute("get_char", new LambdaFun((scope, args) => {
					try {
						return new KString(Char.ConvertFromUtf32((Int32)Converter.ToDouble(scope.This, scope)).ToString());
					}
					catch (ArgumentOutOfRangeException) {
						throw new Exception("out of utf-32 range", stack: scope);
					}
				}));
				SetAttribute("get_bytes", new LambdaFun((scope, args) => {
					Double num = Converter.ToDouble(scope.This, scope);

					return new List(new BitArray(BitConverter.GetBytes(num)).Cast<Boolean>().Select(x => (Value)new Num(x ? 1 : 0)).ToList());
				}));*/
			SetAttribute("to_s", new LambdaFun((scope, args) => {
				Checker.OfType(scope.This, StandartModule.Num, scope);

				Value num = scope.This;
				Double basis = Converter.ToDouble(scope["base"], scope);

				if (basis == 10) {
					return new Str(num.ToString(scope));
				}

				return new Str(Converter.FromTo(num.ToString(scope), "10", basis.ToString()));
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("base", (Num)10)
					}
			});
		}
	}
}
