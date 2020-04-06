#nullable enable

using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;
using KsType = Argent.Xenon.Runtime.KsType;

namespace Argent.Xenon.Ast {
	internal class VariableDeclaration : Expression {
		private Expression variableType;
		private String variableName;
		private Expression assignableExpression;
		private Int32 line;
		private String fileName;

		public VariableDeclaration(Expression variableType, String variableName, Expression assignableExpression, Int32 line, String fileName) {
			this.variableType = variableType;
			this.variableName = variableName;
			this.assignableExpression = assignableExpression;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.variableName);
			return new VariableDeclaration(this.variableType.Closure(manager), this.variableName, this.assignableExpression?.Closure(manager), this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject euObject = this.variableType.Eval(scope);
			XnObject assignableValue = this.assignableExpression.Eval(scope);

			if (euObject is KsTypeable type) {
				try {


					scope.DeclareVariable(this.variableName, type);
					scope.Assign(this.variableName, assignableValue);


				}
				catch (XenonException euex) {
					euex.line = this.line;
					euex.file = this.fileName;
					throw;
				}

				return new Atom(0);
			}

			throw new XenonException(Exceptions.WAIT_A_TYPE, line: this.line, fileName: this.fileName);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}