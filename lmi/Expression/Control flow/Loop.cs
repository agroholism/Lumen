using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Loop : Expression {
		internal String cycleName;
		internal Expression body;

		internal Loop(String cycleName, Expression body) {
			this.cycleName = cycleName;
			this.body = body;
		}

		public Value Eval(Scope scope) {
			while (true) {
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
			return new Loop(this.cycleName, this.body.Closure(manager));
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			while (true) {
REDO:
				IEnumerable<Value> y;
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