﻿using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class TypeOf : Expression {
		private Expression res;
		private String file;
		private Int32 line;

		public TypeOf(Expression res, String file, Int32 line) {
			this.res = res;
			this.file = file;
			this.line = line;
		}

		public IValue Eval(Scope scope) {
			return this.res.Eval(scope).Type;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> evaluationResults = this.res.EvalWithYield(scope);

			foreach(IValue evaluationResult in evaluationResults) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					yield return new GeneratorExpressionTerminalResult(terminalResult.Value.Type);
					yield break;
				}

				yield return evaluationResult;
			}
		}

		public Expression Closure(ClosureManager manager) {
			return new TypeOf(this.res.Closure(manager), this.file, this.line);
		}
	}
}