using StandartLibrary.Expressions;
using System.Collections.Generic;
using System;
using StandartLibrary;
using System.Linq;

namespace Stereotype {
	internal class ForE : Expression {
		public Expression expression;
		public Expression body;
		private String varName;
		private Expression varType;
		private Boolean declaredVar;

		public ForE(String varName, Expression varType, Boolean declaredVar, Expression expressions, Expression statement) {
			this.varName = varName;
			this.varType = varType;
			this.declaredVar = declaredVar;
			this.expression = expressions;
			this.body = statement;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			List<String> bodyVariables = new List<String>();

			bodyVariables.AddRange(visible);

			bodyVariables.Add(this.varName);

			return new ForE(this.varName, this.varType?.Closure(visible, thread), this.declaredVar, this.expression.Closure(visible, thread), this.body.Closure(bodyVariables, thread));
		}

		public Value Eval(Scope scope) {
			Value container = this.expression.Eval(scope);

			Value saver = null;

			if (this.declaredVar) {
				if (scope.ExistsInThisScope(this.varName)) {
					saver = scope[this.varName];
				}

				scope.Set(this.varName, Const.NULL);

				if (this.varType != null) {
					if (scope is Scope tscope) {
						//tscope.AddAnotation(this.varName, this.varType.Eval(scope) as Isable);
					}
				}
			}

			foreach (Value i in container.ToIterator(scope)) {
				new Assigment(this.varName, new ValueE(i), 0, "").Eval(scope);

				try {
					this.body.Eval(scope);
				}
				catch (Break bs) {
					if (bs.UseLabel() > 0) {
						throw bs;
					}
					break;
				}
				catch (NextE) {
					continue;
				}
			}

			if(saver != null) {
				scope[this.varName] = saver;
			}

			return Const.NULL;
		}

		public override String ToString() {
			return "";
		}
	}
}