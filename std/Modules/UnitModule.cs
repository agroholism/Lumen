using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class UnitModule : Module {
		internal UnitModule() {
			this.Name = "Unit";

			this.SetMember("toText", new LambdaFun((e, args) => {
				return new Text(e["this"].ToString());
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("this")
				}
			});
		}
	}
}
