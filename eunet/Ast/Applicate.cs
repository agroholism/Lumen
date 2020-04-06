#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using Argent.Xenon.Runtime;
using KsType = Argent.Xenon.Runtime.KsType;

namespace Argent.Xenon.Ast {
	internal class Applicate : Expression {
		private Expression callableExpression;
		private List<Expression> argumentsExpression;
		private Int32 line;
		private String fileName;

		public Applicate(Expression callableExpression, List<Expression> argumentsExpression, Int32 line, String fileName) {
			this.callableExpression = callableExpression;
			this.argumentsExpression = argumentsExpression;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.callableExpression.Closure(manager), this.argumentsExpression.Select(i => i.Closure(manager)).ToList(), this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject callable = this.callableExpression.Eval(scope);

			try {
				if (callable is Function function) {
					return function.Run(new Scope(), this.argumentsExpression.Select(x => x.Eval(scope)).ToArray());
				}

				if (callable is KsType type) {
					return new Atom(type.Checker(this.argumentsExpression.Select(x => x.Eval(scope)).First()) ? 1 : 0);
				}
			} catch(XenonException xex) when (xex.line == -1) {
				xex.line = this.line;
				xex.file = this.fileName;
				throw;
			}
			
			throw new XenonException(Exceptions.VALUE_NOT_CALLABLE, line: this.line, fileName: this.fileName);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}