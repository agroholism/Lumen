using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	/// <summary>
	/// That's Ord mixin. Used for sorting, compharsion etc
	/// </summary>
	/// module prelude where
	///		...
	///		/// Require let compare x y 
	///		@mixin module Ord
	///			@requirement let compare x y
	///			...
	internal class OrdModule : Module {
		internal OrdModule() {
			this.Name = "Ord";

			/*this.Members["[requirements]"] = new Array(new LambdaFun(null) {
				Name = "compare",
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});*/

			// let < x y = x.compare y < 0
			this.SetMember(Constants.LT, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=
			this.SetMember(Constants.LESS_EQUALS, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) <= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >
			this.SetMember(Constants.GT, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) > 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >=
			this.SetMember(Constants.GREATER_EQUALS, new LambdaFun((scope, args) => {
				Value x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) >= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=>
			this.SetMember(Constants.SHIP, new LambdaFun((scope, args) => {
				Value x = scope["x"];
				return new Number(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope));
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("min", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0 ? x : y;
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("max", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];

				return x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0 ? y : x;
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("between", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];
				Value z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Run(new Scope(scope), y, x).ToDouble(scope) <= 0
				&& comparator.Run(scope, x, z).ToDouble(scope) <= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenExIn", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];
				Value z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Run(new Scope(scope), y, x).ToDouble(scope) < 0
				&& comparator.Run(scope, x, z).ToDouble(scope) <= 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenInEx", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];
				Value z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Run(new Scope(scope), y, x).ToDouble(scope) <= 0
				&& comparator.Run(scope, x, z).ToDouble(scope) < 0);
			}) {
				Arguments = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenExEx", new LambdaFun((scope, args) => {
				Value x = scope["x"];
				Value y = scope["y"];
				Value z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Run(new Scope(scope), y, x).ToDouble(scope) < 0
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
