using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class ColonExpression : Expression {
		private Expression result;
		private String fieldName;
		private Int32 line;
		private String fileName;

		public ColonExpression(Expression result, String fieldName, Int32 line, String fileName) {
			this.result = result;
			this.fieldName = fieldName;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new ColonExpression(this.result.Closure(manager), this.fieldName, this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject src = this.result.Eval(scope);

			if(src is KsType type) {
				return type.EigenNamespace[this.fieldName];
			}


			throw new XenonException(Exceptions.INVALID_OPERATION, line: this.line, fileName: this.fileName);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}