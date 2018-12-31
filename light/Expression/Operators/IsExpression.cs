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
			return new IsExpression(this.Expression.Closure(visible, thread), this.type.Closure(visible, thread));
		}

		public Value Eval(Scope e) {
			Value v = this.Expression.Eval(e);
			if (this.type is IdExpression) {
				String nametype = ((IdExpression)this.type).id;
				if (nametype == "null") {
					return new Bool(v is Null);
				}
			}
			Value p = this.type.Eval(e);

			if (p is Null) {
				return new Bool(v is Null);
			}

			if (v is Expando && p is Expando) {
				return new Bool(((Expando)v).IsChildOf((Expando)p));
			}

			if (v.Equals(p)) {
				return new Bool(true);
			}

			/*if (p is KInterface i) {
				return new Bool(i.HTypeImplemented(v.Type));
			}*/


			return new Bool(false);
		}

		public override String ToString() {
			return this.Expression.ToString() + " is " + this.type;
		}
	}
}
