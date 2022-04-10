using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang;
using Lumen.Lang.Expressions;
using Lumen.Lang.Patterns;

namespace Lumen.Lmi {
	internal class Applicate : Expression {
		private Expression callableExpression;
		private List<Expression> argumentsExpressions;
		private Int32 lineNumber;
		private String fileName;

		public Applicate(Expression callableExpression, List<Expression> argumentsExpressions, String fileName, Int32 lineNumber) {
			this.callableExpression = callableExpression;
			this.argumentsExpressions = argumentsExpressions;
			this.lineNumber = lineNumber;
			this.fileName = fileName;
		}

		public IValue Eval(Scope scope) {
			IValue callableValue = this.callableExpression.Eval(scope);

			if (!callableValue.TryConvertToFunction(out Fun function)) {
				LumenException invalidOperation =
					Helper.InvalidOperation($"can not call value of type {callableValue.Type}");
				invalidOperation.SetLastCallDataIfAbsent(fileName: this.fileName, lineNumber: this.lineNumber);
				throw invalidOperation;
			}

			// Partial application
			if (this.argumentsExpressions.Any(i => i is IdExpression id && id.id == "_")) {
				return this.MakePartialFunction(function, scope);
			}

			IValue[] parameters = this.argumentsExpressions.Select(i => i.Eval(scope)).ToArray();

tail_recursion_entry:
			try {
				IValue functionCallResult =
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

		public IEnumerable<IValue> EvalWithYield(Scope scope) {
			IValue callableValue = null;

			foreach (IValue evaluationResult in this.callableExpression.EvalWithYield(scope)) {
				if (evaluationResult is GeneratorExpressionTerminalResult terminalResult) {
					callableValue = terminalResult.Value;
					break;
				}

				yield return evaluationResult;
			}

			if (!callableValue.TryConvertToFunction(out Fun function)) {
				LumenException invalidOperation =
					Helper.InvalidOperation($"can not call value of type {callableValue.Type}");
				invalidOperation.SetLastCallDataIfAbsent(fileName: this.fileName, lineNumber: this.lineNumber);
				throw invalidOperation;
			}

			// Partial application
			if (this.argumentsExpressions.Any(i => i is IdExpression idExpression && idExpression.id == "_")) {
				yield return new GeneratorExpressionTerminalResult(this.MakePartialFunction(function, scope));
			}

			List<IValue> arguments = new List<IValue>();
			foreach (Expression argumentExpression in this.argumentsExpressions) {
				foreach (IValue argumentEvaluationResult in argumentExpression.EvalWithYield(scope)) {
					if (argumentEvaluationResult is GeneratorExpressionTerminalResult terminalResult) {
						arguments.Add(terminalResult.Value);
						break;
					}
					
					yield return argumentEvaluationResult;
				}
			}

			IValue[] argumentsArray = arguments.ToArray();

			IValue functionCallResult = null;

			try {
				functionCallResult = function.Call(new Scope(scope) { ["rec"] = function }, argumentsArray);
			}
			catch (LumenException lumenException) {
				// This exception handling only needed to add information to call stack.
				lumenException.SetLastCallDataIfAbsent(function.Name, null, -1);
				lumenException.AddToCallStack(null, this.fileName, this.lineNumber);
				throw;
			}

			yield return new GeneratorExpressionTerminalResult(functionCallResult);
		}

		private IValue MakePartialFunction(Fun targetFunction, Scope scope) {
			List<IPattern> wrapperParameters = new List<IPattern>();

			List<Expression> newArguments = new List<Expression>();
			Int32 wrapperParameterPosition = 0;
			foreach (Expression exp in this.argumentsExpressions) {
				if (exp is not IdExpression idExpression || idExpression.id != "_") {
					newArguments.Add(new ValueLiteral(exp.Eval(scope)));
					continue;
				}

				String parameterName = $"[partial-apply-parameter-{wrapperParameterPosition}]";
				wrapperParameters.Add(new NamePattern(parameterName));
				newArguments.Add(new IdExpression(parameterName, idExpression.file, idExpression.line));
				wrapperParameterPosition++;
			}

			return new UserFun(wrapperParameters, new Applicate(new ValueLiteral(targetFunction), newArguments, this.fileName, this.lineNumber)) {
				Name = $"[partial-apply-from {targetFunction.Name}]"
			};
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.callableExpression.Closure(manager), this.argumentsExpressions.Select(i => i.Closure(manager)).ToList(), this.fileName, this.lineNumber);
		}

		public override String ToString() {
			return $"{this.callableExpression} {Utils.ArgumentsToString(this.argumentsExpressions)}";
		}
	}
}