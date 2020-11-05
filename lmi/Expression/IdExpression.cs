using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	public class IdExpression : Expression {
		public String id;
		public String file;
		public Int32 line;

		public IdExpression(String id, String file, Int32 line) {
			this.id = id;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			if (!scope.IsExsists(this.id)) {
				List<String> maybe = scope.FindClosestNames(5, this.id);

				String note = null;

				if (maybe.Count == 1) {
					note = $"Perhaps you meant '{maybe[0]}'?";
				}
				else if (maybe.Count > 3) {
					note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe.Take(3))}";
				}
				else if (maybe.Count > 1) {
					note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe)}";
				}

				throw new LumenException(Exceptions.UNKNOWN_NAME.F(this.id), this.line, this.file) {
					Note = note
				};
			}

			return scope.Get(this.id);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			if (!manager.IsDeclared(this.id)) {
				if (!manager.Scope.IsExsists(this.id)) {
					List<String> maybe = manager.Scope.FindClosestNames(5, this.id);

					String note = null;

					if (maybe.Count == 1) {
						note = $"Perhaps you meant '{maybe[0]}'?";
					}
					else if (maybe.Count > 3) {
						note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe.Take(3))}";
					}
					else if (maybe.Count > 1) {
						note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe)}";
					}

					throw new LumenException(Exceptions.UNKNOWN_NAME.F(this.id), this.line, this.file) {
						Note = note
					};
				}

				return new ValueLiteral(manager.Scope.Get(this.id));
			}

			return this;
		}

		public override String ToString() {
			return this.id;
		}
	}
}
