using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Cloneable : Module {
		internal Cloneable() {
			this.Name = "Cloneable";
			this.EntitiyType = EntitiyType.MODULE;

			this.SetMember("clone", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Cloneable.clone", scope["self"].Type.ToString());
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}
	}
}
