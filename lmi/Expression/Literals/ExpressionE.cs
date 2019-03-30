using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace Lumen.Light {
    internal class ExpressionE : Expression {
        internal Expression expression;

        public ExpressionE(Expression expression) {
            this.expression = expression;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            throw new NotImplementedException();
        }
        public Expression Optimize(Scope scope) {
            return this;
        }
        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }
    }
}