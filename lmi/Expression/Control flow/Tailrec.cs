using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Tailrec : Expression {
		private List<Expression> argumentsExpressions;
		private String fileName;
		private Int32 lineNumber;

		public Tailrec(List<Expression> argumentsExpression, String fileName, Int32 lineNumber) {
			this.argumentsExpressions = argumentsExpression;
			this.fileName = fileName;
			this.lineNumber = lineNumber;
		}

		public IValue Eval(Scope scope) {
			return new TailRecursion(this.argumentsExpressions.Select(i => i.Eval(scope)).ToArray());
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			yield return new TailRecursion(this.argumentsExpressions.Select(i => i.Eval(scope)).ToArray());
			//throw new LumenException("evaluating tailrec in generator");
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasTailRecursion = true;

			return new Tailrec(
				this.argumentsExpressions.Select(i => i.Closure(manager)).ToList(),
				this.fileName, this.lineNumber);
		}

		public override String ToString() {
			return $"tailrec {Utils.ArgumentsToString(this.argumentsExpressions)}";
		}
	}
}