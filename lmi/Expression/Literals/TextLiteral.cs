﻿using System;
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

        public IValue Eval(Scope e) {
            return new Text(this.text);
        }

        public override String ToString() {
            return $"\"{this.text}\"";
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}
	}
}