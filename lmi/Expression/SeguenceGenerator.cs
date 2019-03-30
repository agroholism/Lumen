using System.Collections.Generic;
using System;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace Lumen.Light {
    internal class SequenceGenerator : Expression {
        internal String names;
        internal Expression container;
        internal Expression body;

        public SequenceGenerator(String names, Expression container, Expression body) {
            this.names = names;
            this.container = container;
            this.body = body;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return this;
        }

        public Value Eval(Scope e) {
            return new Enumerator(this.Generator(new Scope(e)));
        }

        internal IEnumerable<Value> Generator(Scope scope) {
            Value current;

            foreach (Value i in this.container.Eval(scope).ToSequence(scope)) {
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

                if (current is Lang.Void) {
                    continue;
                }

                yield return current;
            }
        }
    }
}