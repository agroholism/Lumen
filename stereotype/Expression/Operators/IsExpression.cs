using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	class IsExpression : Expression {
		internal Expression Expression;
		internal Expression type;

		public IsExpression(Expression Expression, Expression type) {
			this.Expression = Expression;
			this.type = type;
		}
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Expression Closure(List<String> visible, Scope thread) {
			return new IsExpression(Expression.Closure(visible, thread), type.Closure(visible, thread));
		}

		public Value Eval(Scope e) {
			Value v = Expression.Eval(e);
			if (type is IdExpression) {
				string nametype = ((IdExpression)type).id;
				if (nametype == "null")
					return new Bool(v is Null);
			}
			Value p = type.Eval(e);

			if (p is Null)
				return new Bool(v is Null);

			if (v is Expando && p is Expando) {
				return new Bool(((Expando)v).IsChildOf((Expando)p));
			}

			if (v.Equals(p))
				return new Bool(true);

			/*if (p is KInterface i) {
				return new Bool(i.HTypeImplemented(v.Type));
			}*/


			return new Bool(false);
		}

		public override string ToString() {
			return Expression.ToString() + " is " + type;
		}
	}
}
