using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class StateModule : Module {
		public StateModule() {
			this.Name = "Ref";

			this.Mixins.Add(Prelude.Format);
			this.Mixins.Add(Prelude.Functor);

			this.SetMember("init", new LambdaFun((scope, args) => {
				return new State(scope["state"]);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("fmap", new LambdaFun((scope, args) => {
				State functor = scope["fc"] as State;
				Fun mapper = scope["fn"].ToFunction(scope);

				return new State(mapper.Run(new Scope(scope), functor.Value));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			this.SetMember("!", new LambdaFun((scope, args) => {
				if (scope["state"] is ArrayRef arrref) {
					return arrref.Value.InternalValue[arrref.Index];
				}

				return (scope["state"] as State).Value;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("unit")
				}
			});

			this.SetMember("<-", new LambdaFun((scope, args) => {
				if(scope["state"] is ArrayRef arrref) {
					return arrref.Value.InternalValue[arrref.Index] = scope["value"];
				}

				State state = scope["state"] as State;
				return state.Value = scope["value"];
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("value"),
				}
			});

			this.SetMember("swap", new LambdaFun((scope, args) => {
				State state = scope["state"] as State;
				State state2 = scope["state'"] as State;
				Value temp = state.Value;
				state.Value = state2.Value;
				state2.Value = temp;
				return state;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state"),
					new NamePattern("state'")
				}
			});

			this.SetMember("inc", new LambdaFun((scope, args) => {
				State state = scope["state"] as State;
				state.Value = new Number(state.Value.ToDouble(scope) + 1);
				return state;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("dec", new LambdaFun((scope, args) => {
				State state = scope["state"] as State;
				state.Value = new Number(state.Value.ToDouble(scope) - 1);
				return state;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("state")
				}
			});

			this.SetMember("format", new LambdaFun((scope, args) => {
				State inc = scope.Get("state") as State;
				System.String fstr = scope.Get("fstr").ToString();
				if (fstr == "v") {
					return new Text(inc.Value.ToString());
				}
				else {
					return new Text(inc.ToString());
				}
			}) {
				Arguments = new List<IPattern> {
						new NamePattern("state"),
						new NamePattern("fstr")
					}
			});
		}
	}
}
