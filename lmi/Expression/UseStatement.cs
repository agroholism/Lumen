using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class UseStatement : Expression {
		private IPattern pattern;
		private Expression assignable;
		private Expression body;
		private Int32 line;
		private String file;

		public UseStatement(IPattern pattern, Expression assignable, Expression body, String file, Int32 line) {
			this.pattern = pattern;
			this.assignable = assignable;
			this.body = body;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			Value context = this.assignable.Eval(scope);

			Fun onEnter = context.Type.GetMember("onEnter", scope).ToFunction(scope);
			Fun onExit = context.Type.GetMember("onExit", scope).ToFunction(scope);

			Value value = onEnter.Run(new Scope(scope), context);

			MatchResult matchResult = this.pattern.Match(value, scope);
			if (!matchResult.Success) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line, file) {
					Note = matchResult.Note
				};
			}

			Value bodyEvaluationResult = Const.UNIT;

			Value raisedException = Prelude.None;
			Value useEvaluationResult;
			try {
				bodyEvaluationResult = this.body.Eval(scope);
			}
			catch (LumenException lumenException) {
				raisedException = Helper.CreateSome(lumenException);;
			}
			catch (Exception exception) {
				raisedException = Helper.CreateSome(new LumenException(exception.Message));
			}
			finally {
				List<Value> arguments = new List<Value> {
					value,
					bodyEvaluationResult,
					raisedException
				};

				useEvaluationResult = onExit.Run(new Scope(scope), context, new List(arguments));
			}

			return useEvaluationResult;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value context = this.assignable.Eval(scope);
			Fun onEnter = context.Type.GetMember("onEnter", scope).ToFunction(scope);
			Fun onExit = context.Type.GetMember("onExit", scope).ToFunction(scope);

			Value value = onEnter.Run(new Scope(scope), context);

			MatchResult matchResult = this.pattern.Match(value, scope);
			if (!matchResult.Success) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line, file) {
					Note = matchResult.Note
				};
			}

			IEnumerable<Value> result = this.body.EvalWithYield(scope);
			foreach (Value i in result) {
				yield return i;
			}

			onExit.Run(new Scope(scope), context, value);
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			return new UseStatement(this.pattern.Closure(manager2) as IPattern,
				this.assignable.Closure(manager), this.body.Closure(manager2), this.file, this.line);
		}
	}
}