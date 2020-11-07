using Lumen.Lang.Expressions;
using Lumen.Lang;
using String = System.String;
using System.Collections.Generic;

namespace Lumen.Lmi {
	internal class WhileCycle : Expression {
		internal String cycleName;
		internal Expression condition;
		internal Expression body;

		internal WhileCycle(String cycleName, Expression condition, Expression body) {
			this.cycleName = cycleName;
			this.condition = condition;
			this.body = body;
		}

		public Value Eval(Scope scope) {
			while (this.condition.Eval(scope).ToBoolean()) {
REDO:
				try {
					this.body.Eval(scope);
				}
				catch (Break breakException) {
					if (breakException.IsMatch(this.cycleName)) {
						break;
					}

					throw breakException;
				}
				catch (Redo redoException) {
					if (redoException.IsMatch(this.cycleName)) {
						goto REDO;
					}

					throw redoException;
				}
				catch (Next) {
					continue;
				}
			}

			return Const.UNIT;
		}

		public Expression Closure(ClosureManager manager) {
			return new WhileCycle(this.cycleName, this.condition.Closure(manager), this.body.Closure(manager));
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			while (this.condition.Eval(scope).ToBoolean()) {
REDO:
				IEnumerable<Value> y = null;
				try {
					y = this.body.EvalWithYield(scope);
				}
				catch (Break breakException) {
					if (breakException.IsMatch(this.cycleName)) {
						break;
					}

					throw breakException;
				}
				catch (Next) {
					continue;
				}
				catch (Redo redoException) {
					if (redoException.IsMatch(this.cycleName)) {
						goto REDO;
					}

					throw redoException;
				}

				if (y != null) {
					foreach (Value it in y) {
						if (it is GeneratorExpressionTerminalResult) {
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