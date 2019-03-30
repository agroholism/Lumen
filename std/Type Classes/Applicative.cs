using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {

    internal class Applicative : TypeClass {
        internal Applicative() {
            this.name = "prelude.Applicative";
            this.TypeParameter = "'T";
            this.Requirements = new List<Fun> {
                new LambdaFun(null) {
                    Arguments = new List<IPattern> {
                        new NamePattern("f"),
                        new NamePattern("m")
                    },
                    Name = "liftA"
                }
            };

            this.SetField("<*>", new LambdaFun((scope, args) => {
                return (scope["m"].Type.GetField("liftA", scope) as Fun).Run(scope, scope["m"], scope["f"]);
            }) {
                Arguments = new List<IPattern> {
                        new NamePattern("f"),
                        new NamePattern("m")
                    },
            });
        }
    }
}
