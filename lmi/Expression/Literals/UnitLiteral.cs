﻿using System;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections.Generic;

namespace Lumen.Lmi {
	public class UnitLiteral : Expression {
        public static UnitLiteral Instance { get; } = new UnitLiteral();

        private UnitLiteral() {

        }

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public IValue Eval(Scope scope) {
            return Const.UNIT;
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
            yield return new GeneratorExpressionTerminalResult(Const.UNIT);
        }

		public override String ToString() {
            return "()";
        }
    }
}