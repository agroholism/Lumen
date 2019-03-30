using System;
using System.Collections.Generic;

namespace ldoc {
    internal class MatchE : Expression {
        private Expression matchedExpression;
        private Dictionary<IPattern, Expression> patterns;

        public MatchE(Expression matchedExpression, Dictionary<IPattern, Expression> patterns) {
            this.matchedExpression = matchedExpression;
            this.patterns = patterns;
        }
    }
}