using System.Collections;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class LumenGenerator : IEnumerable<Value> {
		private Expression generatorBody;
		private Scope associatedScope;

		public LumenGenerator(Expression generatorBody, Scope associatedScope) {
			this.generatorBody = generatorBody;
			this.associatedScope = associatedScope;
		}

		private IEnumerable<Value> Run(Scope scope) {
			foreach (Value i in this.generatorBody.EvalWithYield(scope)) {
				if (i is StopGenerator) {
					yield break;
				}

				yield return i;
			}
		}

		public IEnumerator<Value> GetEnumerator() {
			Scope scope = new Scope(this.associatedScope);
			return new LumenIterator(this.Run(scope).GetEnumerator(), scope);
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}
