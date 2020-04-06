using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
    public class LambdaFunction : Expression {
        public List<IPattern> arguments;
        public Expression body;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public LambdaFunction(List<IPattern> arguments, Expression body) {
            this.arguments = arguments;
            this.body = body;
        }

        public Expression Closure(ClosureManager manager) {
            return new LambdaFunction(this.arguments.Select(i => i.Closure(manager) as IPattern).ToList(), this.body.Closure(manager));
        }

        public Value Eval(Scope e) {
			ClosureManager manager = new ClosureManager(e);

			manager.Declare(new List<String>() { "self", "_" });

			foreach (IPattern i in this.arguments) {
				manager.Declare(i.GetDeclaredVariables());
			}

			return new UserFun(this.arguments, this.body.Closure(manager));
        }

        public override String ToString() {
            String result = "(";

            return (result.Length > 2 ? result.Substring(0, result.Length - 2) : result) + ") => " + this.body.ToString();
        }
    }
}