using System;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class StringE : Expression {
        internal String text;

        public StringE(String text) {
            this.text = text;
        }
        public Expression Closure(System.Collections.Generic.List<String> visible, Scope thread) {
            return this;
        }

        public Value Eval(Scope e) {
            return new Text(this.text);
        }

        public override String ToString() {
            return $"\"{this.text}\"";
        }
    }
}