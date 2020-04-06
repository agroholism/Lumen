using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ClassDeclaration : Expression {
		private String typeName;
		private List<Expression> members;

		public ClassDeclaration(String typeName, List<Expression> members) {
			this.typeName = typeName;
			this.members = members;
		}

		public Value Eval(Scope scope) {
			Scope moduleScope = new Scope(scope);

			foreach (Expression expression in this.members) {
				expression.Eval(moduleScope);
			}

			Module moduleValue = new Module(this.typeName);

			foreach (KeyValuePair<String, Value> i in moduleScope.variables) {
				moduleValue.SetMember(i.Key, i.Value);
			}

			// Add rename

			scope[this.typeName] = moduleValue;

			return Const.UNIT;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.typeName);

			ClosureManager newManager = manager.Clone();

			return new ModuleDeclaration(this.typeName, this.members.Select(i => i.Closure(newManager)).ToList());
		}

		public override String ToString() {
			String result = $"type {this.typeName} = class{Environment.NewLine}";

			foreach (Expression i in this.members) {
				String[] inner = i.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				result += $"\t{String.Join(Environment.NewLine + "\t", inner)}{Environment.NewLine}";
			}

			return result;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new CurrGeenVal(this.Eval(scope));
		}
	}
}