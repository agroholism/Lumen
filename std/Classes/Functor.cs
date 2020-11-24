using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	/// module Prelude where
	///		...
	///		/// Require let format x (fstr: Text)
	///		module Functor where
	///			let >>- (fc: 'T) fn =
	///				functionIsNotImplementedForType "Functor.>>-" 'T
	internal class Functor : Module {
		internal Functor() {
			this.Name = "Functor";

			this.SetMember("fmap", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Functor.fmap", scope["x"].Type, scope);
				return Const.UNIT;
			}) {
				Name = "fmap",
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc")
				}
			});

			this.SetMember(">>-", new LambdaFun((scope, args) => {
				Value fc = scope["fc"];
				return fc.Type.GetMember("fmap", scope).ToFunction(scope).Call(new Scope(scope), scope["fn"], fc);
			}) {
				Name = ">>-",
				Parameters = new List<IPattern> {
					new NamePattern("fc"),
					new NamePattern("fn")
				}
			});

			this.SetMember("-<<", new LambdaFun((scope, args) => {
				Value fc = scope["fc"];
				return fc.Type.GetMember("fmap", scope).ToFunction(scope).Call(new Scope(scope), scope["fn"], fc);
			}) {
				Name = "-<<",
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc")
				}
			});
		}
	}
}
