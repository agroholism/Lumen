using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class ListE : Expression {
        private List<Expression> elements;

        public ListE(List<Expression> elements) {
            this.elements = elements;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) { 
            return new List(LinkedList.Create(this.elements.Select(i => i.Eval(e))));
        }
    }
}