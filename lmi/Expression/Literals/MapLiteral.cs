﻿using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
    internal class MapLiteral : Expression {
        private Dictionary<Expression, Expression> exps;

        public MapLiteral(Dictionary<Expression, Expression> exps) {
            this.exps = exps;
        }

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public IValue Eval(Scope e) {
            Dictionary<IValue, IValue> res = new Dictionary<IValue, IValue>();

            foreach(KeyValuePair<Expression, Expression> i in this.exps) {
                res.Add(i.Key.Eval(e), i.Value.Eval(e));
            }

            return new MutMap(res);
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
            yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
        }
	}
}