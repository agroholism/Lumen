using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class FlowAutomat : BaseValueImpl, IEnumerator<IValue> {
		private IEnumerator<IValue> internalValue;
		private Scope scope;

		public override IType Type => Prelude.Flow.Automat;

		public IValue Current => this.internalValue.Current;
		Object IEnumerator.Current => this.internalValue.Current;

		public IValue Result => 
			this.scope.IsExistsInThisScope(Constants.YIELD_RESULT_SPECIAL_NAME) ? this.scope[Constants.YIELD_RESULT_SPECIAL_NAME] : null;

		public FlowAutomat(IEnumerator<IValue> internalValue, Scope scope) {
			this.internalValue = internalValue;
			this.scope = scope;
		}

		public Boolean Throw(IValue value) {
			this.scope?.Bind(Constants.YIELD_EXCEPTION_SPECIAL_NAME, value);

			return this.MoveNext();
		}

		public IValue Next() {
			this.MoveNext();
			return this.Current;
		}

		public void Dispose() {
			this.internalValue.Dispose();
		}

		public Boolean MoveNext() {
			return this.internalValue.MoveNext();
		}

		public Boolean MoveNext(IValue value) {
			this.scope?.Bind(Constants.YIELD_VALUE_SPECIAL_NAME, value);

			return this.internalValue.MoveNext();
		}

		public void Reset() {
			this.internalValue.Reset();
		}

		public override String ToString() {
			return $"[Flow.Automat #{this.GetHashCode()}]";
		}
	}

	public class GeneratorExpressionTerminalResult : BaseValueImpl {
		public GeneratorExpressionTerminalResult(IValue value) {
			this.Value = value;
		}

		public IValue Value { get; set; }
		public override IType Type => throw new NotImplementedException();
	}
}
