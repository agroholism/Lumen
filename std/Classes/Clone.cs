using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Clone : SystemClass {
		internal Clone() {
			this.Name = "Clone";
		}

		public override void OnImplement(Module target) {
			target.SetMemberIfAbsent("clone", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Clone.clone", target);
				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}
	}
}
