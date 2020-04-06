using System;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;

namespace ldoc {
    internal class StringE : Expression {
        internal String text;

        public StringE(String text) {
            this.text = text;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
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