using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Light {
    internal class TypeClassDeclaration : Expression {
        private System.String name;
        private System.String parameter;
        private List<Expression> members;

        public TypeClassDeclaration(System.String name, System.String parameter, List<Expression> members) {
            this.name = name;
            this.parameter = parameter;
            this.members = members;
        }

        public Expression Closure(List<System.String> visible, Scope scope) {
            return this;
        }

        public Value Eval(Scope e) {
            TypeClass result = new TypeClass {
                name = name
            };

            result.TypeParameter = parameter;

            Scope x = new Scope(e);

            foreach (Expression expression in this.members) {
                expression.Eval(x);
            }

            foreach (KeyValuePair<String, Value> i in x.variables) {
                if(i.Value is UserFun uf && uf.Body == null) {
                    result.Requirements.Add(uf);
                } else {
                    result.SetField(i.Key, i.Value);
                }
            }

            e.Bind(name, result);

            return Const.UNIT;
        }
    }
}