using System;

namespace Lumen.Lang {
	public abstract class BaseValueImpl : Value {
		public abstract IType Type { get; }

		public virtual Value Clone() {
			return this;
		}

		public virtual Int32 CompareTo(Object obj) {
			if(obj is Value value) {
				if (this.Type.HasMixin(Prelude.Ord)) {
					return (this.Type.GetMember("compare", null) as Fun).Run(new Scope(), this, value).ToInt(null);
				}
			}

			throw new LumenException("its not ord");
		}

		public virtual String ToString(String format, IFormatProvider formatProvider) {
			if (this.Type.HasMixin(Prelude.Format)) {
				return (this.Type.GetMember("format", null) as Fun).Run(new Scope(), this, format == null ? new Text("") : new Text(format)).ToString();
			}

			return this.ToString();
		}
	}
}
