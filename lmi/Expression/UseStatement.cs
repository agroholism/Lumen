using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class UseStatement : Expression {
		private System.String name;
		private Expression assignable;
		private Expression body;

		public UseStatement(System.String name, Expression assignable, Expression body) {
			this.name = name;
			this.assignable = assignable;
			this.body = body;
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			manager2.Declare(name);
			return new UseStatement(name, this.assignable.Closure(manager), this.body.Closure(manager2));
		}

		public Value Eval(Scope e) {
			Value context = this.assignable.Eval(e);
			Fun onEnter = context.Type.GetMember("onEnter", e).ToFunction(e);
			Fun onExit = context.Type.GetMember("onExit", e).ToFunction(e);
			Value value = Const.UNIT;
			Value result = Const.UNIT;

			Exception ex = null;

			Value endres;
			try {
				value = onEnter.Run(new Scope(e), context);
				e[this.name] = value;

				result = this.body.Eval(e);
			}
			catch (Exception exep) {
				ex = exep;
			}
			finally {
				if (ex == null) {
					endres = onExit.Run(new Scope(e), context, value, result);
				}
				else {
					endres = onExit.Run(new Scope(e), context, value, result, new Text(ex.Message));
				}
			}

			return endres;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value context = this.assignable.Eval(scope);
			Fun onEnter = context.Type.GetMember("onEnter", scope).ToFunction(scope);
			Fun onExit = context.Type.GetMember("onExit", scope).ToFunction(scope);

			Value value = onEnter.Run(new Scope(scope), context);
			scope[this.name] = value;

			IEnumerable<Value> result = this.body.EvalWithYield(scope);
			foreach (Value i in result) {
				yield return i;
			}

			onExit.Run(new Scope(scope), context, value);
		}
	}
}