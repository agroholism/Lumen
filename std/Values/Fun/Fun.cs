using System;
using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	/// <summary> Delegate for Lumen function </summary>
	/// <param name="scope"> Scope for execution </param>
	public delegate IValue LumenFunc(Scope scope, params IValue[] args);

	/// <summary> Interface for any Lumen function value </summary>
	public interface Fun : IValue {
		/// <summary> Name of function </summary>
		String Name { get; set; }

		/// <summary> Arguments of function </summary>
		List<IPattern> Parameters { get; set; }

		/// <summary> This method runs a function </summary>
		IValue Call(Scope e, params IValue[] args);
	}
}
