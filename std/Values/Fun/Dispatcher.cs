using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class Dispatcher : Fun {
		internal List<Fun> functions = new List<Fun>();
		public String Name { get; set; }

		public List<IPattern> Arguments { get; set; }

		public IType Type => Prelude.Function;

		public Dispatcher(String name) {
			this.Name = name;
		}

		public Dispatcher(String name, Fun first, Fun second) {
			this.Name = name;
			this.Append(first);
			this.Append(second);
		}

		public void Append(Fun f) {
			if (!this.functions.Contains(f)) {
				this.functions.Add(f);
			}
		}

		public Value Run(Scope scope, params Value[] args) {
			foreach (Fun i in this.functions) {
				if (i.Arguments.Count != args.Length) {
					continue;
				}

				Int32 counter = 0;

				Scope s = new Scope(scope);

				Boolean ok = false;

				foreach (IPattern j in i.Arguments) {
					MatchResult m = j.Match(args[counter], s);

					if (!m.Success) {
						ok = true;
						break;
					}
					counter++;
				}

				if (ok) {
					continue;
				}

				Value result = null;
				try {
					result = i.Run(scope, args);
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

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString(null);
		}

		public String ToString(Scope scope) {
			return "dispatcher";
		}
	}
}
