using System;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
    internal class AnyModule : Module {
        internal AnyModule() {
            this.name = "_";

            this.Parent = null;

            this.SetField("String", new LambdaFun((e, args) => {
                if(e.This is SingletonConstructor sc) {
                    return new Text(sc.ToString(e));
                }

                String result = "(" + e.This.Type.ToString(e) + " " 
                    + String.Join<Value>(" ", (e.This as Instance).items) + ")";
                return new Text(result);
            }) {
                Arguments = new System.Collections.Generic.List<IPattern> {
                    new NamePattern("this")
                }
            });

            this.SetField(Op.EQUALS, new LambdaFun((e, args) => {
                Value first = e["x"];
                Value second = e["y"];

                return new Bool(first.Equals(second));
            }) {
                Arguments = new System.Collections.Generic.List<IPattern> {
                    new NamePattern("x"),
                    new NamePattern("y"),
                }
            });

            this.SetField(Op.NOT_EQL, new LambdaFun((e, args) => {
                Value first = e["x"];
                Value second = e["y"];

                return new Bool(!first.Equals(second));
            }) {
                Arguments = new System.Collections.Generic.List<IPattern> {
                    new NamePattern("x"),
                    new NamePattern("y"),
                }
            });
        }
    }
}
