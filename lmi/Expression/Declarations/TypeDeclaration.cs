using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class VariantDeclaration : Expression {
		private readonly String name;
		private readonly List<Expression> members;
		private readonly List<ConstructorMetadata> constructors;
		private readonly List<Expression> derivings;

		public VariantDeclaration(String name, List<ConstructorMetadata> constructors,  List<Expression> members, List<Expression> derivings) {
			this.name = name;
			this.constructors = constructors;
			this.members = members;
			this.derivings = derivings;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.name);
			manager.Declare(this.constructors.Select(i => i.Name));

			return new VariantDeclaration(this.name, this.constructors, this.members.Select(i => i.Closure(manager)).ToList(), this.derivings.Select(i => i.Closure(manager)).ToList());
		}

		public Value Eval(Scope scope) {
			Module createdType = new Module(this.name);

			foreach (ConstructorMetadata i in this.constructors) {
				IType constructor = Helper.CreateConstructor(i.Name, createdType, i.Parameters);

				createdType.SetMember(i.Name, constructor);
				scope.Bind(i.Name, constructor);
			}

			if (this.constructors.Count > 1 || !this.constructors.Any(i => i.Name == this.name)) {
				scope.Bind(this.name, createdType);
			}

			Scope helperScope = new Scope(scope);

			foreach (Expression expression in this.members) {
				expression.Eval(helperScope);
			}

			foreach (KeyValuePair<String, Value> i in helperScope.variables) {
				createdType.SetMember(i.Key, i.Value);
			}

			foreach (Expression deriving in this.derivings) {
				Module typeClass = deriving.Eval(scope) as Module;
				createdType.AppendImplementation(typeClass);
			}

			return Const.UNIT;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public override String ToString() {
			String result = $"type {this.name} = {String.Join(" | ", this.constructors)}";

			if (this.derivings.Count != 0 || this.members.Count != 0) {
				result += Environment.NewLine;

				foreach (Expression i in this.derivings) {
					result += "\tinclude " + i + Environment.NewLine;
				}

				if (this.derivings.Count > 0) {
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