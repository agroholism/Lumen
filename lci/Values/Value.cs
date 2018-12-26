using System;

namespace StandartLibrary {
	public interface Value : IComparable {
		KType Type { get; }

		String ToString(Scope e);

		Value Clone();
	}
}
