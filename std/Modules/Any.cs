using System;
using System.Collections.Generic;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class AnyModule : Module {
		internal AnyModule() {
			this.Name = "Any";

			this.SetMember("toText", new LambdaFun((e, args) => {
				Value self = e["self"];

				if (self is SingletonConstructor sc) {
					return new Text(sc.Name);
				}

				if (self is Instance instance) {
					return new Text($"({instance.Type} {String.Join<Value>(" ", instance.Items)})");
				}

				return new Text(self.ToString());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember(Constants.EQUALS, new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return new Logical(x.Equals(y));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});

			this.SetMember(Constants.NOT_EQUALS, new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return new Logical(!x.Equals(y));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});
		}
	}
}
