using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;

namespace Stereotype
{
	[Serializable]
	internal class SpreadE : Expression {
		public Expression Closure(System.Collections.Generic.List<String> visible, Scope thread) {
			return this;
		}
		internal Expression expression;
        public Int32 i;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public SpreadE(Expression expression)
        {
            this.expression = expression;
            if (expression is SpreadE) {
				this.i += ((SpreadE)expression).i + 1;
			}
		}

        public Int32 Get()
        {
            return this.i;
        }

        public Value Eval(Scope e)
        {
            return this.expression.Eval(e);
        }
    }
}