using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class OrPattern : IPattern {
        private IPattern result;
        private IPattern second;

        public OrPattern(IPattern result, IPattern second) {
            this.result = result;
            this.second = second;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new OrPattern(
                this.result.Closure(visible, scope) as IPattern, 
                this.second.Closure(visible, scope) as IPattern);
        }

        public Value Eval(Scope e) {
            throw new System.NotImplementedException();
        }

        public List<System.String> GetDeclaredVariables() {
            List<System.String> res = new List<System.String>();
            res.AddRange(this.result.GetDeclaredVariables());
            res.AddRange(this.second.GetDeclaredVariables());
            return res;
        }

        public System.Boolean Match(Value value, Scope scope) {
            return this.result.Match(value, scope) || this.second.Match(value, scope);
        }

        public override System.String ToString() {
            return $"{this.result} | {this.second}";
        }
    }
}