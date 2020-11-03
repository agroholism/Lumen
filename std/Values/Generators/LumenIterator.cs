using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class LumenIterator : BaseValueImpl, IEnumerator<Value> {
		private IEnumerator<Value> internalValue;
		private Scope scope;

		public override IType Type => Prelude.Iterator;

		public Value Current => this.internalValue.Current;
		Object IEnumerator.Current => this.internalValue.Current;

		public LumenIterator(IEnumerator<Value> internalValue, Scope scope) {
			this.internalValue = internalValue;
			this.scope = scope;
		}

		public Value Send(Value value) {
			this.scope?.Bind(Constants.YIELD_VALUE_SPECIAL_NAME, value);

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

		public void Reset() {
			this.internalValue.Reset();
		}

		public override String ToString() {
			return $"[Iterator #{this.GetHashCode()}]";
		}
	}
}
