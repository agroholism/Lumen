using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Default : Module {
		internal Default() {
			this.Name = "Default";

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
