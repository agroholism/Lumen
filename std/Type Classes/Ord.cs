using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class OrdModule : TypeClass {
		internal OrdModule() {
			this.name = "prelude.Ord";

			this.Requirements = new System.Collections.Generic.List<Fun> {
				new LambdaFun(null) {
					Name = "compare",
					Arguments = new System.Collections.Generic.List<IPattern> {
						new NamePattern("x"),
						new NamePattern("y")
					}
				}
			};

			// <
			this.SetField(Op.LT, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Bool((x.Type.GetField("compare", scope).ToFunction(scope)).Run(scope, x, scope["y"]).ToDouble(scope) < 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=
			this.SetField(Op.LTEQ, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Bool((x.Type.GetField("compare", scope).ToFunction(scope)).Run(scope, x, scope["y"]).ToDouble(scope) <= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >
			this.SetField(Op.GT, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Bool((x.Type.GetField("compare", scope).ToFunction(scope)).Run(scope, x, scope["y"]).ToDouble(scope) > 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >=
			this.SetField(Op.GTEQ, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Bool((x.Type.GetField("compare", scope).ToFunction(scope)).Run(scope, x, scope["y"]).ToDouble(scope) >= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=>
			this.SetField(Op.SHIP, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Number((x.Type.GetField("compare", scope).ToFunction(scope)).Run(scope, x, scope["y"]).ToDouble(scope));
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetField("min", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return x.Type.GetField("compare", scope).ToFunction(scope).Run(scope, x, y).ToDouble(scope) < 0 ? x : y;
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetField("max", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return x.Type.GetField("compare", scope).ToFunction(scope).Run(scope, x, y).ToDouble(scope) < 0 ? y : x;
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetField("between", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];
				Value z = scope["z"];

				Fun comparator = x.Type.GetField("compare", scope).ToFunction(scope);

				return new Bool(comparator.Run(scope, y, x).ToDouble(scope) < 0 
				&& comparator.Run(scope, x, z).ToDouble(scope) < 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});
		}
	}
}
