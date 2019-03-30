using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {

    internal class Functor : TypeClass {
        internal Functor() {
            this.name = "prelude.Functor";
            this.TypeParameter = "'T";
            this.Requirements = new List<Fun> {
                new LambdaFun(null) {
                    Arguments = new List<IPattern> {
                        new NamePattern("fn"),
                        new NamePattern("fc")
                    },
                    Name = "fmap"
                }
            };

            this.SetField(">>-", new LambdaFun((scope, args) => {
                return (scope["fc"].Type.GetField("fmap", scope) as Fun).Run(scope, scope["fn"], scope["fc"]);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc")
                 },
            });

            this.SetField("<$>", new LambdaFun((scope, args) => {
                return (scope["fc"].Type.GetField("fmap", scope) as Fun).Run(scope, scope["fn"], scope["fc"]);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fc"),
                    new NamePattern("fn")
                 },
            });
        }
    }
}
