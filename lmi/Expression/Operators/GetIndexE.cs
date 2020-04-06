using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace Lumen.Lmi {
    internal class GetIndexE : Expression {
        internal Expression res;
        internal List<Expression> indices;
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }

		public GetIndexE(Expression res, List<Expression> indices, Int32 line, String file) {
            this.res = res;
            this.indices = indices;
			this.Line = line;
			this.File = file;
		}

        public Expression Closure(ClosureManager manager) {
            return new GetIndexE(this.res.Closure(manager), this.indices.Select(i => i.Closure(manager)).ToList(), this.Line, this.File);
        }

        public Value Eval(Scope e) {
			var value = this.res.Eval(e);
			List<Expression> exps = new List<Expression> { new ValueLiteral(value) };

			exps.AddRange(this.indices);

            return new Applicate(new DotOperator(new ValueLiteral(value.Type), Op.GETI, null, -1), exps, this.Line, this.File).Eval(e);
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}