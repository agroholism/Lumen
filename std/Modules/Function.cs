using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    internal sealed class Function : Module {
        internal Function() {
            this.Name = "prelude.Function";

			this.IncludeMixin(Prelude.Functor);

			LambdaFun RCombine = new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["m"], e);
                Fun f = Converter.ToFunction(e["f"], e);

                return new LambdaFun((scope, arguments) => m.Run(new Scope(scope), f.Run(new Scope(scope), arguments))) {
					Arguments = f.Arguments
				};
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("m"),
                    new NamePattern("f"),
                }
            };

            LambdaFun LCombine = new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["fc"], e);
                Fun f = Converter.ToFunction(e["fn"], e);

				return new LambdaFun((scope, arguments) => f.Run(new Scope(scope), m.Run(new Scope(scope), arguments))) {
					Arguments = m.Arguments
				};
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            };

            this.SetMember(Op.PLUS, new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["fc"], e);
                Fun f = Converter.ToFunction(e["fn"], e);

                return new LambdaFun((scope, arguments) => {
					m.Run(new Scope(scope), arguments);
					return f.Run(new Scope(scope), arguments);
				}) {
					Arguments = m.Arguments
				};
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            });

			this.SetMember(Op.STAR, new LambdaFun((e, args) => {
				Fun m = Converter.ToFunction(e["fn"], e);
				Int32 f = e["n"].ToInt(e);

				return new LambdaFun((scope, arguments) => {
					Value result = m.Run(scope, arguments);

					for (Int32 i = 1; i < f; i++) {
						result = m.Run(scope, result);
					}

					return result;
				}) {
					Arguments = m.Arguments
				};
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("fn"),
					new NamePattern("n"),
				}
			});

			this.SetMember(Op.LSH, LCombine);

            this.SetMember(Op.RSH, RCombine);

            // let fmap f m = fmap (m >> f)
            this.SetMember("fmap", LCombine);

            // Applicative
            this.SetMember("liftA", new LambdaFun((scope, args) => {
                Fun obj = scope["f"] as Fun;
                Fun obj2 = scope["m"] as Fun;

                return new LambdaFun((e, a) => obj.Run(e, a[0], obj2.Run(e, a[0])));
            }) {
                Arguments = new List<IPattern> {
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
