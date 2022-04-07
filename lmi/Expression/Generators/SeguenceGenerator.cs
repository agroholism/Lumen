using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class SequenceGenerator : Expression {
		String cycleName;
		internal IPattern names;
		internal Expression container;
		internal Expression body;

		public SequenceGenerator(String cycleName, IPattern names, Expression container, Expression body) {
			this.cycleName = cycleName;
			this.names = names;
			this.container = container;
			this.body = body;
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			manager2.Declare(this.names.GetDeclaredVariables());
			return new SequenceGenerator(this.cycleName, this.names.Closure(manager2) as IPattern, this.container.Closure(manager2), this.body.Closure(manager2));
		}

		public Value Eval(Scope e) {
			return new Flow(this.Generator(new Scope(e)));
		}

		internal IEnumerable<Value> Generator(Scope scope) {
			Value current;

			foreach (Value i in this.container.Eval(scope).ToFlow(scope)) {
				var inner = new Scope(scope);
				this.names.Match(i, inner);

				try {
					current = this.body.Eval(inner);
				}
				catch (Break breakException) {
					if (breakException.IsMatch(this.cycleName)) {
						break;
					}

					throw breakException;
				}
				catch (Next) {
					continue;
				}

				if (current == Const.UNIT) {
					continue;
				}

				yield return current;
			}
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}