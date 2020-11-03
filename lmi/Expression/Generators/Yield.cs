using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Yield : Expression {
		private Expression expression;

		public Yield(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(ClosureManager manager) {
			manager.HasYield = true;
			return new Yield(this.expression.Closure(manager));
		}

		public Value Eval(Scope e) {
			return this.expression.Eval(e);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return this.expression.Eval(scope);
			if (scope.IsExsists("<curr-gen-val>")) {
				yield return new GeneratorTerminalResult( scope["<curr-gen-val>"]);
				scope.Remove("<curr-gen-val>");
			} else {
				yield return new GeneratorTerminalResult(Const.UNIT);
			}
		}
	}

	class GeneratorTerminalResult : BaseValueImpl {
		public GeneratorTerminalResult(Value value) {
			this.Value = value;
		}

		public Value Value { get; set; }
		public override IType Type => throw new System.NotImplementedException();
	}
}