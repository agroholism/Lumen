using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public interface Expression {
        Value Eval(Scope e);

        Expression Closure(List<String> visible, Scope scope);
    }
}
