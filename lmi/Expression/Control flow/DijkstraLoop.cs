using System;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class DijkstraLoop : Expression {
		private String cycleName;
		List<(Expression, Expression)> guardsAndBodies;

		public DijkstraLoop(String cycleName, List<(Expression, Expression)> guardsAndBodies) {
			this.cycleName = cycleName;
			this.guardsAndBodies = guardsAndBodies;
		}

		public Value Eval(Scope scope) {
			Boolean atLeastOneGuardIsTrue = true;
			while (atLeastOneGuardIsTrue) {
REDO:
				try {
					atLeastOneGuardIsTrue = false;
					foreach ((Expression guard, Expression body) in this.guardsAndBodies) {
						if (guard.Eval(scope).ToBoolean()) {
							atLeastOneGuardIsTrue = true;
							body.Eval(scope);
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
				catch (Next nextException) {
					if (nextException.IsMatch(this.cycleName)) {
						continue;
					}

					throw nextException;
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
					foreach ((Expression guard, Expression body) in this.guardsAndBodies) {
						if (guard.Eval(scope).ToBoolean()) {
							atLeastOneGuardIsTrue = true;
							yieldedValues = body.EvalWithYield(scope);
							break;
						}
					}

					if (!atLeastOneGuardIsTrue) {
						break;
					}
				}
				catch (Break breakException) {
					if (breakException.IsMatch(this.cycleName)) {
						break;
					}

					throw breakException;
				}
				catch (Next nextException) {
					if (nextException.IsMatch(this.cycleName)) {
						continue;
					}

					throw nextException;
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
			List<(Expression, Expression)> closured = new List<(Expression, Expression)>();
			List<ClosureManager> managers = new List<ClosureManager>();
			foreach ((Expression guard, Expression body) in this.guardsAndBodies) {
				ClosureManager currentManager = manager.Clone();

				Expression closuredGuard = guard.Closure(currentManager);
				Expression closuredBody = body.Closure(currentManager);
				closured.Add((closuredGuard, closuredBody));

				managers.Add(currentManager);
			}

			foreach(ClosureManager localManager in managers) {
				manager.Assimilate(localManager);
			}

			return new DijkstraLoop(this.cycleName, closured);
		}
	}
}