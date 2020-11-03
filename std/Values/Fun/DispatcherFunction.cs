using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class DispatcherFunction : Fun {
		private List<Fun> functions;
		public String Name { get; set; }
		public List<IPattern> Arguments { get; set; }

		public IType Type => Prelude.Function;

		public DispatcherFunction(String name) {
			this.Name = name;
			this.functions = new List<Fun>();
		}

		public DispatcherFunction(String name, Fun first, Fun second) : this(name) {
			this.Append(first);
			this.Append(second);
		}

		public void Append(Fun function) {
			if (!this.functions.Contains(function)) {
				this.functions.Add(function);
			}
		}

		public Value Run(Scope scope, params Value[] args) {
			if (this.Arguments.Count > args.Length) {
				return Helper.MakePartial(this, args);
			}

			foreach (Fun function in this.functions) {
				Scope s = new Scope(scope);
				Boolean isFit = true;
				Int32 counter = 0;

				foreach (Value arg in args) {
					MatchResult m = function.Arguments[counter].Match(arg, s);

					if (!m.Success) {
						isFit = false;
						break;
					}

					counter++;
				}

				if (!isFit) {
					continue;
				}

				Value result = null;
				try {
					result = function.Run(scope, args);
				}
				catch (Return rt) {
					result = rt.Result;
				}

				return result;
			}

			throw new LumenException($"can not to find overload") {
				Note = $"Aviable overloads: {Environment.NewLine}{String.Join(Environment.NewLine, this.functions.Select(i => $"let { this.Name } {String.Join(" ", i.Arguments)} = ..."))}"
			};
		}

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}

		public override String ToString() {
			return $"[Function #{this.GetHashCode()}]";
		}
	}
}
