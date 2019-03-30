using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class DiscordPattern : IPattern {
        public Expression Closure(List<String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            throw new NotImplementedException();
        }

        public List<String> GetDeclaredVariables() {
            return new List<String>();
        }

        public Boolean Match(Value value, Scope scope) {
            return true;
        }

        public override String ToString() {
            return "_";
        }
    }
}