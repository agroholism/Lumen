﻿using System;

namespace Lumen.Lang {
	public abstract class BaseValueImpl : IValue {
		public abstract IType Type { get; }

		public virtual Int32 CompareTo(Object obj) {
			if (obj is IValue value) {
				if (this.Type.HasImplementation(Prelude.Ord)
					&& this.Type.GetMember("compare", null).TryConvertToFunction(out Fun comparator)) {
					return comparator.Call(new Scope(), this, value).ToInt(null);
				}
			}

			throw new LumenException("its not ord");
		}

		public virtual String ToString(String format, IFormatProvider formatProvider) {
			if (this.Type.HasImplementation(Prelude.Format) &&
				this.Type.GetMember("format", null).TryConvertToFunction(out Fun function)) {
				function.Call(new Scope(), this, new Text(format ?? "")).ToString();
			}

			return this.ToString();
		}
	}
}
