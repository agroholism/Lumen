#nullable enable

using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    public static class Checker {
        public static void OfType(Value value, IObject type, Scope scope) {
            if (value.Type != type) {
                throw new LumenException(Exceptions.TYPE_ERROR.F(type, value.Type));
            }
        }
    }
}
