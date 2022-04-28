using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class SeqModule : Module {
		public AutomatModule Automat { get; } = new AutomatModule();

		public SeqModule() {
			this.Name = "Seq";

			this.SetMember("Automat", Automat);

			this.SetMember("empty", new LambdaFun((scope, args) => {
				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("wrap", new LambdaFun((scope, args) => {
				return new Flow(Enumerable.Repeat(scope["init"], 1));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("replicate", new LambdaFun((scope, args) => {
				return new Flow(Enumerable.Repeat(scope["init"], scope["len"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("len")
				}
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new Flow(Enumerable.Empty<IValue>());
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("toSeq", new LambdaFun((e, args) => {
				return new Flow(e["this"].ToList(e));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("this")
				}
			});

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Name = "fromSeq",
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Container);
		}

		internal class AutomatModule : Module {
			public AutomatStateModule State { get; private set; } = new AutomatStateModule();

			public AutomatModule() {
				this.Name = "Seq.Automat";

				this.SetMember("State", this.State);

				this.SetMember("<init>", new LambdaFun((scope, args) => {
					IEnumerable<IValue> flow = scope["flow"].ToSeq(scope);

					// Enumerator in CustomFlow is FlowAutomat by default,
					// so we should avoid additional wrappers to prevent problems
					// with yield operators
					if (flow is CustomFlow customFlow) {
						return customFlow.GetEnumerator() as IValue;
					}

					return new FlowAutomat(flow.GetEnumerator(), scope);
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("flow")
				}
				});

				this.SetMember("move", new LambdaFun((scope, args) => {
					IValue value = scope["automat"];

					if (value is FlowAutomat automat) {
						if (automat.MoveNext(Const.UNIT)) {
							return State.MakeInter(automat.Current);
						} else {
							return State.MakeFinal(automat.Result ?? Prelude.None);
						}
					}

					throw new Expressions.Break(null);
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("automat"),
				}
				});

				this.SetMember("moveWith", new LambdaFun((scope, args) => {
					IValue value = scope["automat"];

					if (value is FlowAutomat automat) {
						if (automat.MoveNext(scope["value"])) {
							return State.MakeInter(automat.Current);
						} else {
							return State.MakeFinal(automat.Result ?? Prelude.None);
						}
					}

					throw new Expressions.Break(null);
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("value"),
					new NamePattern("automat"),
				}
				});

				this.SetMember("throw", new LambdaFun((scope, args) => {
					IValue value = scope["automat"];

					if (value is FlowAutomat automat) {
						if (automat.Throw(scope["exception"])) {
							return State.MakeInter(automat.Current);
						} else {
							return State.MakeFinal(automat.Result ?? Prelude.None);
						}
					}

					throw new Expressions.Break(null);
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("exception"),
					new NamePattern("automat")
				}
				});

				this.SetMember("fromSeq", new LambdaFun((scope, args) => {
					IValue x = scope["stream"];

					if (x is Flow s && s.InternalValue is CustomFlow lgn) {
						return lgn.GetEnumerator() as IValue;
					}

					IEnumerable<IValue> stream = x.ToSeq(scope);

					return new FlowAutomat(stream.GetEnumerator(), scope);
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
				});
			}

			internal class AutomatStateModule : Module {
				internal Constructor Inter { get; private set; }
				internal Constructor Final { get; private set; }

				public AutomatStateModule() {
					this.Name = "Flow.Automat.State";

					this.Inter = Helper.CreateConstructor($"{this.Name}.Inter", this, new List<String> { "value" }) as Constructor;
					this.Final = Helper.CreateConstructor($"{this.Name}.Final", this, new List<String> { "value" }) as Constructor;

					this.SetMember("Inter", this.Inter);
					this.SetMember("Final", this.Final);
				}

				public IValue MakeInter(IValue value) {
					Instance result = new Instance(this.Inter);
					result.Items[0] = value;
					return result;
				}

				public IValue MakeFinal(IValue value) {
					Instance result = new Instance(this.Final);
					result.Items[0] = value;
					return result;
				}
			}
		}
	}
}
