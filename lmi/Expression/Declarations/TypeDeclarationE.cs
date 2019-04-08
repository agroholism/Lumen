using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Light {
    internal class TypeDeclarationE : Expression {
        private String name;
        List<Expression> members;
        private Dictionary<String, List<String>> constructors;
        List<Expression> derivings;
        List<Boolean> mutModifiers;

        public TypeDeclarationE(String name, Dictionary<String, List<String>> conStringuctors, List<Expression> members, List<Expression> derivings, List<Boolean> mutModifiers) {
            this.name = name;
            this.constructors = conStringuctors;
            this.members = members;
            this.derivings = derivings;
            this.mutModifiers = mutModifiers;
        }

        public Expression Closure(List<String> visible, Scope scope) {
			visible.Add(this.name);
			visible.AddRange(this.constructors.Select(i => i.Key));
            return new TypeDeclarationE(this.name, constructors, members.Select(i => i.Closure(visible, scope)).ToList(), this.derivings.Select(i => i.Closure(visible, scope)).ToList(), this.mutModifiers);
        }

        public Value Eval(Scope e) {
            Module mainType = new Module(this.name);

            foreach (KeyValuePair<String, List<String>> i in this.constructors) {
                IObject constructor = Helper.CreateConstructor(i.Key, mainType, i.Value);

                mainType.SetField(i.Key, constructor);
                e.Bind(i.Key, constructor);
            }

			if (this.constructors.Count > 1 || !this.constructors.ContainsKey(this.name)) {
				e.Bind(this.name, mainType);
			}

			Scope x = new Scope(e);

            foreach (Expression expression in this.members) {
                expression.Eval(x);
            }

            foreach (KeyValuePair<String, Value> i in x.variables) {
                mainType.SetField(i.Key, i.Value);
            }

            foreach(var deriving in this.derivings) {
                TypeClass typeClass = deriving.Eval(e) as TypeClass;
                if (typeClass.IsTypeImplement(mainType)) {
                    mainType.Derive(typeClass);
                }
            }

            mainType.Parent = Prelude.Any;

            return Const.UNIT;
        }
    }
}