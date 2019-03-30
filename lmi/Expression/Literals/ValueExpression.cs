using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace Lumen.Light {
    [Serializable]
    public class ValueE : Expression {
        public Value val;

        public ValueE(Value Object) {
            this.val = Object;
        }

        public Expression Closure(List<String> visible, Scope thread) {
            return this;
        }

        public ValueE(Double Object) {
            this.val = new Number(Object);
        }

        public Value Eval(Scope e) {
            return this.val;
        }

        public override String ToString() {
            return this.val.ToString();
        }
    }
}
