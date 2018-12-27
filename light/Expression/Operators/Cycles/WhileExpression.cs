using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using System.Collections.Generic;

namespace Stereotype {
	internal class WhileExpression : Expression {
		internal Expression condition;
		internal Expression body;

		internal WhileExpression(Expression condition, Expression body) {
			this.condition = condition;
			this.body = body;
		}

		public Value Eval(Scope scope) {
			while (this.condition.Eval(scope).ToBoolean()) {
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

			return Const.NULL;
		}

		public Expression Optimize(Scope scope) {
			return this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new WhileExpression(this.condition.Closure(visible, thread), this.body.Closure(visible, thread));
		}

		public override String ToString() {
			return "while(" + this.condition.ToString() + ") { " + Environment.NewLine + this.body.ToString() + Environment.NewLine + "}";
		}
	}
}