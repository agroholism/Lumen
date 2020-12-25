using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Monad : Module {
		internal Monad() {
			this.Name = "Monad";

			this.SetMember("bind", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Monad.bind", scope["x"].Type);
				return Const.UNIT;
			}) {
				Name = "bind",
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("monad")
				}
			});

			this.SetMember("pure", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Monad.bind", new Text("T"));
				return Const.UNIT;
			}) {
				Name = "pure",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember(">>=", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind", scope).ToFunction(scope).Call(scope, scope["fn"], monad);
			}) {
				Name = ">>=",
				Parameters = new List<IPattern> {
					new NamePattern("monad"),
					new NamePattern("fn")
				}
			});

			this.SetMember("=<<", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind", scope).ToFunction(scope).Call(scope, scope["fn"], monad);
			}) {
				Name = "=<<",
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("monad")
				}
			});
		}
	}
}
