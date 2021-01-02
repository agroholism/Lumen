using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal sealed class FunctionModule : Module {
		internal FunctionModule() {
			this.Name = "Function";

			this.AppendImplementation(Prelude.Monoid);
			this.AppendImplementation(Prelude.Monad);

			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
				Fun m = scope["fc"].ToFunction(scope);
				Fun f = scope["fn"].ToFunction(scope);

				return new LambdaFun((inScope, inArgs) => {
					Value x = inScope["x"];

					Value mx = m.Call(new Scope(), x);
					Value fx = f.Call(new Scope(), x);

					return mx.Type.GetMember("+", inScope).ToFunction(inScope)
						.Call(new Scope(), mx, fx);
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("x")
					}
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fc"),
					new NamePattern("fn"),
				}
			});

			LambdaFun emptyFunction = new LambdaFun((inScope, inArgs) => {
				return inScope["x"].Type
							.GetMember("empty", inScope)
							.ToFunction(inScope)
							.Call(new Scope(), Const.UNIT);
			}) {
				Parameters = new List<IPattern> { new NamePattern("x") }
			};

			this.SetMember("empty", new LambdaFun((scope, args) => emptyFunction) {
				Parameters = new List<IPattern> {
					new NamePattern("_"),
				}
			});

			this.SetMember("pure", new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new LambdaFun((inScope, inArgs) => x) {
					Parameters = new List<IPattern> {
						new NamePattern("_")
					}
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x"),
				}
			});

			this.SetMember("map", new LambdaFun((scope, args) => {
				Fun functor = scope["functor"].ToFunction(scope);
				Fun function = scope["function"].ToFunction(scope);

				return new LambdaFun((inScope, inArgs) =>
					function.Call(new Scope(inScope), functor.Call(new Scope(inScope), inArgs))) {
					Parameters = functor.Parameters
				};
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("function", this),
					new ExactTypePattern("functor", this),
				}
			});

			this.SetMember("lift", new LambdaFun((scope, args) => {
				Fun func = scope["func"].ToFunction(scope);
				Fun appl = scope["appl"].ToFunction(scope);

				return new LambdaFun((e, a) => {
					Value arg = e["x"];

					return appl.Call(new Scope(), arg).ToFunction(e)
						.Call(new Scope(), func.Call(new Scope(), arg));
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("x")
					}
				};
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("func", this),
					new ExactTypePattern("appl", this),
				}
			});
			
			this.SetMember("bind", new LambdaFun((scope, args) => {
				Fun func = scope["func"].ToFunction(scope);
				Fun monad = scope["monad"].ToFunction(scope);

				return new LambdaFun((inScope, inArgs) => {
					Value arg = inScope["x"];

					Value hres = monad.Call(new Scope(), arg);
					Fun fres = func.Call(new Scope(), hres).ToFunction(inScope);

					return fres.Call(new Scope(), arg);
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("x")
					}
				};
			}) {
				Parameters = new List<IPattern> {
					new ExactTypePattern("func", this),
					new ExactTypePattern("monad", this),
				}
			});

			Fun ConcatWith(Fun m, Fun f, Fun with) {
				return new LambdaFun((inScope, inArgs) => {
					Value x = inScope["x"];

					Value mx = m.Call(new Scope(), x);
					Value fx = f.Call(new Scope(), x);

					return with.Call(new Scope(), mx, fx);
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("x")
					}
				};
			}

			this.SetMember("concatWith", new LambdaFun((scope, args) => {
				IEnumerable<Value> functions = scope["functions"].ToSeq(scope);
				Fun with = scope["with"].ToFunction(scope);

				return functions.Select(i => i.ToFunction(scope))
					.Aggregate((x, y) => ConcatWith(x, y, with));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("functions"),
					new NamePattern("with"),
				}
			});
		}
	}
}
