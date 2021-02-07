using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Functor : SystemClass {
		internal Functor() : base("Functor") {
			this.SetMember(">>", new LambdaFun((scope, args) => {
				Value functor = scope["functor"];
				return functor.Type.GetMember("map").ToFunction(scope)
					.Call(new Scope(scope), scope["function"], functor);
			}) {
				Name = ">>",
				Parameters = new List<IPattern> {
					new NamePattern("functor"),
					new NamePattern("function")
				}
			});

			this.SetMember("<<", new LambdaFun((scope, args) => {
				Value functor = scope["functor"];
				return functor.Type.GetMember("map").ToFunction(scope)
					.Call(new Scope(scope), scope["function"], functor);
			}) {
				Name = "<<",
				Parameters = new List<IPattern> {
					new NamePattern("function"),
					new NamePattern("functor")
				}
			});
		}

		public override void OnImplement(Module target) {
			target.SetMemberIfAbsent("map", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Functor.map", target);
				return Const.UNIT;
			}) {
				Name = "map",
				Parameters = new List<IPattern> {
					new NamePattern("function"),
					new NamePattern("functor")
				}
			});
		}
	}
}
