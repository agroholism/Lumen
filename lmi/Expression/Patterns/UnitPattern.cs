using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class UnitPattern : IPattern {
        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            return new List<System.String>();
        }

        public System.Boolean Match(Value value, Scope scope) {
            return value is Void;
        }

        public override System.String ToString() {
            return "()";
        }
    }
}