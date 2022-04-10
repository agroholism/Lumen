﻿using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace Lumen.Lmi {
	internal class Indexation : Expression {
        internal Expression expression;
        internal List<Expression> indices;
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }

		public Indexation(Expression expression, List<Expression> indices, Int32 line, String file) {
            this.expression = expression;
            this.indices = indices;
			this.Line = line;
			this.File = file;
		}

        public Expression Closure(ClosureManager manager) {
            return new Indexation(this.expression.Closure(manager), this.indices.Select(i => i.Closure(manager)).ToList(), this.Line, this.File);
        }

        public IValue Eval(Scope e) {
			IValue value = this.expression.Eval(e);
			List<Expression> exps = new List<Expression> { 
				new ArrayLiteral(this.indices), 
				new ValueLiteral(value) 
			};

            return new Applicate(new DotOperator(new ValueLiteral(value.Type), Constants.GETI, null, -1), exps, this.File, this.Line).Eval(e);
        }

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}