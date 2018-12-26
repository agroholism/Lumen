using StandartLibrary.Expressions;
using StandartLibrary;
using System;

namespace Stereotype
{
	[Serializable]
	internal class SpreadE : Expression {
		public Expression Closure(System.Collections.Generic.List<String> visible, Scope thread) {
			return this;
		}
		internal Expression expression;
        public int i;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public SpreadE(Expression expression)
        {
            this.expression = expression;
            if (expression is SpreadE)
                i += ((SpreadE)expression).i + 1;
        }

        public int Get()
        {
            return i;
        }

        public Value Eval(Scope e)
        {
            return expression.Eval(e);
        }
    }
}