using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang {
	public class MutMap : IValue {
		internal Dictionary<IValue, IValue> InternalValue { get; private set; }

		public MutMap() {
			this.InternalValue = new Dictionary<IValue, IValue>();
		}

		public MutMap(Dictionary<IValue, IValue> value) {
			this.InternalValue = value;
		}

		public override Boolean Equals(Object obj) {
			if (obj is MutMap map) {
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

		public IType Type => Prelude.MutMap;

		public override String ToString() {
			if (this.InternalValue.Count == 0) {
				return "[:]";
			}

			StringBuilder builder = new StringBuilder("[");
			Int32 indexer = 0;
			foreach (KeyValuePair<IValue, IValue> i in this.InternalValue) {
				builder.Append(i.Key + ": " + i.Value.ToString());
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
