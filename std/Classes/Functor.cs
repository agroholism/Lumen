using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	/// <summary>
	/// That's Format mixin. Used in string interpolation with invariant culture
	/// </summary>
	/// module Prelude where
	///		...
	///		/// Require let format x (fstr: Text)
	///		module Functor where
	///			let >>- (fc: 'T) fn =
	///				functionIsNotImplementedForType "Functor.>>-" 'T
	internal class Functor : Module {
		internal Functor() {
			this.Name = "Functor";
			this.EntitiyType = EntitiyType.MODULE;

			this.SetMember("fmap", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Functor.fmap", scope["x"].Type.ToString());
				return Const.UNIT;
			}) {
				Name = "fmap",
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc")
				}
			});

			this.SetMember(">>-", new LambdaFun((scope, args) => {
				Value fc = scope["fc"];
				return fc.Type.GetMember("fmap", scope).ToFunction(scope).Run(scope, scope["fn"], fc);
			}) {
				Name = ">>-",
				Arguments = new List<IPattern> {
					new NamePattern("fc"),
					new NamePattern("fn")
				}
			});
		}
	}
}
