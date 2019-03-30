using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Light {
    internal class ListGenerator : Expression {
        private SequenceGenerator sequenceGenerator;

        public ListGenerator(SequenceGenerator sequenceGenerator) {
            this.sequenceGenerator = sequenceGenerator;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return new ListGenerator(this.sequenceGenerator.Closure(visible, scope) as SequenceGenerator);
        }

        public Value Eval(Scope e) {
            return new List(LinkedList.Create(this.sequenceGenerator.Generator(e)));
        }
    }
}