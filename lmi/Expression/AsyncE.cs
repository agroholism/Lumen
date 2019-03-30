using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Threading.Tasks;
using String = System.String;

namespace Lumen.Light {
    internal class AsyncE : Expression {
        private Expression expression;

        public AsyncE(Expression expression) {
            this.expression = expression;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            Expression ex = this.expression.Closure(new List<String>(), e);

            Task.Factory.StartNew(() => {
                return ex.Eval(e);
            });

            return Const.UNIT;
        }

        public Expression Optimize(Scope scope) {
            return this;
        }
    }
}