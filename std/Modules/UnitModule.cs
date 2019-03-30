using System.Collections.Generic;
using System;
using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal sealed class RVoid : Module {
        internal RVoid() {
            this.name = "void";

            this.SetField(Op.EQUALS, new LambdaFun((e, args) => new Bool(args[0] is Void)));
        }
    }

    internal sealed class DateTimeModule : Module {
        internal DateTimeModule() {
            this.name = "prelude.DateTime";

            this.SetField("now", new LambdaFun((e, args) => {
                return new Text(DateTime.Now.ToLongTimeString());
            }) {
                Arguments = new List<IPattern> {
                    
                }
            });
        }
    }
}
