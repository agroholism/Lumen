using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class AutomatModule : Module {
		public AutomatModule() {
			this.Name = "Automat";

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				IEnumerable<Value> flow = scope["flow"].ToFlow(scope);

				// Enumerator in CustomFlow is FlowAutomat by default,
				// so we should avoid additional wrappers to prevent problems
				// with yield operators
				if (flow is CustomFlow customFlow) {
					return customFlow.GetEnumerator() as Value;
				}

				return new FlowAutomat(flow.GetEnumerator(), scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("flow")
				}
			});

			this.SetMember("move", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is FlowAutomat generator) {
					return new Logical(generator.MoveNext(scope["val"]));
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("val"),
					new NamePattern("iter")
				}
			});

			this.SetMember("getCurrent", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is FlowAutomat generator) {
					return generator.Current;
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter")
				}
			});

			this.SetMember("getResult", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is FlowAutomat generator) {
					return generator.AutomatResult;
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter")
				}
			});

			this.SetMember("getNext", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is FlowAutomat generator) {
					return generator.Next();
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter")
				}
			});



			this.SetMember("!", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is FlowAutomat generator) {
					return generator.Next();
				}

				return iter;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter"),
					new NamePattern("_")
				}
			});

			this.SetMember("send", new LambdaFun((scope, args) => {
				Value x = scope["iter"];

				if (x is FlowAutomat generator) {
					return generator.Send(scope["value"]);
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("value"),
					new NamePattern("iter"),
				}
			});

			this.SetMember("throw", new LambdaFun((scope, args) => {
				Value x = scope["iterator"];

				if (x is FlowAutomat generator) {
					return generator.Throw(scope["ex"]);
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("ex"),
					new NamePattern("iterator")
				}
			});

			this.SetMember("<-", new LambdaFun((scope, args) => {
				Value x = scope["iter"];

				if (x is FlowAutomat generator) {
					return generator.Send(scope["value"]);
				}

				return x;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter"),
					new NamePattern("value")
				}
			});

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				Value x = scope["stream"];

				if (x is Flow s && s.InternalValue is CustomFlow lgn) {
					return lgn.GetEnumerator() as Value;
				}

				IEnumerable<Value> stream = x.ToFlow(scope);

				return new FlowAutomat(stream.GetEnumerator(), scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});
		}
	}
}
