using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class ArrayE : Expression {
        private List<Expression> elements;

        public ArrayE(List<Expression> elements) {
            this.elements = elements;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new ArrayE(this.elements.Select(i => i.Closure(visible, scope)).ToList());
        }

        public Value Eval(Scope e) {
            return new Array(this.elements.Select(i => i.Eval(e)).ToList());
        }
    }
}