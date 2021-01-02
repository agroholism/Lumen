using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	public class UserFun : Fun {
		private Expression body;
		public List<IPattern> Parameters { get; set; } = new List<IPattern>();
		public String Name { get; set; }
		public IType Type => Prelude.Function;

		public UserFun(List<IPattern> arguments, Expression body) {
			this.Parameters = arguments;
			this.body = body;
		}

		public UserFun(List<IPattern> arguments, Expression body, String name) : this(arguments, body) {
			this.Name = name;
		}

		public override String ToString() {
			return this.Name ?? $"[Function {this.GetHashCode()}]";
		}

		public Value Call(Scope e, params Value[] arguments) {
			if (this.Parameters.Count(i => i is not ContextPattern) > arguments.Length) {
				return Helper.MakePartial(this, arguments);
			}

			Int32 counter = 0;
			Int32 offset = 0;

			while (counter < this.Parameters.Count) {
				if (counter - offset >= arguments.Length) {
					return Helper.MakePartial(this, arguments);
				}

				MatchResult match = this.Parameters[counter].Match(arguments[counter - offset], e);

				if (match.Kind == MatchResultKind.Delayed) {
					offset++;
					counter++;
					continue;
				}

				if (match.Kind != MatchResultKind.Success) {
					LumenException exception = Helper.InvalidArgument(
						this.Parameters[counter].ToString(),
						Exceptions.FUNCTION_CAN_NOT_BE_APPLIED.F(String.Join(" ", this.Parameters)));
					exception.Note = $"Details: {match.Note}";
					throw exception;
				}
				counter++;
			}

			while (offset > 0) {
				ContextPattern cp = this.Parameters[offset - 1] as ContextPattern;

				MatchResult match = cp.Match(e[cp.identifier], e);

				if (match.Kind != MatchResultKind.Success) {
					String[] strs = match.Note.Split('|');

					LumenException exception = Helper.InvalidArgument(
						this.Parameters[offset - 1].ToString(),
						strs[0] + ".");

					exception.Note = strs[1];

					throw exception;
				}

				offset--;
			}

			Value result;
			try {
				result = this.body.Eval(e);
			}
			catch (Return rt) {
				result = rt.Result;
			}

			Int32 delta = arguments.Length - this.Parameters.Count;
			if (delta > 0) {
				if (this.Parameters.Count == 0 && arguments[0] == Const.UNIT) {
					return result;
				}

				Int32 position = this.Parameters.Count;
				while (position < arguments.Length) {
					Fun func = result.ToFunction(e);
					result = func.Call(e, arguments[position]);
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
