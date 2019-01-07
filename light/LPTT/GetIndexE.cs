using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class GetIndexE : Expression {
		private Expression res;
		private List<Expression> indices;

		public GetIndexE(Expression res, List<Expression> indices) {
			this.res = res;
			this.indices = indices;
		}

		public Expression Closure(List<System.String> visible, Scope scope) {
			return this;
		}

		public Value Eval(Scope e) {
			Value v = this.res.Eval(e);
			if (v is IObject rec) {
				return new DotApplicate(new DotExpression(new ValueE(v), "@generic"), indices).Eval(e);
			}
			else {
				return new DotApplicate(new DotExpression(new ValueE(v), Op.GETI), indices).Eval(e);
			}
		}

		public Expression Optimize(Scope scope) {
			throw new System.NotImplementedException();
		}
	}
}