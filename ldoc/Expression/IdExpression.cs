using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
	public class IdExpression : Expression {
		public String id;
		public Int32 line;
		public String file;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public IdExpression(String id, Int32 line, String file) {
			this.id = id;
			this.line = line;
			this.file = file;
		}
		public Value Eval(Scope e) {
			if (!e.IsExsists(this.id)) {
				List<String> maybe = e.variables.Keys
									.Where(i => Helper.Tanimoto(i, this.id) > 0.3)
									.OrderBy(i => Helper.Tanimoto(i, this.id))
									.ToList();

				throw new LumenException(Exceptions.UNKNOWN_IDENTIFITER.F(this.id), null, line, file) {
					Note = maybe.Count > 0 ? $"Maybe you mean {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe)}" : null
				};
			}

			return e.Get(this.id);
		}

		public Expression Closure(ClosureManager manager) {
			if (!(manager.IsDeclared(this.id))) {
				if (!manager.Scope.IsExsists(this.id)) {
					List<String> maybe = manager.Declarations
						.Concat(manager.Scope.variables.Keys)
						.Where(i => Helper.Tanimoto(i, this.id) > 0.3)
						.OrderBy(i => Helper.Tanimoto(i, this.id))
						.ToList();

					throw new LumenException(Exceptions.UNKNOWN_IDENTIFITER.F(this.id), null, line, file) {
						Note = maybe.Count > 0 ? $"Maybe you mean {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", manager.Declarations.Concat(manager.Scope.variables.Keys).Where(i => Helper.Tanimoto(i, this.id) > 0.3).OrderBy(i => Helper.Tanimoto(i, this.id)))}" : null
					};
				}
				return new ValueE(manager.Scope.Get(this.id));
			}

			return this;
		}

		public override String ToString() {
			return this.id;
		}
	}
}
