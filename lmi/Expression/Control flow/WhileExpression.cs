using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;
using System.Collections.Generic;

namespace Lumen.Lmi {
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
                } catch (Break bs) {
                    if (bs.UseLabel() > 0) {
                        throw bs;
                    }
                    break;
                } catch (Next) {
                    continue;
                }
            }

            return Const.UNIT;
        }

        public Expression Closure(ClosureManager manager) {
            return new WhileExpression(this.condition.Closure(manager), this.body.Closure(manager));
        }

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			while (this.condition.Eval(scope).ToBoolean()) {
				IEnumerable<Value> y = null;
				try {
					y = this.body.EvalWithYield(scope);
				}
				catch (Break bs) {
					if (bs.UseLabel() > 0) {
						throw bs;
					}
					break;
				}
				catch (Next) {
					continue;
				}

				if (y != null) {
					foreach (Value it in y) {
						if (it is GeneratorTerminalResult) {
							continue;
						}
						yield return it;
					}
				}
			}
		}

		/* public override String ToString() {
			 return $"while {this.condition}: {Utils.Bodify(this.body)}";
		 }*/
	}
}