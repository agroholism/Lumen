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

			this.SetMember("onEnter", new LambdaFun((scope, args) => {
				return scope["x"];
			}) {
				Name = "onEnter",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("onExit", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Context.onExit", scope["x"].Type, scope);
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
