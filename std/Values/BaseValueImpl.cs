using System;

namespace Lumen.Lang {
	public abstract class BaseValueImpl : Value {
		public abstract IType Type { get; }

		public virtual Int32 CompareTo(Object obj) {
			if (obj is Value value) {
				if (this.Type.HasImplementation(Prelude.Ord)
					&& this.Type.GetMember("compare", null).TryConvertToFunction(out Fun comparator)) {
					return comparator.Run(new Scope(), this, value).ToInt(null);
				}
			}

			throw new LumenException("its not ord");
		}

		public virtual String ToString(String format, IFormatProvider formatProvider) {
			if (this.Type.HasImplementation(Prelude.Format) &&
				this.Type.GetMember("format", null).TryConvertToFunction(out Fun function)) {
				function.Run(new Scope(), this, new Text(format ?? "")).ToString();
			}

			return this.ToString();
		}
	}
}
