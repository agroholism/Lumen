using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal sealed class FunctionModule : Module {
		internal FunctionModule() {
			this.Name = "Function";

			this.AppendImplementation(Prelude.Functor);
			this.AppendImplementation(Prelude.Applicative);

			LambdaFun CombainToLeft = new LambdaFun((e, args) => {
				Fun m = Converter.ToFunction(e["fc"], e);
				Fun f = Converter.ToFunction(e["fn"], e);

				return new LambdaFun((scope, arguments) => 
					f.Call(new Scope(scope), m.Call(new Scope(scope), arguments))) { Parameters = m.Parameters };
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			};

			this.SetMember(Constants.PLUS, new LambdaFun((e, args) => {
				Fun m = Converter.ToFunction(e["fc"], e);
				Fun f = Converter.ToFunction(e["fn"], e);

				return new LambdaFun((scope, arguments) => {
					m.Call(new Scope(scope), arguments);
					return f.Call(new Scope(scope), arguments);
				}) {
					Parameters = m.Parameters
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("fc"),
				}
			});

			this.SetMember(Constants.STAR, new LambdaFun((e, args) => {
				Fun m = Converter.ToFunction(e["fn"], e);
				Int32 f = e["n"].ToInt(e);

				return new LambdaFun((scope, arguments) => {
					Value result = m.Call(scope, arguments);

					for (Int32 i = 1; i < f; i++) {
						result = m.Call(scope, result);
					}

					return result;
				}) {
					Parameters = m.Parameters
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("n"),
				}
			});

			// let fmap f m = fmap (m >> f)
			this.SetMember("fmap", CombainToLeft);

			// Applicative
			this.SetMember("liftA", new LambdaFun((scope, args) => {
				Fun obj = scope["f"].ToFunction(scope);
				Fun obj2 = scope["m"].ToFunction(scope);

				return new LambdaFun((e, a) => {
					Value al = e["x'"];

					Fun z = obj2.Call(new Scope(), al).ToFunction(e);

					return z.Call(new Scope(), obj.Call(new Scope(), al));
				}) {
					Parameters = new List<IPattern> {
					new NamePattern("x'")
				}
				};
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("f"),
					new NamePattern("m"),
				}
			});

			// add >>
			// add <<
			// add +=
			// add -=
			// add +
			// add -
			// add .curry
			// add .combine
		}
	}
}
