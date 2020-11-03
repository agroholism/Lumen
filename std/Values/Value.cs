using System;

namespace Lumen.Lang {
	public interface Value : IComparable, IFormattable {
		IType Type { get; }
	}
}
