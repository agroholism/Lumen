using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Lmi {
	internal class IndexationAssign : Expression {
		internal Indexation expression;
		internal Expression assignable;
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }

		public IndexationAssign(Indexation expression, Expression assignable, Int32 line, String file) {
			this.expression = expression;
			this.assignable = assignable;
			this.Line = line;
			this.File = file;
		}

		public Expression Closure(ClosureManager manager) {
			return new IndexationAssign(this.expression.Closure(manager) as Indexation, this.assignable.Closure(manager), this.Line, this.File);
		}

		public IValue Eval(Scope e) {
			IValue value = this.expression.expression.Eval(e);
			List<Expression> exps = new List<Expression> {
				new ArrayLiteral(this.expression.indices),
				this.assignable,
				new ValueLiteral(value)
			};

			return new Applicate(
				new DotOperator(new ValueLiteral(value.Type), Constants.SETI, null, -1), exps, this.File, this.Line).Eval(e);
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}