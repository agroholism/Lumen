using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Applicate : Expression {
		private Expression callableExpression;
		private List<Expression> argumentsExpression;
		private Int32 lineNumber;
		private String fileName;

		public Applicate(Expression callableExpression, List<Expression> argumentsExpression, Int32 lineNumber, String fileName) {
			this.callableExpression = callableExpression;
			this.argumentsExpression = argumentsExpression;
			this.lineNumber = lineNumber;
			this.fileName = fileName;
		}

		public Value Eval(Scope scope) {
			Value callableValue = this.callableExpression.Eval(scope);

			if (callableValue is SingletonConstructor) {
				return callableValue;
			}

			try {
				return this.CallFunction(callableValue.ToFunction(scope), scope);
			}
			catch (LumenException lex) {
				lex.file ??= this.fileName;
				lex.line = lex.line == -1 ? this.lineNumber : lex.line;

				if (lex.functionName == null && scope.ExistsInThisScope("rec") && scope["rec"] is Fun fun1) {
					lex.functionName = fun1.Name;
				}

				throw;
			}
		}

		private Value CallFunction(Fun function, Scope e) {
			if (this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_")) {
				return this.MakePartial(function, e);
			}

			Scope innerScope = new Scope(e);

			try {
				Value[] arguments = this.argumentsExpression.Select(i => i.Eval(e)).ToArray();

				return this.ProcessCall(e, innerScope, function, arguments);
			}
			catch (TailRecursion.Tailrec tailrec) {
TAIL_RECURSION:
				try {
					return this.ProcessCall(e, innerScope, function, tailrec.newArguments);
				}
				catch (TailRecursion.Tailrec _tailrec) {
					tailrec = _tailrec;
					goto TAIL_RECURSION;
				}
			}
		}

		private Value ProcessCall(Scope parent, Scope scope, Fun function, Value[] args) {
			try {
				scope["rec"] = function;

				return function.Run(scope, args);
			}
			catch (LumenException lex) {
				lex.file ??= this.fileName;
				lex.line = lex.line == -1 ? this.lineNumber : lex.line;

				if (lex.functionName == null && scope.ExistsInThisScope("rec") && scope["rec"] is Fun fun) {
					lex.functionName = fun.Name;
				}
				else {
					if (parent.ExistsInThisScope("rec") && scope["rec"] is Fun fun1) {
						lex.AddToCallStack(fun1.Name, this.fileName, this.lineNumber);
					}
					else {
						lex.AddToCallStack(null, this.fileName, this.lineNumber);
					}
				}

				throw;
			}
		}

		private Value MakePartial(Fun function, Scope parent) {
			List<IPattern> arguments = new List<IPattern>();

			List<Expression> expressions = new List<Expression>();
			Int32 x = 0;
			foreach (Expression exp in this.argumentsExpression) {
				if (!(exp is IdExpression ide) || ide.id != "_") {
					expressions.Add(new ValueLiteral(exp.Eval(parent)));
					continue;
				}

				String parameterName = $"<apply-part-arg-{x}>";
				arguments.Add(new NamePattern(parameterName));
				expressions.Add(new IdExpression(parameterName, ide.line, ide.file));
				x++;
			}

			return new UserFun(arguments, new Applicate(new ValueLiteral(function), expressions, this.lineNumber, this.fileName)) {
				Name = $"<apply-part-from-{function.Name ?? "<anonym-func>"}>"
			};
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value callableValue = Const.UNIT;
			foreach (Value result in this.callableExpression.EvalWithYield(scope)) {
				if (result is CurrGeenVal cgv) {
					callableValue = cgv.Value;
				}
				else {
					yield return result;
				}
			}

			if (callableValue is SingletonConstructor) {
				yield return new CurrGeenVal(callableValue);
				yield break;
			}

			IEnumerable<Value> results;

			try {
				results = this.CallFunctionWithYield(callableValue.ToFunction(scope), scope);
			}
			catch (LumenException lex) {
				lex.file ??= this.fileName;
				lex.line = lex.line == -1 ? this.lineNumber : lex.line;

				if (lex.functionName == null && scope.ExistsInThisScope("rec") && scope["rec"] is Fun fun1) {
					lex.functionName = fun1.Name;
				}

				throw;
			}

			foreach (Value result in results) {
				yield return result;
			}
		}

		private IEnumerable<Value> CallFunctionWithYield(Fun function, Scope e) {
			if (this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_")) {
				yield return new CurrGeenVal(this.MakePartial(function, e));
			}

			Scope innerScope = new Scope(e);

			List<Value> args = new List<Value>();
			foreach (Expression i in this.argumentsExpression) {
				foreach (Value j in i.EvalWithYield(e)) {
					switch (j) {
						case CurrGeenVal cgv:
							args.Add(cgv.Value);
							break;
						default:
							yield return j;
							break;
					}
				}
			}

			yield return new CurrGeenVal(this.ProcessCall(e, innerScope, function, args.ToArray()));
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.callableExpression.Closure(manager), this.argumentsExpression.Select(i => i.Closure(manager)).ToList(), this.lineNumber, this.fileName);
		}

		public override String ToString() {
			return this.callableExpression.ToString() + " " + Utils.ArgumentsToString(this.argumentsExpression);
		}
	}
}