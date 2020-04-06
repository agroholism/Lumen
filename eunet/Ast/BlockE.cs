using System.Collections.Generic;
using System.Linq;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class BlockE : Expression {
		private List<Expression> exps;

		public BlockE(List<Expression> exps) {
			this.exps = exps;
		}

		public Expression Closure(ClosureManager manager) {
			return new BlockE(this.exps.Select(x => x.Closure(manager)).ToList());
		}

		public XnObject Eval(Scope scope) {
			for (System.Int32 i = 0; i < this.exps.Count - 1; i++) {
				this.exps[i].Eval(scope);
			}

			return this.exps[this.exps.Count - 1].Eval(scope);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			for (System.Int32 i = 0; i < this.exps.Count - 1; i++) {
				var x = this.exps[i].EvalWithYield(scope);
				foreach (var it in x) {
					yield return it;
				}
			}

			var z =  this.exps[this.exps.Count - 1].EvalWithYield(scope);
			foreach (var it in z) {
				yield return it;
			}
		}
	}
}