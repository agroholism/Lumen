using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Context : Module {
		internal Context() {
			this.Name = "Context";

			this.SetMember("onEnter", new LambdaFun((scope, args) => {
				return scope["x"];
			}) {
				Name = "onEnter",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("onExit", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Context.onExit", scope["x"].Type);
				return Const.UNIT;
			}) {
				Name = "onExit",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});
		}
	}
}
