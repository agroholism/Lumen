using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace ldoc {
    class IsExpression : Expression {
        internal Expression Expression;
        internal Expression type;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public IsExpression(Expression Expression, Expression type) {
            this.Expression = Expression;
            this.type = type;
        }

        public Expression Closure(ClosureManager manager) {
            return new IsExpression(this.Expression.Closure(manager), this.type.Closure(manager));
        }

        public Value Eval(Scope e) {
            Value v = this.Expression.Eval(e);

            if (this.type is IdExpression) {
                String nametype = ((IdExpression)this.type).id;
                if (nametype == "void") {
                  //  return new Bool(v is Lang.Void);
                }
            }

            Value p = this.type.Eval(e);


            if (v is IType && p is IType io) {
                return (Bool)io.IsParentOf(v);
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
