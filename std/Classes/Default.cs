using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Default : SystemClass {
		internal Default() : base("Default") {
			this.SetMember("default", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Default.default", scope["self"].Type);
				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}
	}
}
