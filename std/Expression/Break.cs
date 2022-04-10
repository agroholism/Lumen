using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class Break : Exception, Expression {
		private String label;

		public Break(String label) : base("unexpected break with label " + label) {
			this.label = label;
		}

		public Boolean IsMatch(String otherLabel) {
			return this.label == otherLabel;
		}

		public IValue Eval(Scope e) {
			throw this;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			throw this;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}

	public class Redo : Exception, Expression {
		private String label;

		public Redo(String label) : base("unexpected redo with label " + label) {
			this.label = label;
		}

		public Boolean IsMatch(String otherLabel) {
			return this.label == otherLabel;
		}

		public IValue Eval(Scope e) {
			throw this;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			throw this;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}

	public class Retry : Exception, Expression {
		public Retry() : base("unexpected retry ") {
		}

		public IValue Eval(Scope e) {
			throw this;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			throw this;
		}

		public Expression Closure(ClosureManager manager) {
			return this;
		}
	}
}