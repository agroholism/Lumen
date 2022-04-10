﻿using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class OrdModule : SystemClass {
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
				IValue x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=
			this.SetMember(Constants.LESS_EQUALS, new LambdaFun((scope, args) => {
				IValue x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) <= 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >
			this.SetMember(Constants.GT, new LambdaFun((scope, args) => {
				IValue x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) > 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// >=
			this.SetMember(Constants.GREATER_EQUALS, new LambdaFun((scope, args) => {
				IValue x = scope["x"];

				return new Logical(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) >= 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			// <=>
			this.SetMember(Constants.SHIP, new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				return new Number(x.CallMethod("compare", scope, scope["y"]).ToDouble(scope));
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("min", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];

				return x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0 ? x : y;
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("max", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];

				return x.CallMethod("compare", scope, scope["y"]).ToDouble(scope) < 0 ? y : x;
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y")
				}
			});

			this.SetMember("between", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];
				IValue z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Call(new Scope(scope), y, x).ToDouble(scope) <= 0
				&& comparator.Call(scope, x, z).ToDouble(scope) <= 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenExIn", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];
				IValue z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Call(new Scope(scope), y, x).ToDouble(scope) < 0
				&& comparator.Call(scope, x, z).ToDouble(scope) <= 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenInEx", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];
				IValue z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Call(new Scope(scope), y, x).ToDouble(scope) <= 0
				&& comparator.Call(scope, x, z).ToDouble(scope) < 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});

			this.SetMember("betweenExEx", new LambdaFun((scope, args) => {
				IValue x = scope["x"];
				IValue y = scope["y"];
				IValue z = scope["z"];

				Fun comparator = x.Type.GetMember("compare", scope).ToFunction(scope);

				return new Logical(comparator.Call(new Scope(scope), y, x).ToDouble(scope) < 0
				&& comparator.Call(scope, x, z).ToDouble(scope) < 0);
			}) {
				Parameters = new System.Collections.Generic.List<IPattern> {
					new NamePattern("x"),
					new NamePattern("y"),
					new NamePattern("z")
				}
			});
		}
	}
}
