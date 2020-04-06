#nullable enable

using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class Assigment : Expression {
		private String identifier;
		private Expression assignableExpression;
		private Int32 line;
		private String fileName;

		public Assigment(String identifier, Expression assignableExpression, Int32 line, String fileName) {
			this.identifier = identifier;
			this.assignableExpression = assignableExpression;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new Assigment(this.identifier, this.assignableExpression.Closure(manager), 
				this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject euObject = this.assignableExpression.Eval(scope);

			try {


				scope.Assign(this.identifier, euObject);


			} catch(XenonException euex) when(euex.line == -1) {
				euex.line = this.line;
				euex.file = this.fileName;
				throw;
			}

			return euObject;
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}