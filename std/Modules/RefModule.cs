using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class RefModule : Module {
		public RefModule() {
			this.Name = "Ref";

			this.AppendImplementation(Prelude.Format);
			this.AppendImplementation(Prelude.Functor);
			this.AppendImplementation(Prelude.Applicative);
			this.AppendImplementation(Prelude.Default);

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Ref(Const.UNIT);
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return new Ref(scope["state"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("fmap", new LambdaFun((scope, args) => {
				Ref functor = scope["fc"] as Ref;
				Fun mapper = scope["fn"].ToFunction(scope);

				return new Ref(mapper.Call(new Scope(scope), functor.Value));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			this.SetMember("liftA", new LambdaFun((scope, args) => {
				Ref functor = scope["f"] as Ref;
				Value m = scope["m"];
				return m.CallMethodFlip("fmap", scope, functor.Value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});

			this.SetMember("!", new LambdaFun((scope, args) => {
				return (scope["state"] as Ref).Value;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("unit")
				}
			});

			this.SetMember("<-", new LambdaFun((scope, args) => {
				Ref state = scope["state"] as Ref;
				return state.Value = scope["value"];
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("value"),
				}
			});

			this.SetMember("swap", new LambdaFun((scope, args) => {
				Ref state = scope["state"] as Ref;
				Ref state2 = scope["state'"] as Ref;
				Value temp = state.Value;
				state.Value = state2.Value;
				state2.Value = temp;
				return state;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("state'")
				}
			});

			this.SetMember("inc", new LambdaFun((scope, args) => {
				Ref state = scope["state"] as Ref;
				state.Value = new Number(state.Value.ToDouble(scope) + 1);
				return state;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("dec", new LambdaFun((scope, args) => {
				Ref state = scope["state"] as Ref;
				state.Value = new Number(state.Value.ToDouble(scope) - 1);
				return state;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("format", new LambdaFun((scope, args) => {
				Ref inc = scope.Get("state") as Ref;
				System.String fstr = scope.Get("fstr").ToString();
				if (fstr == "v") {
					return new Text(inc.Value.ToString());
				}
				else {
					return new Text(inc.ToString());
				}
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("state"),
						new NamePattern("fstr")
					}
			});
		}
	}
}
