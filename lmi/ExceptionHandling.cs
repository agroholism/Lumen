using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class ExceptionHandling : Expression {
		private Expression tryBody;
		private Dictionary<IPattern, Expression> patterns;
		private Expression finallyBody;

		public ExceptionHandling(Expression tryBody, Dictionary<IPattern, Expression> patterns, Expression finallyBody) {
			this.tryBody = tryBody;
			this.patterns = patterns;
			this.finallyBody = finallyBody;
		}

		public Expression Closure(ClosureManager manager) {
			Dictionary<IPattern, Expression> patterns = new Dictionary<IPattern, Expression>();

			foreach (KeyValuePair<IPattern, Expression> i in this.patterns) {
				ClosureManager manager2 = manager.Clone();

				IPattern ip = i.Key.Closure(manager2) as IPattern;

				patterns.Add(ip, i.Value.Closure(manager2));

				if (manager2.HasYield) {
					manager.HasYield = true;
				}
			}

			return new ExceptionHandling(this.tryBody.Closure(manager), patterns, this.finallyBody?.Closure(manager));
		}

		public Value Eval(Scope scope) {
			Value expressionResult = null;

RETRY_OUTER:
			Boolean isRetry = false;

			try {
				expressionResult = this.tryBody.Eval(scope);
			}
			catch (Exception ex) {
				Value raisedException = ex as LumenException ?? new LumenException(ex.Message);

				scope[Constants.LAST_EXCEPTION_SPECIAL_NAME] = raisedException;
RETRY:
				if (isRetry) {
					goto RETRY_OUTER;
				}

				try {
					foreach (KeyValuePair<IPattern, Expression> i in this.patterns) {
						if (i.Key.Match(raisedException, scope).Success) {
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
					this.finallyBody?.Eval(scope);
				}
			}

			return expressionResult;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			IEnumerator<Value> enumerator = this.tryBody.EvalWithYield(scope).GetEnumerator();

			GeneratorTerminalResult terminalResult = null;

			Exception exception = null;
			Boolean canNext = true;
			while (true) {
				Value val;

				try {
					if (!canNext) {
						break;
					}

					canNext = enumerator.MoveNext();
					val = enumerator.Current;

				}
				catch (Exception ex) {
					exception = ex;
					break;
				}

				if (val is GeneratorTerminalResult gtr) {
					terminalResult = gtr;
				}
				else {
					yield return val;
				}
			}

			if (exception != null) {
				Value raisedException = exception as LumenException ?? new LumenException(exception.Message);

				foreach (KeyValuePair<IPattern, Expression> i in this.patterns) {
					if (i.Key.Match(raisedException, scope).Success) {
						foreach (Value x in i.Value.EvalWithYield(scope)) {
							if (x is GeneratorTerminalResult gtr) {
								terminalResult = gtr;
							}
							else {
								yield return x;
							}
						}
						break;
					}
				}
			}

			if (this.finallyBody != null) {
				foreach (Value x in this.finallyBody.EvalWithYield(scope)) {
					yield return x;
				}
			}

			yield return terminalResult;
		}
	}
}