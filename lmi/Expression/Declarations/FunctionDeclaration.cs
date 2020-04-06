using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	public class FunctionDeclaration : Expression {
		internal String name;
		internal List<IPattern> arguments;
		internal Expression body;
		private Int32 lineNumber;
		private String fileName;

		public FunctionDeclaration(String name, List<IPattern> arguments, Expression body, Int32 lineNumber, String fileName) {
			this.name = name;
			this.arguments = arguments;
			this.body = body;
			this.lineNumber = lineNumber;
			this.fileName = fileName;
		}

		public Value Eval(Scope scope) {
			ClosureManager manager = new ClosureManager(scope);

			// Make closure of arguments patterns
			// Because here can be (x: Number), (Pair x y), etc. 
			List<IPattern> closuredPatterns =
				this.arguments.Select(argument => argument.Closure(manager) as IPattern).ToList();

			Expression closuredBody = this.body?.Closure(manager);

			Fun result = null;

			// If it's true - this is generator
			if (manager.HasYield) {
				if (manager.HasTailRecursion) {
					throw new LumenException("function can not have tail recursion and yield at the same time");
				}

				result = new LambdaFun((scope1, args) => new Stream(new LumenGenerator {
					generatorBody = closuredBody,
					AssociatedScope = scope1
				})) {
					Arguments = closuredPatterns,
					Name = this.name
				};
			}

			// If it's not generator - just create a regular function
			result ??=
				new UserFun(closuredPatterns, closuredBody, this.name);

			// Multi dispatching
			if (scope.ExistsInThisScope(this.name)) {
				Value value = scope[this.name];

				if (value is Dispatcher dispatcher) {
					// Dispatcher is already exists
					dispatcher.Append(result);
				}
				else if (value is Fun f) {
					// Exists just function - we should make a dispatcher
					scope.Bind(this.name, new Dispatcher(this.name, f, result));
				}
				else {
					// Ii is not a function - we can not define function
					throw new LumenException(Exceptions.IDENTIFIER_IS_ALREADY_EXISTS_IN_MODULE.F(this.name, "<main>"));
				}

				return result;
			}

			// Binding with this name does not exists yet - create it
			scope.Bind(this.name, result);

			return result;
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new CurrGeenVal(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.name);

			// We should create a new manager because function shoudn't see herself name
			ClosureManager newManager = manager.Clone();

			return new FunctionDeclaration(this.name, this.arguments.Select(i => i.Closure(newManager) as IPattern).ToList(), this.body.Closure(newManager), this.lineNumber, this.fileName);
		}

		public override String ToString() {
			return $"let {this.name} {String.Join(" ", this.arguments)} {(this.body == null ? "" : "=" + Environment.NewLine)} {Utils.Bodify(this.body)}";
		}
	}
}