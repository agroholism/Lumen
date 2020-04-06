#nullable enable

using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class BinaryExpression : Expression {
		internal Expression expressionOne;
		internal Expression? expressionTwo;
		internal String operation;
		private Int32 line;
		private String fileName;

		public BinaryExpression(Expression expressionOne, Expression? expressionTwo, String operation, Int32 line, String fileName) {
			this.expressionOne = expressionOne;
			this.expressionTwo = expressionTwo;
			this.operation = operation;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(ClosureManager manager) {
			return new BinaryExpression(this.expressionOne.Closure(manager), this.expressionTwo?.Closure(manager), this.operation, this.line, this.fileName);
		}

		public XnObject Eval(Scope scope) {
			XnObject operandOne = this.expressionOne.Eval(scope);

			if (this.expressionTwo != null) {
				if (this.operation == Operation.AND) {
					return new Atom((!XnStd.AsBool(operandOne) ? false : XnStd.AsBool(this.expressionTwo.Eval(scope))) ? 1 : 0);
				}

				if (this.operation == Operation.OR) {
					return new Atom((XnStd.AsBool(operandOne) ? true : XnStd.AsBool(this.expressionTwo.Eval(scope))) ? 1 : 0);
				}

				if (this.operation == Operation.XOR) {
					return new Atom((XnStd.AsBool(operandOne) ^ XnStd.AsBool(this.expressionTwo.Eval(scope))) ? 1 : 0);
				}

				XnObject operandTwo = this.expressionTwo?.Eval(scope);
				return this.Apply(operandOne, operandTwo, operation, scope);
			}
			else {
				if (this.operation == Operation.NOT) {
					return new Atom(!XnStd.AsBool(operandOne) ? 1 : 0);
				}

				return this.Apply(operandOne, operation, scope);
			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		private XnObject Apply(XnObject obj1, string operation, Scope scope) {
			KsTypeable a = obj1.Type;

			if (a is KsType type && type.EigenNamespace.variables.TryGetValue(operation, out var x)) {
				return (x as Function).Run(new Scope(), obj1);
			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}

		private XnObject Apply(XnObject obj1, XnObject obj2, string operation, Scope scope) {
			KsTypeable a = obj1.Type;

			if(a is KsType type && type.EigenNamespace.variables.TryGetValue(operation, out var x)) {
				return (x as Function).Run(new Scope(), obj1, obj2);
			}

			throw new XenonException(Exceptions.INVALID_OPERATION);
		}
	}
}