using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Container : SystemClass {
		internal Container () {
			this.Name = "Container";

			this.AppendImplementation(Prelude.Collection);
			this.AppendImplementation(Prelude.Monoid);
			this.AppendImplementation(Prelude.Monad);

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Container.fromSeq", scope["self"].Type);
				return Const.UNIT;
			}) {
				Name = "fromSeq",
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});
		}

		public override void OnImplement(Module target) {
			target.SetMemberIfAbsent("empty", new LambdaFun((scope, args) => {
				return target.GetMember("fromSeq", scope)
					.ToFunction(scope).Call(new Scope(scope), Flow.Empty);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("_")
				}
			});
		}
	}
}
