using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal class ComparableModule : TypeClass {
        internal ComparableModule() {
            this.Requirements = new System.Collections.Generic.List<Fun> {
                new LambdaFun(null) {
                    Name = "<=>",
                    Arguments = new System.Collections.Generic.List<IPattern> {
                        new NamePattern("x"),
                        new NamePattern("y")
                    }
                }
            };

            // =
            this.SetField(Op.EQUALS, new LambdaFun((scope, args) => {
                return new Bool(scope.This.Equals(scope["other"]));
            }) {
                Arguments = Const.ThisOther
            });

            // <>
            this.SetField(Op.NOT_EQL, new LambdaFun((scope, args) => {
                return new Bool(!scope.This.Equals(scope["other"]));
            }) {
                Arguments = Const.ThisOther
            });

            // <
            this.SetField(Op.LT, new LambdaFun((scope, args) => {
                return new Bool(scope.This.CompareTo(scope["other"]) < 0);
            }) {
                Arguments = Const.ThisOther
            });

            // <=
            this.SetField(Op.LTEQ, new LambdaFun((scope, args) => {
                return new Bool(scope.This.CompareTo(scope["other"]) <= 0);
            }) {
                Arguments = Const.ThisOther
            });

            // >
            this.SetField(Op.GT, new LambdaFun((scope, args) => {
                return new Bool(scope.This.CompareTo(scope["other"]) > 0);
            }) {
                Arguments = Const.ThisOther
            });

            // >=
            this.SetField(Op.GTEQ, new LambdaFun((scope, args) => {
                return new Bool(scope.This.CompareTo(scope["other"]) >= 0);
            }) {
                Arguments = Const.ThisOther
            });

            // <=>
            this.SetField("compare", new LambdaFun((scope, args) => {
                return new Number(scope.This.CompareTo(scope["other"]));
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("min", new LambdaFun((scope, args) => {
                return scope.This.CompareTo(scope["other"]) < 0 ? scope.This : scope["other"];
            }) {
                Arguments = Const.ThisOther
            });

            this.SetField("max", new LambdaFun((scope, args) => {
                return scope.This.CompareTo(scope["other"]) > 0 ? scope.This : scope["other"];
            }) {
                Arguments = Const.ThisOther
            });
        }
    }
}
