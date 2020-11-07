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

			if (!callableValue.TryConvertToFunction(out Fun function)) {
				LumenException invalidOperation =
					Helper.InvalidOperation($"can not call value of type {callableValue.Type}");
				invalidOperation.SetLastCallDataIfAbsent(fileName: this.fileName, lineNumber: this.lineNumber);
				throw invalidOperation;
			}

			// Partial application
			if (this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_")) {
				return this.MakePartialFunction(function, scope);
			}

			Value[] parameters = this.argumentsExpression.Select(i => i.Eval(scope)).ToArray();

tail_recursion_entry:
			try {
				Value functionCallResult = 
					function.Call(new Scope(scope) { ["rec"] = function }, parameters);

				// Returning TailRecursion means tailrec call
				if (functionCallResult is TailRecursion tailRecursion) {
					parameters = tailRecursion.newArguments;
					goto tail_recursion_entry;
				}

				return functionCallResult;
			}
			catch (LumenException lumenException) {
				// This exception handling only needed to add information to call stack.
				lumenException.SetLastCallDataIfAbsent(function.Name, null, -1);
				lumenException.AddToCallStack(null, this.fileName, this.lineNumber);
				throw;
			}
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			Value callableValue = Const.UNIT;

			foreach (Value result in this.callableExpression.EvalWithYield(scope)) {
				if (result is GeneratorExpressionTerminalResult generatorExpressionTerminalResult) {
					callableValue = generatorExpressionTerminalResult.Value;
					break;
				}
				else {
					yield return result;
				}
			}

			if (callableValue is SingletonConstructor) {
				yield return new GeneratorExpressionTerminalResult(callableValue);
				yield break;
			}

			IEnumerable<Value> functionCallResults;

			try {
				functionCallResults = this.CallFunctionInGenerator(callableValue.ToFunction(scope), scope);
			}
			catch (LumenException lumenException) {
				String currentFunctionName = null;

				if (scope.TryGetFromThisScope("rec", out Value rec) && rec.TryConvertToFunction(out Fun currentFunction)) {
					currentFunctionName = currentFunction.Name;
				}

				lumenException.SetLastCallDataIfAbsent(currentFunctionName, this.fileName, this.lineNumber);

				throw;
			}

			foreach (Value result in functionCallResults) {
				yield return result;
			}
		}

		private IEnumerable<Value> CallFunctionInGenerator(Fun function, Scope scope) {
			// Partial application
			if (this.argumentsExpression.Any(i => i is IdExpression id && id.id == "_")) {
				yield return new GeneratorExpressionTerminalResult(this.MakePartialFunction(function, scope));
			}

			Scope functionScope = new Scope(scope) {
				["rec"] = function
			};

			List<Value> arguments = new List<Value>();
			foreach (Expression argumentExpression in this.argumentsExpression) {
				foreach (Value argumentEvaluationResult in argumentExpression.EvalWithYield(scope)) {
					if (argumentEvaluationResult is GeneratorExpressionTerminalResult terminalResult) {
						arguments.Add(terminalResult.Value);
						break;
					}
					else {
						yield return argumentEvaluationResult;
					}
				}
			}

			Value[] argumentsArray = arguments.ToArray();

			Value functionCallResult = null;

			try {
				functionCallResult = function.Call(functionScope, argumentsArray);
			}
			catch (LumenException lex) {
				lex.AddToCallStack(null, this.fileName, this.lineNumber);
				throw;
			}

			yield return new GeneratorExpressionTerminalResult(functionCallResult);
		}

		private Value MakePartialFunction(Fun targetFunction, Scope parent) {
			List<IPattern> arguments = new List<IPattern>();

			List<Expression> newParameters = new List<Expression>();
			Int32 x = 0;
			foreach (Expression exp in this.argumentsExpression) {
				if (!(exp is IdExpression ide) || ide.id != "_") {
					newParameters.Add(new ValueLiteral(exp.Eval(parent)));
					continue;
				}

				String parameterName = $"<apply-part-arg-{x}>";
				arguments.Add(new NamePattern(parameterName));
				newParameters.Add(new IdExpression(parameterName, ide.file, ide.line));
				x++;
			}

			return new UserFun(arguments, new Applicate(new ValueLiteral(targetFunction), newParameters, this.lineNumber, this.fileName)) {
				Name = $"<apply-part-from-{targetFunction.Name ?? "<anonym-func>"}>"
			};
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.callableExpression.Closure(manager), this.argumentsExpression.Select(i => i.Closure(manager)).ToList(), this.lineNumber, this.fileName);
		}

		public override String ToString() {
			return this.callableExpression.ToString() + " " + Utils.ArgumentsToString(this.argumentsExpression);
		}
	}
}