using System.Collections.Generic;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class MutModule : Module {
		public MutModule() {
			this.Name = "Mut";

			this.AppendImplementation(Prelude.Format);
			this.AppendImplementation(Prelude.Functor);
			this.AppendImplementation(Prelude.Applicative);
			this.AppendImplementation(Prelude.Default);

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Mut(Const.UNIT);
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return new Mut(scope["state"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("fmap", new LambdaFun((scope, args) => {
				Mut functor = scope["fc"] as Mut;
				Fun mapper = scope["fn"].ToFunction(scope);

				return new Mut(mapper.Call(new Scope(scope), functor.Value));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			this.SetMember("liftA", new LambdaFun((scope, args) => {
				Mut functor = scope["f"] as Mut;
				Value m = scope["m"];
				return m.CallMethodFlip("fmap", scope, functor.Value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});

			this.SetMember("!", new LambdaFun((scope, args) => {
				return (scope["state"] as Mut).Value;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("unit")
				}
			});

			this.SetMember("<-", new LambdaFun((scope, args) => {
				Mut state = scope["state"] as Mut;
				return state.Value = scope["value"];
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("value"),
				}
			});

			this.SetMember("swap", new LambdaFun((scope, args) => {
				Mut state = scope["state"] as Mut;
				Mut state2 = scope["state'"] as Mut;
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
				Mut state = scope["state"] as Mut;
				state.Value = new Number(state.Value.ToDouble(scope) + 1);
				return state;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("dec", new LambdaFun((scope, args) => {
				Mut state = scope["state"] as Mut;
				state.Value = new Number(state.Value.ToDouble(scope) - 1);
				return state;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("format", new LambdaFun((scope, args) => {
				Mut inc = scope.Get("state") as Mut;
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
