using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ClassDeclaration : Expression {
		private String className;
		private List<Expression> membersExpressions;
		private List<Expression> derivingExpressions;

		public ClassDeclaration(String className, List<Expression> members, List<Expression> derivings) {
			this.className = className;
			this.membersExpressions = members;
			this.derivingExpressions = derivings;
		}

		public Value Eval(Scope scope) {
			Module moduleValue = new Module(this.className);

			Scope moduleScope = new Scope(scope) {
				[this.className] = moduleValue
			};

			foreach (Expression memberExpression in this.membersExpressions) {
				memberExpression.Eval(moduleScope);
			}

			foreach (KeyValuePair<String, Value> member in moduleScope.variables) {
				// There are class in this scope, so to avoid recursion we check
				if (member.Value != moduleValue) {
					moduleValue.SetMember(member.Key, member.Value);
				}
			}

			foreach (Expression derivingExpression in this.derivingExpressions) {
				Module typeClass = derivingExpression.Eval(scope) as Module;
				if (typeClass == moduleValue) {
					throw new LumenException("implementing itself");
				}
				moduleValue.AppendImplementation(typeClass);
			}

			scope[this.className] = moduleValue;

			return Const.UNIT;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			// yields are not support in class body so we ignore them
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager newManager = manager.Clone();
			newManager.Declare(this.className);

			ClassDeclaration result = new ClassDeclaration(this.className, this.membersExpressions.Select(i => i.Closure(newManager)).ToList(), this.derivingExpressions.Select(i => i.Closure(manager)).ToList());
			manager.Declare(this.className);
			return result;
		}

		public override String ToString() {
			String result = $"type {this.className} = class{Environment.NewLine}";

			foreach (Expression i in this.membersExpressions) {
				String[] inner = i.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				result += $"\t{String.Join(Environment.NewLine + "\t", inner)}{Environment.NewLine}";
			}

			return result;
		}
	}
}