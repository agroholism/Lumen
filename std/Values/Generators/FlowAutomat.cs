﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class FlowAutomat : BaseValueImpl, IEnumerator<Value> {
		private IEnumerator<Value> internalValue;
		private Scope scope;

		public override IType Type => FlowModule.Automat;

		public Value Current => this.internalValue.Current;
		Object IEnumerator.Current => this.internalValue.Current;

		public Value AutomatResult => 
			this.scope.ExistsInThisScope(Constants.YIELD_RESULT_SPECIAL_NAME) ? this.scope[Constants.YIELD_RESULT_SPECIAL_NAME] : null;

		public FlowAutomat(IEnumerator<Value> internalValue, Scope scope) {
			this.internalValue = internalValue;
			this.scope = scope;
		}

		public Value Send(Value value) {
			this.scope?.Bind(Constants.YIELD_VALUE_SPECIAL_NAME, value);

			return this.Next();
		}

		public Value Throw(Value value) {
			this.scope?.Bind(Constants.YIELD_EXCEPTION_SPECIAL_NAME, value);

			return this.Next();
		}

		public Value Next() {
			this.MoveNext();
			return this.Current;
		}

		public void Dispose() {
			this.internalValue.Dispose();
		}

		public Boolean MoveNext() {
			return this.internalValue.MoveNext();
		}

		public Boolean MoveNext(Value value) {
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
		public GeneratorExpressionTerminalResult(Value value) {
			this.Value = value;
		}

		public Value Value { get; set; }
		public override IType Type => throw new NotImplementedException();
	}
}
