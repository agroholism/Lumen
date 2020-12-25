using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class IteratorModule : Module {
		public IteratorModule() {
			this.Name = "Iterator";

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

			this.SetMember("getNext", new LambdaFun((scope, args) => {
				Value iter = scope["iter"];

				if (iter is LumenIterator generator) {
					return generator.Next();
				}

				return iter;
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
					return generator.Send(scope["value"]);
				}

				return x;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("iter"),
					new NamePattern("value")
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
					new NamePattern("x")
				}
			});
		}
	}
}
