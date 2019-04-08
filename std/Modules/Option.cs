using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;

namespace Lumen.Lang {
    internal class Option : Module {
        public Constructor Some { get; private set; }
        public IObject None { get; private set; }

        public Option() {
            this.name = "prelude.Option";

            this.Some = Helper.CreateConstructor("prelude.Option.Some", this, new[] { "x" }) as Constructor;
            this.None = Helper.CreateConstructor("prelude.Option.None", this, new String[0]);

            this.Parent = Prelude.Any;

            this.SetField("Some", this.Some);
            this.SetField("None", this.None);

            LambdaFun fmap = new LambdaFun((scope, args) => {
                IObject obj = scope["fc"] as IObject;

                if (obj == this.None) {
                    return this.None;
                } else if (this.Some.IsParentOf(obj)) {
                    Fun f = scope["fn"] as Fun;
                    return Helper.CreateSome(f.Run(new Scope(scope), Prelude.DeconstructSome(obj, scope)));
                }

                throw new LumenException("fmap option");
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            };

            this.SetField("fmap", fmap);

            // Applicative
            this.SetField("liftA", new LambdaFun((scope, args) => {
                IObject obj = scope["f"] as IObject;

                if (obj == this.None) {
                    return this.None;
                } else if (this.Some.IsParentOf(obj)) {
                    return fmap.Run(new Scope(scope), Prelude.DeconstructSome(obj, scope), scope["m"]);
                }

                throw new LumenException("liftA option");
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("m"),
                    new NamePattern("f"),
                }
            });

            this.SetField("liftB", new LambdaFun((scope, args) => {
                IObject obj = scope["m"] as IObject;

                if (obj == this.None) {
                    return this.None;
                } else if (this.Some.IsParentOf(obj)) {
                    Fun f = scope["f"] as Fun;
                    return f.Run(new Scope(scope), Prelude.DeconstructSome(obj, scope));
                }

                throw new LumenException("fmap option");
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("m"),
                    new NamePattern("f"),
                }
            });

            this.SetField("String", new LambdaFun((scope, args) => {
                IObject obj = scope["this"] as IObject;
                if (this.Some.IsParentOf(obj)) {
                    return new Text($"Some {obj.GetField("x", scope)}");
                } else {
                    return new Text("None");
                }

            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            });

            this.Derive(Prelude.Functor);
            this.Derive(Prelude.Applicative);
            this.Derive(Prelude.Monad);
        }
    }
}
