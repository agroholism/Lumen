using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class ExceptionHandling : Expression {
		private Expression tryBody;
		private Dictionary<IPattern, Expression> exceptBodies;
		private Expression ensureBody;

		public ExceptionHandling(Expression tryBody, Dictionary<IPattern, Expression> exceptBodies, Expression finallyBody) {
			this.tryBody = tryBody;
			this.exceptBodies = exceptBodies;
			this.ensureBody = finallyBody;
		}

		public IValue Eval(Scope scope) {
			IValue expressionResult = null;

RETRY_OUTER:
			Boolean isRetry = false;

			try {
				expressionResult = this.tryBody.Eval(scope);
			}
			catch (Exception e) when (e is Return || e is Break || e is Next) {
				throw;
			}
			catch (Exception ex) {
				IValue raisedException = ex as LumenException ?? new LumenException(ex.Message);

				scope[Constants.LAST_EXCEPTION_SPECIAL_NAME] = raisedException;
RETRY:
				if (isRetry) {
					goto RETRY_OUTER;
				}

				try {
					foreach (KeyValuePair<IPattern, Expression> i in this.exceptBodies) {
						if (i.Key.Match(raisedException, scope).IsSuccess) {
							return i.Value.Eval(scope);
						}
					}
				}
				catch (Retry) {
					isRetry = true;
					goto RETRY;
				}

				throw;
			}
			finally {
				if (!isRetry) {
					this.ensureBody?.Eval(scope);
				}
			}

			return expressionResult;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IEnumerator<IValue> enumerator = this.tryBody.EvalWithYield(scope).GetEnumerator();

			GeneratorExpressionTerminalResult tryExceptRescueResult = null;

			Exception exception = null;
			Boolean canNext = true;
			while (true) {
				IValue value;

				try {
					if (!canNext) {
						break;
					}

					canNext = enumerator.MoveNext();
					value = enumerator.Current;
				}
				catch (Exception e) when (e is Return || e is Break || e is Next) {
					throw;
				}
				catch (Exception ex) {
					exception = ex;
					break;
				}

				if (value is GeneratorExpressionTerminalResult terminalResult) {
					tryExceptRescueResult = terminalResult;
				}
				else {
					yield return value;
				}
			}

			if (exception != null) {
				IValue raisedException = exception as LumenException ?? new LumenException(exception.Message);

				foreach (KeyValuePair<IPattern, Expression> exceptBody in this.exceptBodies) {
					if (exceptBody.Key.Match(raisedException, scope).IsSuccess) {
						foreach (IValue evaluationResult in exceptBody.Value.EvalWithYield(scope)) {
							if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
								tryExceptRescueResult = terminalResult;
							}
							else {
								yield return evaluationResult;
							}
						}
						break;
					}
				}
			}

			if (this.ensureBody != null) {
				foreach (IValue evaluationResult in this.ensureBody.EvalWithYield(scope)) {
					yield return evaluationResult;
				}
			}

			yield return tryExceptRescueResult;
		}

		public Expression Closure(ClosureManager manager) {
			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			foreach (KeyValuePair<IPattern, Expression> i in this.exceptBodies) {
				ClosureManager manager2 = manager.Clone();

				IPattern ip = i.Key.Closure(manager2) as IPattern;

				patterns.Add(ip, i.Value.Closure(manager2));

				if (manager2.HasYield) {
					manager.HasYield = true;
				}
			}

			return new ExceptionHandling(this.tryBody.Closure(manager), patterns, this.ensureBody?.Closure(manager));
		}
	}
}