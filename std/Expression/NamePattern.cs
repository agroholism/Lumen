using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
    public class NamePattern : IPattern {
        private String id;

        public NamePattern(String id) {
            this.id = id;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            if (!visible.Contains(this.id)) {
                visible.Add(this.id);
            }

            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String> { this.id };
        }

        public Boolean Match(Value value, Scope scope) {
            scope[this.id] = value;
            return true;
        }

        public override String ToString() {
            return this.id.ToString();
        }
    }
}
