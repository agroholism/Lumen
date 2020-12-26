using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ExceptionDeclaration : Expression {
		private String typeName;
		private List<ConstructorMetadata> constructors;
		private List<Expression> members;
		private List<Expression> derivings;

		public ExceptionDeclaration(String typeName, List<ConstructorMetadata> constructors, List<Expression> members, List<Expression> derivings) {
			this.typeName = typeName;
			this.constructors = constructors;
			this.members = members;
			this.derivings = derivings;
		}

		public Expression Closure(ClosureManager manager) {
			throw new NotImplementedException();
		}

		public Value Eval(Scope scope) {
			Module createdType = new Module(this.typeName);

			foreach (ConstructorMetadata i in this.constructors) {
				Dictionary<String, List<IType>> dict = new();
				foreach (var item in i.Parameters) {
					dict.Add(item.Key, item.Value.Select(i => i.Eval(scope) as IType).ToList());
				}

				IType constructor = new ExceptionConstructor(i.Name, createdType, dict);

				createdType.SetMember(i.Name, constructor);
				scope.Bind(i.Name, constructor);
			}

			if (this.constructors.Count > 1 || !this.constructors.Any(i => i.Name == this.typeName)) {
				scope.Bind(this.typeName, createdType);
			}

			Scope helperScope = new Scope(scope);

			foreach (Expression expression in this.members) {
				expression.Eval(helperScope);
			}

			foreach (KeyValuePair<String, Value> i in helperScope.variables) {
				createdType.SetMember(i.Key, i.Value);
			}

			foreach (Expression deriving in this.derivings) {
				Class typeClass = deriving.Eval(scope) as Class;
				createdType.AppendImplementation(typeClass);
			}

			createdType.AppendImplementation(Prelude.Exception);

			return Const.UNIT;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			throw new NotImplementedException();
		}
	}
}