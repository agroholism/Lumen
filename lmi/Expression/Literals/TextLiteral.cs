using System;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;

namespace Lumen.Lmi {
    internal class TextLiteral : Expression {
        internal String text;

        public TextLiteral(String text) {
            this.text = text;
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

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}