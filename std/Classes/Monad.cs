using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Monad : SystemClass {
		internal Monad() {
			this.Name = "Monad";

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

			this.SetMember("pure", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Monad.bind", new Text("'T"));
				return Const.UNIT;
			}) {
				Name = "pure",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember(">>=", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind", scope).ToFunction(scope).Call(new Scope(), scope["fn"], monad);
			}) {
				Name = ">>=",
				Parameters = new List<IPattern> {
					new NamePattern("monad"),
					new NamePattern("fn")
				}
			});

			this.SetMember("=<<", new LambdaFun((scope, args) => {
				Value monad = scope["monad"];
				return monad.Type.GetMember("bind", scope).ToFunction(scope).Call(new Scope(), scope["fn"], monad);
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

				System.Console.WriteLine("Monad.lift");

				return mf.CallMethod(">>=", scope, new LambdaFun((inScope, inArgs) => {
					Fun f = inScope["f"].ToFunction(inScope); // mf >>= fun f -> ...

					return ma.CallMethod(">>=", scope, new LambdaFun((inScope2, inArgs2) => {
						Value a = inScope2["a"]; // ma >>= fun a -> ...

						Value fa = f.Call(new Scope(), a); // f a

						// pure fa
						return mf.Type.GetMember("pure", inScope2)
								.ToFunction(inScope2)
								.Call(new Scope(), fa);

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
