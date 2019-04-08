using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {

    internal class Monad : TypeClass {
        internal Monad() {
            this.name = "prelude.Monad";
            this.TypeParameter = "'T";
            this.Requirements = new List<Fun> {
                new LambdaFun(null) {
                    Arguments = new List<IPattern> {
                        new NamePattern("f"),
                        new NamePattern("m")
                    },
                    Name = "liftB"
                }
            };

            this.SetField(">>=", new LambdaFun((scope, args) => {
                return (scope["m"].Type.GetField("liftB", scope) as Fun).Run(scope, scope["m"], scope["f"]);
            }) {
                Arguments = new List<IPattern> {
					new NamePattern("m"),
                    new NamePattern("f")
                },
            });
        }
    }
}
