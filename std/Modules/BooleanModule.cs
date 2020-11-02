using System;

namespace Lumen.Lang {
    internal sealed class BooleanModule : Module {
        internal BooleanModule() {
            this.Name = "prelude.Boolean";

			#region operators

            this.SetMember("compare", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                return (Number)scope["this"].CompareTo(other);
            }) {
                Arguments = Const.SelfOther
            });

            #endregion

            this.SetMember("toText", new LambdaFun((e, args) => new Text(e["this"].ToString())) {
                Arguments = Const.Self
            });

            this.SetMember("toNumber", new LambdaFun((e, args) => (Number)(Converter.ToBoolean(e["this"]) ? 1 : 0)) {
                Arguments = Const.Self
            });

            this.SetMember("clone", new LambdaFun((scope, args) => {
                return scope["self"];
            }) {
                Arguments = Const.Self
            });

            this.IncludeMixin(Prelude.Ord);
            this.IncludeMixin(Prelude.Cloneable);
        }
    }
}
