using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using Lumen;
using System.Collections.Generic;

namespace Stereotype
{
	[Serializable]
	internal class IsMatching : Expression
    {
        private readonly Expression expr;
        private readonly Expression e;
        private readonly String n;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public IsMatching(Expression expr, Expression e, String n)
        {
            this.expr = expr;
            this.e = e;
            this.n = n;
        }

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e)
        {
            Value v = this.expr.Eval(e);
            if (Converter.ToBoolean(new IsExpression(new ValueE(v), this.e).Eval(e)))
            {
                e.Set(this.n, v);
                return new Bool(true);
            }
            else
            {
                return new Bool(false);
            }
        }
    }
}