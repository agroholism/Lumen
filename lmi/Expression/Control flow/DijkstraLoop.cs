using System;
using System.Linq;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class DijkstraLoop : Expression {
		private String cycleName;
		List<(Expression, Expression)> guardAndBodies;

		public DijkstraLoop(String cycleName, List<(Expression, Expression)> guardAndBodies) {
			this.cycleName = cycleName;
			this.guardAndBodies = guardAndBodies;
		}

		public Value Eval(Scope scope) {
			Boolean atLeastOneGuardIsTrue = true;
			while (atLeastOneGuardIsTrue) {
				REDO:
				try {
					atLeastOneGuardIsTrue = false;
					foreach ((Expression guard, Expression body) in this.guardAndBodies) {
						if(guard.Eval(scope).ToBoolean()) {
							atLeastOneGuardIsTrue = true;
							body.Eval(scope);
							break;
						}
					}

					if(!atLeastOneGuardIsTrue) {
						break;
					}
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
			Boolean atLeastOneGuardIsTrue = true;

			while (atLeastOneGuardIsTrue) {
				IEnumerable<Value> yieldedValues = null;
REDO:
				try {
					atLeastOneGuardIsTrue = false;
					foreach ((Expression guard, Expression body) in this.guardAndBodies) {
						if (guard.Eval(scope).ToBoolean()) {
							atLeastOneGuardIsTrue = true;
							yieldedValues = body.EvalWithYield(scope);
							break;
						}
					}
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
						if (yieldedValue is GeneratorExpressionTerminalResult) {
							continue;
						}

						yield return yieldedValue;
					}
				}
			}
		}

		public Expression Closure(ClosureManager manager) {
			return new DijkstraLoop(this.cycleName, this.guardAndBodies.Select(i => 
				(i.Item1.Closure(manager), i.Item2.Closure(manager))).ToList());
		}
	}
}