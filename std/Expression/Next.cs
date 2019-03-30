using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public class Next : Exception, Expression {
        public Value Eval(Scope e) {
            throw this;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return this;
        }
    }
}