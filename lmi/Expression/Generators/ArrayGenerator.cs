using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ArrayGenerator : Expression {
		private SequenceGenerator sequenceGenerator;

		public ArrayGenerator(SequenceGenerator sequenceGenerator) {
			this.sequenceGenerator = sequenceGenerator;
		}

		public Expression Closure(ClosureManager manager) {
			return new ArrayGenerator(this.sequenceGenerator.Closure(manager) as SequenceGenerator);
		}

		public Value Eval(Scope e) {
			return new MutableArray(this.sequenceGenerator.Generator(e));
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}