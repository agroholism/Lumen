using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class LumenIterator : BaseValueImpl, IEnumerator<Value> {
		public IEnumerator<Value> InnerValue { get; }
		public Scope Scope { get; set; }

		public override IType Type => Prelude.Iterator;

		public Value Current => this.InnerValue.Current;

		Object IEnumerator.Current => this.InnerValue.Current;

		public LumenIterator(IEnumerator<Value> innerValue) {
			this.InnerValue = innerValue;
		}

		public Value Send(Value value) {
			if (this.Scope != null) {
				this.Scope["<curr-gen-val>"] = value;
			}

			this.MoveNext();
			return this.Current;
		}

		public Value Next() {
			this.MoveNext();
			return this.Current;
		}

		public void Dispose() {
			this.InnerValue.Dispose();
		}

		public Boolean MoveNext() {
			return this.InnerValue.MoveNext();
		}

		public void Reset() {
			this.InnerValue.Reset();
		}

		public override String ToString() {
			return $"[Iterator #{this.GetHashCode()}]";
		}
	}
}
