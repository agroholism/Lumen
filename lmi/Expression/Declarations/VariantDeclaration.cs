using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class VariantDeclaration : Expression {
		private readonly String typeName;
		private readonly List<Expression> members;
		private readonly List<ConstructorMetadata> constructors;
		private readonly List<Expression> implementsExpressions;

		public VariantDeclaration(String typeName, List<ConstructorMetadata> constructors,  List<Expression> members, List<Expression> implementsExpressions) {
			this.typeName = typeName;
			this.constructors = constructors;
			this.members = members;
			this.implementsExpressions = implementsExpressions;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.typeName);
			manager.Declare(this.constructors.Select(i => i.Name));

			return new VariantDeclaration(this.typeName, this.constructors, this.members.Select(i => i.Closure(manager)).ToList(), this.implementsExpressions.Select(i => i.Closure(manager)).ToList());
		}

		public IValue Eval(Scope scope) {
			Module createdType = new Module(this.typeName);

			foreach (ConstructorMetadata i in this.constructors) {
				Dictionary<String, List<IType>> dict = new();

				foreach (var item in i.Parameters) {
					dict.Add(item.Key, item.Value.Select(i => i.Eval(scope) as IType).ToList());
				}

				IType constructor = Helper.CreateConstructor(i.Name, createdType, dict);

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

			foreach (KeyValuePair<String, IValue> i in helperScope.variables) {
				createdType.SetMember(i.Key, i.Value);
			}

			foreach (String privateName in helperScope.Privates) {
				createdType.DeclarePrivate(privateName);
			}

			foreach (Expression deriving in this.implementsExpressions) {
				Class typeClass = deriving.Eval(scope) as Class;

				createdType.AppendImplementation(typeClass);
			}

			return Const.UNIT;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public override String ToString() {
			String result = $"type {this.typeName} = {String.Join(" | ", this.constructors)}";

			if (this.implementsExpressions.Count != 0 || this.members.Count != 0) {
				result += Environment.NewLine;

				foreach (Expression i in this.implementsExpressions) {
					result += "\tinclude " + i + Environment.NewLine;
				}

				if (this.implementsExpressions.Count > 0) {
					result += "\t" + Environment.NewLine;
				}

				foreach (Expression i in this.members) {
					String[] inner = i.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
					result += $"\t{String.Join(Environment.NewLine + "\t", inner)}{Environment.NewLine}\t{Environment.NewLine}";
				}
			}

			return result;
		}
	}
}