using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace ldoc {
    internal class MatchE : Expression {
        private Expression matchedExpression;
        private Dictionary<IPattern, Expression> patterns;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public MatchE(Expression matchedExpression, Dictionary<IPattern, Expression> patterns) {
            this.matchedExpression = matchedExpression;
            this.patterns = patterns;
        }

        public Expression Closure(ClosureManager manager) {
            Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();
            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
				ClosureManager manager2 = manager.Clone();

                IPattern ip = i.Key.Closure(manager2) as IPattern;

                patterns.Add(ip, i.Value.Closure(manager2));
            }

            return new MatchE(this.matchedExpression.Closure(manager), patterns);
        }

        public Value Eval(Scope e) {
            Value value = this.matchedExpression.Eval(e);

            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
                if(!i.Key.Match(value, e).Success) {
                    return i.Value.Eval(e);
                }
            }

            return Const.UNIT;
        }
    }
}