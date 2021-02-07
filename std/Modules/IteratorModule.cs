using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class IteratorModule : Type {
		public IteratorModule() : base("Iterator") {
			this.SetMember("<init>", new LambdaFun((scope, args) => {
				Value x = scope["stream"];

				if (x is Seq s && s.InternalValue is LumenGenerator lgn) {
					return lgn.GetEnumerator() as Value;
				}

				IEnumerable<Value> stream = x.ToSeq(scope);

				return new LumenIterator(stream.GetEnumerator(), scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("moveNext", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is LumenIterator generator) {
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

				if (iter is LumenIterator generator) {
					var res = generator.Current;
					if (res is GeneratorExpressionTerminalResult terminalResult) {
						return terminalResult.Value;
					}
					return res;
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter")
				}
			});

			this.SetMember("getNext", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is LumenIterator generator) {
					var res = generator.Next();
					if (res is GeneratorExpressionTerminalResult terminalResult) {
						throw new Expressions.Return(terminalResult.Value);
					}
					return res;
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter")
				}
			});

			this.SetMember("!", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is LumenIterator generator) {
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

				if (x is LumenIterator generator) {
					Value res = generator.Send(scope["value"]);
					if(res is GeneratorExpressionTerminalResult terminalResult) {
						throw new Expressions.Return(terminalResult.Value);
					}
					return res;
				}

				throw new Expressions.Break(null);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter"),
					new NamePattern("value")
				}
			});

			this.SetMember("throw", new LambdaFun((scope, args) => {
				Value x = scope["iterator"];

				if (x is LumenIterator generator) {
					Value res = generator.Throw(scope["ex"]);
					if (res is GeneratorExpressionTerminalResult terminalResult) {//?
						throw new Expressions.Return(terminalResult.Value);
					}
					return res;
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

				if (x is LumenIterator generator) {
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

				if (x is Seq s && s.InternalValue is LumenGenerator lgn) {
					return lgn.GetEnumerator() as Value;
				}

				IEnumerable<Value> stream = x.ToSeq(scope);

				return new LumenIterator(stream.GetEnumerator(), scope);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});
		}
	}
}
