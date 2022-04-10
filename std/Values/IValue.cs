using System;

namespace Lumen.Lang {
	public interface IValue : IComparable, IFormattable {
		IType Type { get; }
	}
}
