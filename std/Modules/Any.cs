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
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember(Constants.EQUALS, new LambdaFun((e, args) => {
				Value first = e["x"];
				Value second = e["y"];

				return new Logical(first.Equals(second));
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});

			this.SetMember(Constants.NOT_EQL, new LambdaFun((e, args) => {
				Value first = e["x"];
				Value second = e["y"];

				return new Logical(!first.Equals(second));
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
				}
			});
		}
	}

	internal class UnitModule : Module {
		internal UnitModule() {
			this.Name = "Unit";

			this.SetMember("toText", new LambdaFun((e, args) => {
				return new Text(e["this"].ToString());
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("this")
				}
			});
		}
	}
}
