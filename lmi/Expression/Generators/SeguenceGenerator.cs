using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	internal class SequenceGenerator : Expression {
        internal IPattern names;
        internal Expression container;
        internal Expression body;

        public SequenceGenerator(IPattern names, Expression container, Expression body) {
            this.names = names;
            this.container = container;
            this.body = body;
        }

        public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			manager2.Declare(this.names.GetDeclaredVariables());
            return new SequenceGenerator(this.names.Closure(manager2) as IPattern, this.container.Closure(manager2), this.body.Closure(manager2));
        }

        public Value Eval(Scope e) {
            return new Stream(this.Generator(new Scope(e)));
        }

        internal IEnumerable<Value> Generator(Scope scope) {
            Value current;

            foreach (Value i in this.container.Eval(scope).ToStream(scope)) {
				this.names.Match(i, scope);

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