using System;

namespace Lumen.Lang {
    internal sealed class BooleanModule : Module {
        internal BooleanModule() {
            this.Name = "prelude.Boolean";

			#region operators

			this.SetMember(Op.NOT, new LambdaFun((scope, args) => {
                Boolean value = scope["this"].ToBoolean();

                return new Bool(!value);
            }) {
                Arguments = Const.Self
            });

            this.SetMember(Op.OR, new LambdaFun((scope, args) => {
                Boolean value = scope["this"].ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value || other);
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.XOR, new LambdaFun((scope, args) => {
                Boolean value = scope["this"].ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value ^ other);
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember(Op.AND, new LambdaFun((scope, args) => {
                Boolean value = scope["this"].ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value && other);
            }) {
                Arguments = Const.SelfOther
            });

            this.SetMember("compare", new LambdaFun((scope, args) => {
                Value other = scope["other"];

                return (Number)scope["this"].CompareTo(other);
            }) {
                Arguments = Const.SelfOther
            });
            #endregion

            this.SetMember("String", new LambdaFun((e, args) => new Text(e["this"].ToString())) {
                Arguments = Const.Self
            });
            this.SetMember("Number", new LambdaFun((e, args) => (Number)(Converter.ToBoolean(e["this"]) ? 1 : 0)) {
                Arguments = Const.Self
            });

            this.IncludeMixin(Prelude.Ord);
        }
    }
}
