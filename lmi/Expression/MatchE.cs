using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace Lumen.Light {
    internal class MatchE : Expression {
        private Expression matchedExpression;
        private Dictionary<IPattern, Expression> patterns;

        public MatchE(Expression matchedExpression, Dictionary<IPattern, Expression> patterns) {
            this.matchedExpression = matchedExpression;
            this.patterns = patterns;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();
            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
                List<String> nls = new List<String>();
                nls.AddRange(visible);
                IPattern ip = i.Key.Closure(nls, scope) as IPattern;

                patterns.Add(ip, i.Value.Closure(nls, scope));
            }

            return new MatchE(this.matchedExpression.Closure(visible, scope), patterns);
        }

        public Value Eval(Scope e) {
            Value value = this.matchedExpression.Eval(e);

            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
                if(i.Key.Match(value, e)) {
                    return i.Value.Eval(e);
                }
            }

            return Const.UNIT;
        }
    }
}