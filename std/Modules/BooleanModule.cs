using System;

namespace Lumen.Lang {
    internal sealed class BooleanModule : Module {
        internal BooleanModule() {
            this.name = "prelude.Boolean";

            #region operators

            this.SetField(Op.NOT, new LambdaFun((scope, args) => {
                Boolean value = scope.This.ToBoolean();

                return new Bool(!value);
            }) {
                Arguments = Const.This
            });

            this.SetField(Op.OR, new LambdaFun((scope, args) => {
                Boolean value = scope.This.ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value || other);
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.XOR, new LambdaFun((scope, args) => {
                Boolean value = scope.This.ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value ^ other);
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.AND, new LambdaFun((scope, args) => {
                Boolean value = scope.This.ToBoolean();
                Boolean other = scope["other"].ToBoolean();

                return new Bool(value && other);
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField(Op.SHIP, new LambdaFun((scope, args) => {
                Value other = scope["other"];

                return (Number)scope.This.CompareTo(other);
            }) {
                Arguments = Const.ThisOther
            });
            #endregion

            this.SetField("String", new LambdaFun((e, args) => new Text(e.This.ToString(e))) {
                Arguments = Const.This
            });
            this.SetField("Number", new LambdaFun((e, args) => (Number)(Converter.ToBoolean(e.This) ? 1 : 0)) {
                Arguments = Const.This
            });

            this.Derive(Prelude.Comparable);
        }
    }
}
