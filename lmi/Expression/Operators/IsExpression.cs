using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;

namespace Lumen.Light {
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
                if (nametype == "void") {
                    return new Bool(v is Lang.Void);
                }
            }

            Value p = this.type.Eval(e);

            if (p is Lang.Void) {
                return new Bool(v is Lang.Void);
            }

            if (v is IObject && p is IObject io) {
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
