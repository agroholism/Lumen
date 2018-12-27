using System;

namespace Lumen.Lang.Std {
	public interface Value : IComparable {
		Record Type { get; }

		String ToString(Scope e);

		Value Clone();
	}
}
