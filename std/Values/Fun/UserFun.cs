using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class UserFun : Fun {
		public Expression Body;
		public List<IPattern> Arguments { get; set; } = new List<IPattern>();
		public String Name { get; set; }
		public IType Type => Prelude.Function;

		public UserFun(List<IPattern> arguments, Expression Body) {
			this.Arguments = arguments;
			this.Body = Body;
		}

		public UserFun(List<IPattern> arguments, Expression Body, String name) : this(arguments, Body) {
			this.Name = name;
		}

		public override String ToString() {
			return this.Name ?? $"[Function {this.GetHashCode()}]";
		}

		public Value Run(Scope e, params Value[] arguments) {
			if (this.Arguments.Count > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			Int32 counter = 0;

			while (counter < this.Arguments.Count) {
				MatchResult match = this.Arguments[counter].Match(arguments[counter], e);
				if (!match.Success) {
					throw new LumenException(Exceptions.FUNCTION_CAN_NOT_BE_APPLIED.F(String.Join(" ", this.Arguments))) {
						Note = $"Details: {match.Note}"
					};
				}
				counter++;
			}

			Value result;
			try {
				result = this.Body.Eval(e);
			}
			catch (Return rt) {
				result = rt.Result;
			}

			Int32 delta = arguments.Length - this.Arguments.Count;
			if (delta > 0) {
				if (this.Arguments.Count == 0 && arguments[0] == Const.UNIT) {
					return result;
				}

				Int32 position = this.Arguments.Count;
				while (position < arguments.Length) {
					Fun func = result.ToFunction(e);
					result = func.Run(e, arguments[position]);
					position++;
				}
			}

			return result;
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return "user fun";
		}
	}
}
