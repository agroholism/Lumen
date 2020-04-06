using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	internal class TypeClassDeclaration : Expression {
        private String name;
        private String parameter;
        private List<Expression> members;

        public TypeClassDeclaration(String name, String parameter, List<Expression> members) {
            this.name = name;
            this.parameter = parameter;
            this.members = members;
        }
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
            return this;
        }

        public Value Eval(Scope e) {
			Module result = new Module {
                Name = name
            };

            Scope x = new Scope(e);

            foreach (Expression expression in this.members) {
                expression.Eval(x);
            }

            foreach (KeyValuePair<String, Value> i in x.variables) {
                    result.SetMember(i.Key, i.Value);
            }

            e.Bind(name, result);

            return Const.UNIT;
        }
    }
}