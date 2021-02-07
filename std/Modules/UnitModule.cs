using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class UnitModule : Type {
		internal UnitModule() : base("Unit") {
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
