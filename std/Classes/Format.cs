using System.Collections.Generic;

using Lumen.Lang.Expressions;

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
	internal class Format : Module {
        internal Format() {
            this.Name = "Format";
			this.EntitiyType = EntitiyType.MODULE;

			this.SetMember("format", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Format.format", scope["x"].Type.ToString());
				return Const.UNIT;
			}) {
				Name = "format",
				Arguments = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("fstr")
				}
			});
		}
    }
}
