using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    public class LambdaLiteral : Expression {
        public List<IPattern> arguments;
        public Expression body;
 
        public LambdaLiteral(List<IPattern> arguments, Expression body) {
            this.arguments = arguments;
            this.body = body;
        }

        public Expression Closure(ClosureManager manager) {
			ClosureManager newManager = manager.Clone();
			return new LambdaLiteral(this.arguments.Select(i => i.Closure(newManager) as IPattern).ToList(), this.body.Closure(newManager));
        }

        public Value Eval(Scope e) {
			ClosureManager manager = new ClosureManager(e);

			manager.Declare(new List<String>() { "self", "_" });

			foreach (IPattern i in this.arguments) {
				manager.Declare(i.GetDeclaredVariables());
			}

			var x = this.body.Closure(manager);

			if (manager.HasYield) {
				return new LambdaFun((s, a) => {
					return new Stream(new LumenGenerator { generatorBody = x, AssociatedScope = s });
				}) {
					Arguments = arguments
				};
			}

			// todo
			return new UserFun(this.arguments, x);
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		/*public override String ToString() {
			if(this.arguments.Count == 1) {
				return "() -> " + Utils.Bodify(this.body);
			}

            return $"({String.Join(" ", this.arguments)}) -> " + Utils.Bodify(this.body);
        }*/
	}
}