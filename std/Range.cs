using System;
using System.Collections;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Range : IEnumerator<Value> {
		readonly Value begin;
		readonly Value end;
		readonly Value step;
		Boolean flag;

		public Range(Value begin, Value end, Value step) {
			this.begin = begin;
			this.end = end;
			this.step = step;
			this.Current = begin;
		}

		public Value Current { get; private set; }

		Object IEnumerator.Current {
			get => this.Current;
		}

		public void Dispose() {

		}

		public Boolean MoveNext() {
			Scope s = new Scope(null) {
				This = this.Current
			};

			if (!this.flag) {
				this.flag = true;
				return Converter.ToBoolean(((Fun)this.Current.Type.Get("!=", null)).Run(s, this.end));
			}

			if (this.step == null) {
				this.Current = ((Fun)this.Current.Type.Get("++", null)).Run(s);
			}
			else {
				this.Current = ((Fun)this.Current.Type.Get("+", null)).Run(s, this.step);
			}

			return Converter.ToBoolean(((Fun)this.Current.Type.Get("!=", null)).Run(s, this.end));
		}

		public void Reset() {
			this.Current = this.begin;
		}
	}
}
