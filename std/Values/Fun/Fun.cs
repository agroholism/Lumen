using System;
using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	/// <summary> Delegate for Lumen function </summary>
	/// <param name="scope"> Scope for execution </param>
	public delegate Value LumenFunc(Scope scope, params Value[] args);

	/// <summary> Interface for any Lumen function value </summary>
	public interface Fun : Value {
		/// <summary> Name of function </summary>
		String Name { get; set; }

		/// <summary> Arguments of function </summary>
		List<IPattern> Parameters { get; set; }

		/// <summary> This method runs a function </summary>
		Value Call(Scope e, params Value[] args);
	}
}
