using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class UnitModule : Module {
		internal UnitModule() {
			this.Name = "Unit";

			this.SetMember("toText", new LambdaFun((scope, args) => {
				return new Text(scope["self"].ToString());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}
	}
}
