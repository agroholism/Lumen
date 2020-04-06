#nullable enable

using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {

	internal class Exit : Expression {
		private Expression expression;
		private Int32 line;
		private String fileName;

		public Exit(Expression expression, Int32 line, String fileName) {
			this.expression = expression;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new Exit(this.expression.Closure(manager), this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject obj = this.expression.Eval(scope);

			throw new ExitException(obj is Nil ? null : obj.ToString(), this.line, this.fileName);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}