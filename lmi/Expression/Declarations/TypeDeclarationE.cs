using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using System.Collections.Generic;

namespace Lumen.Light {
    internal class TypeDeclarationE : Expression {
        private String name;
        List<Expression> members;
        private Dictionary<String, List<String>> constructors;

        public TypeDeclarationE(String name, Dictionary<String, List<String>> conStringuctors, List<Expression> members) {
            this.name = name;
            this.constructors = conStringuctors;
            this.members = members;
        }

        public Expression Closure(List<String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            Module mainType = new Module(this.name);

            foreach (KeyValuePair<String, List<String>> i in this.constructors) {
                IObject constructor = Helper.CreateConstructor(i.Key, mainType, i.Value);

                mainType.SetField(i.Key, constructor);
                e.Bind(i.Key, constructor);
            }

            e.Bind(this.name, mainType);

            Scope x = new Scope(e);

            foreach (Expression expression in this.members) {
                expression.Eval(x);
            }

            foreach (KeyValuePair<String, Value> i in x.variables) {
                mainType.SetField(i.Key, i.Value);
            }

            mainType.Parent = Prelude.Any;

            return Const.UNIT;
        }
    }
}