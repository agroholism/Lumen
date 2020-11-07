using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Clone : Module {
		internal Clone() {
			this.Name = "Clone";

			this.SetMember("clone", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Clone.clone", scope["self"].Type, scope);
				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}
	}
}
