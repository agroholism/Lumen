using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class WhileMatch : Expression {
		private String cycleName;
		private IPattern pattern;
		private Expression assinableExpression;
		private Expression body;

		public WhileMatch(String cycleName, IPattern pattern, Expression assinableExpression, Expression body) {
			this.cycleName = cycleName;
			this.pattern = pattern;
			this.assinableExpression = assinableExpression;
			this.body = body;
		}

		public Value Eval(Scope scope) {
			Value value = this.assinableExpression.Eval(scope);

			while (this.pattern.Match(value, scope).Success) {
				REDO:
				try {
					this.body.Eval(scope);
					value = this.assinableExpression.Eval(scope);
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
			}

			return Const.UNIT;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value value = this.assinableExpression.Eval(scope);
	
			while (this.pattern.Match(value, scope).Success) {
				IEnumerable<Value> yieldedValues;
REDO:
				try {
					yieldedValues = this.body.EvalWithYield(scope);
					value = this.assinableExpression.Eval(scope);
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

				if (yieldedValues != null) {
					foreach (Value yieldedValue in yieldedValues) {
						if (yieldedValue is GeneratorTerminalResult) {
							continue;
						}

						yield return yieldedValue;
					}
				}
			}
		}

		public Expression Closure(ClosureManager manager) {
			return new WhileMatch(this.cycleName, this.pattern.Closure(manager) as IPattern,
				this.assinableExpression.Closure(manager), this.body.Closure(manager));
		}
	}
}