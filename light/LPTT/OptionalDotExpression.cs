using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using System.Collections.Generic;

namespace Stereotype
{
	[Serializable]
	internal class OptionalDotExpression : Expression
    {
        private readonly Expression res;
        private readonly String v;

        public OptionalDotExpression(Expression res, String v)
        {
            this.res = res;
            this.v = v;
        }
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e)
        {
            Value a = this.res.Eval(e);
            if (a is Module) {
				if (((Module)a).Contains(this.v)) {
					return ((Module)a).Get(this.v);
				}
				else {
					return Const.NULL;
				}
			}

			if (a is Expando) {
				if (((Expando)a).IsExists(this.v)) {
					return ((Expando)a).Get(this.v, AccessModifiers.PUBLIC, e);
				}
				else {
					return Const.NULL;
				}
			}

			if (a is IObject) {
				if (((IObject)a).Type.Contains(this.v)) {
					return ((IObject)a).Get(this.v, AccessModifiers.PRIVATE, e);
				}
				else {
					return Const.NULL;
				}
			}

			if (a.Type.Contains(this.v)) {
				return a.Type.Get(this.v, e);
			}
			else {
				return Const.NULL;
			}
		}

        public override String ToString()
        {
            return this.res.ToString() + "." + this.v;
        }
    }
}