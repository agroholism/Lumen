using StandartLibrary.Expressions;
using System;
using StandartLibrary;
using System.Collections.Generic;

namespace Stereotype
{
	[Serializable]
	internal class IsMatching : Expression
    {
        private Expression expr;
        private Expression e;
        private string n;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public IsMatching(Expression expr, Expression e, string n)
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
            Value v = expr.Eval(e);
            if (Converter.ToBoolean(new IsExpression(new ValueE(v), this.e).Eval(e)))
            {
                e.Set(n, v);
                return new Bool(true);
            }
            else
            {
                return new Bool(false);
            }
        }
    }
}