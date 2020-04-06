using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class IdExpression : Expression {
		private String text;
		private Int32 line;
		private String fileName;

		public IdExpression(String text, Int32 line, String fileName) {
			this.text = text;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return manager.IsDeclared(this.text) ? this : (Expression)new ValueE(manager.Scope.Get(this.text));
		}

		public XnObject Eval(Scope scope) {
			try {


				return scope.Get(this.text);


			} catch (XenonException euex) {
				euex.line = this.line;
				euex.file = this.fileName;
				throw;
			}
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}