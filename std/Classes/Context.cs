using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	/// module Prelude where
	///		...
	///		module Context where
	///			let format (x: 'T) (fstr: Text) =
	///				functionIsNotImplementedForType "Format.format" 'T
	internal class Context : Module {
		internal Context() {
			this.Name = "Context";
			this.EntitiyType = EntitiyType.MODULE;

			this.SetMember("onEnter", new LambdaFun((scope, args) => {
				return scope["x"];
			}) {
				Name = "onEnter",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("onExit", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Context.onExit", scope["x"].Type.ToString());
				return Const.UNIT;
			}) {
				Name = "onExit",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});
		}
	}
}
