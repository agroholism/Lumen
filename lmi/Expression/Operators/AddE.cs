using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class AddE : Expression {
        private Expression result;
        private Expression right;

        public AddE(Expression result, Expression right) {
            this.result = result;
            this.right = right;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new AddE(this.result.Closure(visible, scope), this.right.Closure(visible, scope));
        }

        public Value Eval(Scope e) {
            Value right = this.right.Eval(e);
            if(right is List list) {
                return new List(new LinkedList(this.result.Eval(e), list.value));
            }
            throw new System.Exception("wait a list ::");
        }
    }
}