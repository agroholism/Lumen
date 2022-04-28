﻿using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class ForCycle : Expression {
		internal String cycleName;
		internal IPattern pattern;
		internal Expression expression;
		internal Expression body;

		public ForCycle(String cycleName, IPattern pattern, Expression expression, Expression body) {
			this.cycleName = cycleName;
			this.pattern = pattern;
			this.body = body;
			this.expression = expression;
		}

		public IValue Eval(Scope scope) {
			IValue container = this.expression.Eval(scope);

			List<String> declared = this.pattern.GetDeclaredVariables();
			foreach (IValue i in container.ToSeq(scope)) {
REDO:
				MatchResult matchResult = this.pattern.Match(i, scope);
				if (!matchResult.IsSuccess) {
					throw new LumenException(matchResult.Note);
				}

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
				catch (Next nextException) {
					if (nextException.IsMatch(this.cycleName)) {
						continue;
					}

					throw nextException;
				}
				finally {
					foreach (String declaration in declared) {
						scope.Remove(declaration);
					}
				}
			}

			return Const.UNIT;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerable<IValue> containerEvaluationResults = this.expression.EvalWithYield(scope);

			IValue container = Const.UNIT;
			foreach (IValue evaluationResult in containerEvaluationResults) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					container = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			IValue returnResult = null;

			foreach (IValue i in container.ToSeq(scope)) {
REDO:
				MatchResult matchResult = this.pattern.Match(i, scope);
				if (!matchResult.IsSuccess) {
					throw new LumenException(matchResult.Note);
				}

				IEnumerable<IValue> iterationEvaluationResults = null;

				try {
					iterationEvaluationResults = this.body.EvalWithYield(scope);
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
				catch (Next nextException) {
					if (nextException.IsMatch(this.cycleName)) {
						continue;
					}

					throw nextException;
				}

				if (iterationEvaluationResults != null) {
					foreach (IValue it in iterationEvaluationResults) {
						if (it is GeneratorExpressionTerminalResult) {
							break;
						}

						yield return it;
					}
				}
			}

			if (returnResult != null) {
				yield return new GeneratorExpressionTerminalResult(returnResult);
			}
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();

			manager2.Declare(this.pattern.GetDeclaredVariables());

			ForCycle res = new ForCycle(this.cycleName, this.pattern, this.expression.Closure(manager), this.body.Closure(manager2));

			if (!manager.HasYield) {
				manager.HasYield = manager2.HasYield;
			}

			return res;
		}

		/*public override String ToString() {
            return $"for {this.pattern} in {this.expression}: {Utils.Bodify(this.body)}";
        }*/
	}
}