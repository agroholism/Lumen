using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public interface IPattern : Expression {
        Boolean Match(Value value, Scope scope);

        List<String> GetDeclaredVariables();
    }
}
