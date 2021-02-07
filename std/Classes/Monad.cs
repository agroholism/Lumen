using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Monad : SystemClass {
		internal Monad() : base ("Monad") {
			this.AppendImplementation(Prelude.Applicative);

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

			this.SetMember(">>=", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind").ToFunction(scope).Call(new Scope(), scope["fn"], monad);
			}) {
				Name = ">>=",
				Parameters = new List<IPattern> {
					new NamePattern("monad"),
					new NamePattern("fn")
				}
			});

			this.SetMember("=<<", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind").ToFunction(scope).Call(new Scope(), scope["fn"], monad);
			}) {
				Name = "=<<",
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("monad")
				}
			});

			this.SetMember("lift", new LambdaFun((scope, args) => {
				Value mf = scope["mf"];
				Value ma = scope["ma"];

				Fun wrap = mf.Type.GetMember("wrap").ToFunction(scope);
				Fun mabind = ma.Type.GetMember(">>=").ToFunction(scope);

				return mf.CallMethod(">>=", scope, new LambdaFun((inScope, inArgs) => {
					Fun f = inScope["f"].ToFunction(inScope); // mf >>= fun f -> ...

					return mabind.Call(new Scope(scope), ma, new LambdaFun((inScope2, inArgs2) => {
						Value a = inScope2["a"]; // ma >>= fun a -> ...

						Value fa = f.Call(new Scope(), a); // f a

						return wrap.Call(new Scope(), fa); // pure (f a)

					}) {
						Parameters = new List<IPattern> {
							new NamePattern("a")
						}
					});
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("f")
					}
				});
			}) {
				Name = "lift",
				Parameters = new List<IPattern> {
					new NamePattern("ma"),
					new NamePattern("mf")
				}
			});
		}
	}
}
