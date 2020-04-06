using System;
using System.Collections.Generic;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class NullableExpression : Expression {
		private Expression res;

		public NullableExpression(Expression res) {
			this.res = res;
		}

		public Expression Closure(ClosureManager manager) {
			return new NullableExpression(this.res.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			XnObject ksObject = this.res.Eval(scope);

			if(ksObject is KsTypeable typeable) {
				return XnStd.GetNullable(typeable);
			}

			throw new Exception();
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}

	internal class ImmutExpression : Expression {
		private Expression res;

		public ImmutExpression(Expression res) {
			this.res = res;
		}

		public Expression Closure(ClosureManager manager) {
			return new ImmutExpression(this.res.Closure(manager));
		}

		public XnObject Eval(Scope scope) {
			XnObject ksObject = this.res.Eval(scope);

			if (ksObject is KsTypeable typeable) {
				return XnStd.GetMutable(typeable);
			}

			throw new Exception();
		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}