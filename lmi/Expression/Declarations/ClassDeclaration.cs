using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ClassDeclaration : Expression {
		private String className;
		private String typeParam;
		private List<Expression> membersExpressions;
		private List<Expression> derivingExpressions;

		public ClassDeclaration(String className, String typeParam, List<Expression> members, List<Expression> derivings) {
			this.className = className;
			this.typeParam = typeParam;
			this.membersExpressions = members;
			this.derivingExpressions = derivings;
		}

		public IValue Eval(Scope scope) {
			Class moduleValue = new Class(this.className, this.typeParam ?? "_");

			Scope moduleScope = new Scope(scope) {
				[this.className] = moduleValue
			};

			foreach(Expression i in this.membersExpressions) {
				ClosureManager cm = new (moduleScope);
				cm.Declare(this.typeParam);
				moduleValue.Declarations.Add(i.Closure(cm));
			}

			foreach (Expression derivingExpression in this.derivingExpressions) {
				Class typeClass = derivingExpression.Eval(scope) as Class;

				if (typeClass == moduleValue) {
					throw new LumenException("implementing itself");
				}

				moduleValue.AppendImplementation(typeClass);
			}

			scope[this.className] = moduleValue;

			return Const.UNIT;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			// yields are not support in class body so we ignore them
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager newManager = manager.Clone();
			newManager.Declare(this.className);

			ClassDeclaration result = new ClassDeclaration(this.className, this.typeParam, this.membersExpressions.Select(i => i.Closure(newManager)).ToList(), this.derivingExpressions.Select(i => i.Closure(manager)).ToList());
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