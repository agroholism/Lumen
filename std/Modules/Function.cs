using Lumen.Lang.Expressions;
using System.Collections.Generic;

namespace Lumen.Lang {
    internal sealed class Function : Module {
        internal Function() {
            this.name = "prelude.Function";

            LambdaFun RCombine = new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["m"], e);
                Fun f = Converter.ToFunction(e["f"], e);

                return new LambdaFun((scope, arguments) => m.Run(new Scope(scope), f.Run(new Scope(scope), arguments)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("m"),
                    new NamePattern("f"),
                }
            };

            LambdaFun LCombine = new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["fc"], e);
                Fun f = Converter.ToFunction(e["fn"], e);

                return new LambdaFun((scope, arguments) => f.Run(new Scope(scope), m.Run(new Scope(scope), arguments)));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            };

            this.SetField(Op.PLUS, new LambdaFun((e, args) => {
                Fun m = Converter.ToFunction(e["fc"], e);
                Fun f = Converter.ToFunction(e["fn"], e);

                return new LambdaFun((scope, arguments) => {
                    m.Run(new Scope(scope), arguments);
                    return f.Run(new Scope(scope), arguments);
                });
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            });

            this.SetField(Op.LSH, LCombine);

            this.SetField(Op.RSH, RCombine);

            // let fmap f m = fmap (m >> f)
            this.SetField("fmap", LCombine);

            // Applicative
            this.SetField("liftA", new LambdaFun((scope, args) => {
                Fun obj = scope["f"] as Fun;
                Fun obj2 = scope["m"] as Fun;

                return new LambdaFun((e, a) => obj.Run(e, a[0], obj2.Run(e, a[0])));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("m"),
                }
            });

            this.Derive(Prelude.Functor);
            this.Derive(Prelude.Applicative);
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
