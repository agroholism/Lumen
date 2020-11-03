using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	internal class TypeDeclaration : Expression {
		public readonly String name;
		public readonly List<Expression> members;
		public readonly List<ConstructorMetadata> constructors;
		public readonly List<Expression> derivings;

		public TypeDeclaration(String name, List<ConstructorMetadata> constructors, List<Expression> members, List<Expression> derivings) {
			this.name = name;
			this.constructors = constructors;
			this.members = members;
			this.derivings = derivings;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.name);
			manager.Declare(this.constructors.Select(i => i.Name));
			return new TypeDeclaration(this.name, constructors, members.Select(i => i.Closure(manager)).ToList(), this.derivings.Select(i => i.Closure(manager)).ToList());
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Value Eval(Scope e) {
			Module mainType = new Module(this.name);

			foreach (ConstructorMetadata i in this.constructors) {
				IType constructor = Helper.CreateConstructor(
					i.Name, mainType,
					i.Parameters.Select(i => i.Name).ToList());

				mainType.SetMember(i.Name, constructor);
				e.Bind(i.Name, constructor);
			}

			if (this.constructors.Count > 1 || !this.constructors.Any(i => i.Name == this.name)) {
				e.Bind(this.name, mainType);
			}

			Scope x = new Scope(e);

			foreach (Expression expression in this.members) {
				expression.Eval(x);
			}

			foreach (KeyValuePair<String, Value> i in x.variables) {
				mainType.SetMember(i.Key, i.Value);
			}

			foreach (Expression deriving in this.derivings) {
				Module typeClass = deriving.Eval(e) as Module;
				mainType.AppendImplementation(typeClass);
			}

			return Const.UNIT;
		}
	}
}