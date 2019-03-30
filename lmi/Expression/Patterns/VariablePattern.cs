using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class VariablePattern : IPattern {
        private System.String id;

        public VariablePattern(System.String id) {
            this.id = id;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new ValuePattern(scope[this.id]);
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            return new List<System.String>();
        }

        public System.Boolean Match(Value value, Scope scope) {
            return new ValuePattern(scope[this.id]).Match(value, scope);
        }

        public override System.String ToString() {
            return this.id;
        }
    }
}