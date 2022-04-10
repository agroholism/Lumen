using System;
using System.Collections.Generic;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

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

		public IValue Eval(Scope scope) {
			IValue context = this.assignable.Eval(scope);

			Fun onEnter = context.Type.GetMember("onEnter", scope).ToFunction(scope);
			Fun onExit = context.Type.GetMember("onExit", scope).ToFunction(scope);

			IValue value = onEnter.Call(new Scope(scope), context);

			MatchResult matchResult = this.pattern.Match(value, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line, file) {
					Note = matchResult.Note
				};
			}

			IValue bodyEvaluationResult = Const.UNIT;

			IValue raisedException = Prelude.None;
			IValue useEvaluationResult;
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
				List<IValue> arguments = new List<IValue> {
					value,
					bodyEvaluationResult,
					raisedException
				};

				useEvaluationResult = onExit.Call(new Scope(scope), context, new List(arguments));
			}

			return useEvaluationResult;
		}

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IValue context = this.assignable.Eval(scope);
			Fun onEnter = context.Type.GetMember("onEnter", scope).ToFunction(scope);
			Fun onExit = context.Type.GetMember("onExit", scope).ToFunction(scope);

			IValue value = onEnter.Call(new Scope(scope), context);

			MatchResult matchResult = this.pattern.Match(value, scope);
			if (!matchResult.IsSuccess) {
				throw new LumenException(Exceptions.NAME_CAN_NOT_BE_DEFINED, line, file) {
					Note = matchResult.Note
				};
			}

			IEnumerable<IValue> result = this.body.EvalWithYield(scope);
			foreach (IValue i in result) {
				yield return i;
			}

			onExit.Call(new Scope(scope), context, value);
		}

		public Expression Closure(ClosureManager manager) {
			ClosureManager manager2 = manager.Clone();
			return new UseStatement(this.pattern.Closure(manager2) as IPattern,
				this.assignable.Closure(manager), this.body.Closure(manager2), this.file, this.line);
		}
	}
}