using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class PairLiteral : Expression {
		private Expression result;
		private Expression sndExpression;
		private System.Int32 line;
		private System.String fileName;

		public PairLiteral(Expression result, Expression sndExpression, System.Int32 line, System.String fileName) {
			this.result = result;
			this.sndExpression = sndExpression;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new PairLiteral(this.result.Closure(manager), this.sndExpression.Closure(manager), this.line, this.fileName);
		}

		public Value Eval(Scope e) {
			return Helper.CreatePair(this.result.Eval(e), this.sndExpression.Eval(e));
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}