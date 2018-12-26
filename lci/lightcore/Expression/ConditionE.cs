using System;
using System.Collections.Generic;

using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	internal class ConditionE : Expression {
		public Expression condition;
		public Expression falseExpression;
		public Expression trueExpression;

		public ConditionE(Expression condition, Expression trueExpression, Expression falseExpression) {
			this.condition = condition;
			this.trueExpression = trueExpression;
			this.falseExpression = falseExpression;
		}

		public Value Eval(Scope scope) {
			Boolean result = this.condition.Eval(scope).ToBoolean();

			return result ? this.trueExpression.Eval(scope) : this.falseExpression.Eval(scope);
		}

		public Expression Optimize(Scope scope) {
			this.condition = this.condition.Optimize(scope);

			if (this.condition is ValueE ve) {
				if (Converter.ToBoolean(ve.val)) {
					return this.trueExpression.Optimize(scope);
				}
				else {
					return this.falseExpression.Optimize(scope);
				}
			}

			return this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new ConditionE(this.condition.Closure(visible, thread), this.trueExpression.Closure(visible, thread), this.falseExpression.Closure(visible, thread));
		}

		public override String ToString() {
			String result = "if(" + this.condition.ToString() + ") " + Environment.NewLine + "\t" + this.trueExpression.ToString();
			if (this.falseExpression != null && !(this.falseExpression is UnknownExpression)) {
				result += Environment.NewLine + "else " + Environment.NewLine + this.falseExpression.ToString();
			}

			return result;
		}
	}
}