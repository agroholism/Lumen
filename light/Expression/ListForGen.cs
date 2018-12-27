using System.Collections.Generic;
using System;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class ListForGen : Expression {
		internal String names;
		internal Expression container;
		internal Expression body;

		public ListForGen(String names, Expression container, Expression body) {
			this.names = names;
			this.container = container;
			this.body = body;
		}
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			return new Enumerator(Generator(new Scope(e)));
		}

		private IEnumerable<Value> Generator(Scope scope) {
			Value current;

			foreach (Value i in container.Eval(scope).ToIterator(scope)) {
				scope.Set(this.names, i);

				try {
					current = body.Eval(scope);
				}
				catch (Break bs) {
					if (bs.UseLabel() > 0) {
						throw bs;
					}

					break;
				}
				catch (NextE) {
					continue;
				}

				if(current is Null) {
					continue;
				}

				yield return current;
			}
		}
	}
}