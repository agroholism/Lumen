using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
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
