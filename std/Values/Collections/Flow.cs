using System;
using System.Linq;
using System.Collections.Generic;

namespace Lumen.Lang {
	public sealed class Flow : IValue {
		internal IEnumerable<IValue> InternalValue { get; private set; }

		public static Flow Empty => new Flow(Enumerable.Empty<IValue>());

		public Flow(IEnumerable<IValue> innerValue) {
			this.InternalValue = innerValue;
		}

		public IValue Clone() {
			return (IValue)this.MemberwiseClone();
		}

		public Int32 CompareTo(Object obj) {
			throw new LumenException($"can not compare value of type 'Kernel.Enumerator' with value of type '{obj.GetType()}'");
		}

		public Boolean IsCustomFlow { get => this.InternalValue is CustomFlow; }

		public FlowAutomat GetAutomat() {
			if (this.IsCustomFlow) {
				return (this.InternalValue as CustomFlow).GetEnumerator() as FlowAutomat;
			}

			return null;
		}

		public IType Type => Prelude.Seq;

		public override String ToString() {
			return $"[Flow #{this.GetHashCode()}]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return this.ToString();
		}
	}
}
