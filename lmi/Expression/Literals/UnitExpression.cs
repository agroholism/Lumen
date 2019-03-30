using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    public class UnitExpression : Expression {
        public static UnitExpression Instance { get; } = new UnitExpression();

        private UnitExpression() {

        }

        public Expression Closure(List<String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope scope) {
            return Const.UNIT;
        }

        public override String ToString() {
            return "()";
        }
    }
}