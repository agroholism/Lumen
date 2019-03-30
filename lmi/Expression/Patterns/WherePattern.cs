using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Light {
    internal class WherePattern : IPattern {
        private IPattern result;
        private Expression exp;

        public WherePattern(IPattern result, Expression exp) {
            this.result = result;
            this.exp = exp;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            return this.result.GetDeclaredVariables();
        }

        public System.Boolean Match(Value value, Scope scope) {
            return this.result.Match(value, scope) && this.exp.Eval(scope).ToBoolean();
        }
    }
}