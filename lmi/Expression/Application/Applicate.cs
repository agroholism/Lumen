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
				String currentFunctionName = null;

				if (scope.ExistsInThisScope("rec") && scope["rec"] is Fun currentFunction) {
					currentFunctionName = currentFunction.Name;
				}

				lex.AddToCallStack(currentFunctionName, this.fileName, this.lineNumber);

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

				return this.ProcessCall(innerScope, function, arguments);
			}
			catch (TailRecursion.Tailrec tailrec) {
TAIL_RECURSION:
				try {
					return this.ProcessCall(innerScope, function, tailrec.newArguments);
				}
				catch (TailRecursion.Tailrec _tailrec) {
					tailrec = _tailrec;
					goto TAIL_RECURSION;
				}
			}
		}

		private Value ProcessCall(Scope scope, Fun function, Value[] args) {
			try {
				scope["rec"] = function;

				return function.Run(scope, args);
			}
			catch (LumenException lex) {
				String currentFunctionName = null;

				if (scope.ExistsInThisScope("rec") && scope["rec"] is Fun currentFunction) {
					currentFunctionName = currentFunction.Name;
				}

				lex.SetDataIfAbsent(currentFunctionName, this.fileName, this.lineNumber);

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
				if (result is GeneratorTerminalResult cgv) {
					callableValue = cgv.Value;
				}
				else {
					yield return result;
				}
			}

			if (callableValue is SingletonConstructor) {
				yield return new GeneratorTerminalResult(callableValue);
				yield break;
			}

			IEnumerable<Value> results;

			try {
				results = this.CallFunctionWithYield(callableValue.ToFunction(scope), scope);
			}
			catch (LumenException lex) {
				String currentFunctionName = null;

				if (scope.ExistsInThisScope("rec") && scope["rec"] is Fun currentFunction) {
					currentFunctionName = currentFunction.Name;
				}

				lex.AddToCallStack(currentFunctionName, this.fileName, this.lineNumber);

				throw;
			}

			foreach (Value result in results) {
				yield return result;
			}
		}

		private IEnumerable<Value> CallFunctionWithYield(Fun function, Scope e) {
			if (this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_")) {
				yield return new GeneratorTerminalResult(this.MakePartial(function, e));
			}

			Scope innerScope = new Scope(e);

			List<Value> args = new List<Value>();
			foreach (Expression i in this.argumentsExpression) {
				foreach (Value j in i.EvalWithYield(e)) {
					switch (j) {
						case GeneratorTerminalResult cgv:
							args.Add(cgv.Value);
							break;
						default:
							yield return j;
							break;
					}
				}
			}

			yield return new GeneratorTerminalResult(this.ProcessCall(innerScope, function, args.ToArray()));
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.callableExpression.Closure(manager), this.argumentsExpression.Select(i => i.Closure(manager)).ToList(), this.lineNumber, this.fileName);
		}

		public override String ToString() {
			return this.callableExpression.ToString() + " " + Utils.ArgumentsToString(this.argumentsExpression);
		}
	}
}