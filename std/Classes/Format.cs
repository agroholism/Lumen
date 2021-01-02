using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	/// <summary>
	/// That's Format mixin. Used in string interpolation with invariant culture
	/// </summary>
	/// module Prelude where
	///		...
	///		/// Require let format x (fstr: Text)
	///		module Format where
	///			let format (x: 'T) (fstr: Text) =
	///				functionIsNotImplementedForType "Format.format" 'T
	internal class Format : SystemClass {
		internal Format() {
			this.Name = "Format";

			this.SetMember("format", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Format.format", scope["x"].Type);
				return Const.UNIT;
			}) {
				Name = "format",
				Parameters = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("fstr")
				}
			});
		}
	}
}
