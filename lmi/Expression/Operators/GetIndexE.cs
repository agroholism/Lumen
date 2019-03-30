using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Light {
    internal class GetIndexE : Expression {
        internal Expression res;
        internal List<Expression> indices;

        public GetIndexE(Expression res, List<Expression> indices) {
            this.res = res;
            this.indices = indices;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            Value v = this.res.Eval(e);
            return new DotApplicate(new DotExpression(new ValueE(v), Op.GETI, null, -1), this.indices).Eval(e);
        }

        public Expression Optimize(Scope scope) {
            throw new System.NotImplementedException();
        }
    }
}