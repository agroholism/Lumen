using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class AsPattern : IPattern {
        private IPattern listPattern;
        private String ide;

        public AsPattern(IPattern listPattern, String ide) {
            this.listPattern = listPattern;
            this.ide = ide;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            visible.Add(this.ide);
            return new AsPattern(this.listPattern.Closure(visible, scope) as IPattern, this.ide);
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            List<String> res = new List<String> { this.ide};
            res.AddRange(this.listPattern.GetDeclaredVariables());
            return res;
        }

        public Boolean Match(Value value, Scope scope) {
            if(this.listPattern.Match(value, scope)) {
                scope[this.ide] = value;
                return true;
            }

            return false;
        }

        public override String ToString() {
            return $"{this.listPattern.ToString()} as {this.ide}";
        }
    }
}