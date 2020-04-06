using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace ldoc {
    internal class GetIndexE : Expression {
        internal Expression res;
        internal List<Expression> indices;
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public GetIndexE(Expression res, List<Expression> indices, Int32 line, String file) {
            this.res = res;
            this.indices = indices;
			this.Line = line;
			this.File = file;
		}

        public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) {
            Value v = this.res.Eval(e);
            return new DotApplicate(new DotExpression(new ValueE(v), Op.GETI, null, -1), new List<Expression> { new ValueE(new Lumen.Lang.Array(this.indices.Select(i => i.Eval(e)).ToList())) }, this.Line, this.File).Eval(e);
        }
    }
}