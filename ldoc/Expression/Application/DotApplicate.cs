using System;
using System.Collections.Generic;
using System.Linq;

namespace ldoc {
    internal class DotApplicate : Expression {
        internal DotExpression callable;
        internal List<Expression> argumentsExperssions;

        public DotApplicate(Expression callable, List<Expression> exps) {
            this.callable = callable as DotExpression;
            this.argumentsExperssions = exps;
        }
    }
}