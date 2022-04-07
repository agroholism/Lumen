using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	public class LambdaLiteral : Expression {
		private List<IPattern> parameters;
		private Expression body;

		public LambdaLiteral(List<IPattern> parameters, Expression body) {
			this.parameters = parameters;
			this.body = body;
		}

		public Value Eval(Scope scope) {
			ClosureManager manager = new ClosureManager(scope);

			List<IPattern> closuredParameters =
				this.parameters.Select(parameter => parameter.Closure(manager) as IPattern).ToList();

			Expression closuredBody = this.body.Closure(manager);

			if (manager.HasYield) {
				return new LambdaFun((generatorScope, _) =>
					new Flow(new CustomFlow(closuredBody, generatorScope)), closuredParameters
				);
			}

			return new UserFun(closuredParameters, closuredBody, "[AnonymousFunction]");
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager innerManager = manager.Clone();
			return new LambdaLiteral(
				this.parameters.Select(i => i.Closure(innerManager) as IPattern).ToList(), 
				this.body.Closure(innerManager));
		}

		public override String ToString() {
			if (this.parameters.Count == 1) {
				return "fun -> " + Utils.Bodify(this.body);
			}

			return $"fun {String.Join(" ", this.parameters)} -> " + Utils.Bodify(this.body);
		}
	}
}