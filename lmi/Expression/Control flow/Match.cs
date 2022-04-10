﻿using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class Match : Expression {
        private Expression matchedExpression;
        private Dictionary<IPattern, Expression> patterns;

        public Match(Expression matchedExpression, Dictionary<IPattern, Expression> patterns) {
            this.matchedExpression = matchedExpression;
            this.patterns = patterns;
        }

        public Expression Closure(ClosureManager manager) {
            Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
				ClosureManager manager2 = manager.Clone();

                IPattern ip = i.Key.Closure(manager2) as IPattern;
               
                patterns.Add(ip, i.Value.Closure(manager2));

				if(manager2.HasYield) {
					manager.HasYield = true;
				}
            }

            return new Match(this.matchedExpression.Closure(manager), patterns);
        }

        public IValue Eval(Scope e) {
            IValue value = this.matchedExpression.Eval(e);
 
            foreach(KeyValuePair<IPattern, Expression> i in this.patterns) {
                if(i.Key.Match(value, e).IsSuccess) {
                    return i.Value.Eval(e);
                }
            }

            return Const.UNIT;
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IValue value = this.matchedExpression.Eval(scope);

			foreach (KeyValuePair<IPattern, Expression> i in this.patterns) {
				if (i.Key.Match(value, scope).IsSuccess) {
					IEnumerable<IValue> result = i.Value.EvalWithYield(scope);

					if (result != null) {
						foreach(IValue x in result) {
							yield return x;
						}
					}

					yield break;
				}
			}
		}
	}
}