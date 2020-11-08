using System;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class AnyModule : Module {
		internal AnyModule() {
			this.Name = "_";

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
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("is", new LambdaFun((e, args) => {
				Value first = e["x"];
				IType second = e["y"] as IType;

				return new Logical(second.IsParentOf(first));
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});

			this.SetMember(Constants.EQUALS, new LambdaFun((e, args) => {
				Value first = e["x"];
				Value second = e["y"];

				return new Logical(first.Equals(second));
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});

			this.SetMember(Constants.NOT_EQUALS, new LambdaFun((e, args) => {
				Value first = e["x"];
				Value second = e["y"];

				return new Logical(!first.Equals(second));
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});
		}
	}
}
