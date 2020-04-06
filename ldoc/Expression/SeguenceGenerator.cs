using System.Collections.Generic;
using System;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace ldoc {
    internal class SequenceGenerator : Expression {
        internal String names;
        internal Expression container;
        internal Expression body;

        public SequenceGenerator(String names, Expression container, Expression body) {
            this.names = names;
            this.container = container;
            this.body = body;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			manager2.Declare(names);
            return new SequenceGenerator(names, container.Closure(manager2), body.Closure(manager2));
        }

        public Value Eval(Scope e) {
            return new Stream(this.Generator(new Scope(e)));
        }

        internal IEnumerable<Value> Generator(Scope scope) {
            Value current;

            foreach (Value i in this.container.Eval(scope).ToStream(scope)) {
                scope.Bind(this.names, i);

                try {
                    current = this.body.Eval(scope);
                } catch (Break bs) {
                    if (bs.UseLabel() > 0) {
                        throw bs;
                    }

                    break;
                } catch (Next) {
                    continue;
                }

                if (current == Prelude.None) {
                    continue;
                }

                yield return current;
            }
        }
    }
}