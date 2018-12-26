using System;

namespace Lumen.Lang.Std {
	public interface Value : IComparable {
		KType Type { get; }

		String ToString(Scope e);

		Value Clone();
	}
}
