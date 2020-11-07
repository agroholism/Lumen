﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang {
	public class MutableMap : Value {
		internal Dictionary<Value, Value> InternalValue { get; private set; }

		public MutableMap() {
			this.InternalValue = new Dictionary<Value, Value>();
		}

		public MutableMap(Dictionary<Value, Value> value) {
			this.InternalValue = value;
		}

		public override Boolean Equals(Object obj) {
			if (obj is MutableMap map) {
				if (ReferenceEquals(this, map)) {
					return true;
				}

				if (map.InternalValue.Count != this.InternalValue.Count) {
					return false;
				}

				return this.InternalValue.Zip(map.InternalValue, (p1, p2) => p1.Key.Equals(p2.Key) && p1.Value.Equals(p2.Value)).All(x => x);
			}
			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return -342344534;
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public IType Type => Prelude.MutableMap;

		public override String ToString() {
			if (this.InternalValue.Count == 0) {
				return "[:]";
			}

			StringBuilder builder = new StringBuilder("[");
			Int32 indexer = 0;
			foreach (KeyValuePair<Value, Value> i in this.InternalValue) {
				builder.Append(i.Key + " : " + i.Value.ToString());
				if (indexer < this.InternalValue.Count - 1) {
					builder.Append(", ");
				}

				indexer++;
			}
			builder.Append("]");
			return builder.ToString();
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}
	}
}