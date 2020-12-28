using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class OptionModule : Module {
		public Constructor Some { get; private set; }
		public IType None { get; private set; }

		public OptionModule() {
			this.Name = "Option";

			this.AppendImplementation(Prelude.Functor);
			this.AppendImplementation(Prelude.Applicative);
			this.AppendImplementation(Prelude.Default);

			this.Some = Helper.CreateConstructor("Option.Some", this, new List<String> { "x" }) as Constructor;
			this.None = Helper.CreateConstructor("Option.None", this, new List<String>());

			this.SetMember("Some", this.Some);
			this.SetMember("None", this.None);

			this.SetMember("pure", this.Some);

			this.SetMember("default", new LambdaFun((scope, args) => {
				return this.None;
			}) {
				Parameters = new List<IPattern> { }
			});

			LambdaFun fmap = new LambdaFun((scope, args) => {
				Value functor = scope["fc"];

				if (functor == this.None) {
					return this.None;
				}
				else if (this.Some.IsParentOf(functor)) {
					Fun mapper = scope["fn"].ToFunction(scope);
					return Helper.CreateSome(mapper.Call(new Scope(scope), Prelude.DeconstructSome(functor)));
				}

				throw new LumenException(Exceptions.TYPE_ERROR.F(this, functor.Type));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			};

			this.SetMember("map", fmap);

			// Applicative
			this.SetMember("lift", new LambdaFun((scope, args) => {
				Value obj = scope["f"];

				if (obj == this.None) {
					return this.None;
				}
				else if (this.Some.IsParentOf(obj)) {
					return scope["m"].CallMethodFlip("map", scope, Prelude.DeconstructSome(obj));
				}

				throw new LumenException("liftA option");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});

			this.SetMember("bind", new LambdaFun((scope, args) => {
				IType obj = scope["m"] as IType;

				if (obj == this.None) {
					return this.None;
				}
				else if (this.Some.IsParentOf(obj)) {
					Fun f = scope["f"] as Fun;
					return f.Call(new Scope(scope), Prelude.DeconstructSome(obj));
				}

				throw new LumenException("fmap option");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("f"),
				}
			});

			this.SetMember("String", new LambdaFun((scope, args) => {
				IType obj = scope["this"] as IType;
				if (this.Some.IsParentOf(obj)) {
					return new Text($"Some {obj.GetMember("x", scope)}");
				}
				else {
					return new Text("None");
				}

			}) {
				Parameters = new List<IPattern> {
					new NamePattern("this")
				}
			});
		}
	}
}