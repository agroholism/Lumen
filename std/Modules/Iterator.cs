using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class Iterator : Module {
		public Iterator() {
			this.Name = "prelude.Iterator";

			this.SetMember("init", new LambdaFun((scope, args) => {
				Value x = scope["stream"];

				if (x is Stream s && s.innerValue is LumenGenerator lgn) {
					return lgn.GetEnumerator() as Value;
				}

				IEnumerable<Value> stream = x.ToStream(scope);

				return new LumenIterator(stream.GetEnumerator());
			}) {
				Arguments = new List<IPattern> {
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
				Arguments = new List<IPattern> {
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
				Arguments = new List<IPattern> {
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
				Arguments = new List<IPattern> {
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
				Arguments = new List<IPattern> {
					new NamePattern("iter"),
					new NamePattern("value")
				}
			});

			this.SetMember("toStream", new LambdaFun((e, args) => {
				return new Stream(e["this"].ToList(e));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("this")
				}
			});

			this.SetMember("fromStream", new LambdaFun((scope, args) => {
				return new LumenIterator(scope["x"].ToStream(scope).GetEnumerator());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.IncludeMixin(Prelude.Collection);
		}
	}
}
