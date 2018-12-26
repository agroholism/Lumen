using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using System.Collections.Generic;

namespace Stereotype
{
	[Serializable]
	internal class OptionalDotExpression : Expression
    {
        private Expression res;
        private string v;

        public OptionalDotExpression(Expression res, string v)
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
            Value a = res.Eval(e);
            if (a is Module)
                if (((Module)a).Contains(v))
                    return ((Module)a).Get(v);
                else return Const.NULL;
            if (a is Expando)
                if (((Expando)a).IsExists(v))
                    return ((Expando)a).Get(v, AccessModifiers.PUBLIC, e);
                else return Const.NULL;
            if (a is IObject)
                if (((IObject)a).Type.Contains(v))
                    return ((IObject)a).Get(v, AccessModifiers.PRIVATE, e);
                else return Const.NULL;
            if (a.Type.Contains(v))
                return a.Type.Get(v, e);
            else return Const.NULL;
        }

        public override string ToString()
        {
            return res.ToString() + "." + v;
        }
    }
}