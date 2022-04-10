using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class Monoid : SystemClass {
		internal Monoid() {
			this.Name = "Monoid";

			this.SetMember("+", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Monoid.+", scope["x"].Type);
				return Const.UNIT;
			}) {
				Name = "+",
				Parameters = new List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("empty", new LambdaFun((scope, args) => {
				Prelude.FunctionIsNotImplementedForType("Monoid.empty", new Text("'T"));
				return Const.UNIT;
			}) {
				Name = "empty",
				Parameters = new List<IPattern> {
					new NamePattern("_"),
				}
			});

			this.SetMember("isEmpty", new LambdaFun((scope, args) => {
				IValue monoid = scope["monoid"];
				return new Logical(monoid.CallMethod("empty", scope).Equals(monoid));
			}) {
				Name = "isEmpty",
				Parameters = new List<IPattern> {
					new NamePattern("monoid"),
				}
			});

			this.SetMember("concat", new LambdaFun((scope, args) => {
				return scope["monoids"].ToFlow(scope).Aggregate((x, y) => 
					x.Type.GetMember("+", scope).ToFunction(scope).Call(new Scope(), x, y));
			}) {
				Name = "concat",
				Parameters = new List<IPattern> {
					new NamePattern("monoids"),
				}
			});
		}
	}
}
