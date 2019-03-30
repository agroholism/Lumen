using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class OpenModule : Expression {
        private readonly Expression expression;

        public OpenModule(Expression expression) {
            this.expression = expression;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new OpenModule(this.expression.Closure(visible, scope));
        }

        public Value Eval(Scope e) {
            Value value = this.expression.Eval(e);

            if (value is Module module) {
                foreach (KeyValuePair<System.String, Value> i in module.variables) {
                    e[i.Key] = i.Value;
                }
                return Const.UNIT;
            }

            throw new LumenException("it's not a module");
        }
    }
}