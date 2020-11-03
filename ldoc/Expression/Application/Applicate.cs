using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace ldoc {
	internal class Applicate : Expression {
		internal Expression Callable { get; private set; }
		internal List<Expression> ArgumentsExpression { get; private set; }
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }

		public Applicate(Expression callable, List<Expression> argumentsExpression, Int32 line, String file) {
			this.Callable = callable;
			this.ArgumentsExpression = argumentsExpression;
			this.Line = line;
			this.File = file;
		}

		public Value Eval(Scope scope) {
			Value callable = this.Callable.Eval(scope);

			switch (callable) {
				case Fun fun:
					return this.EvalFun(fun, scope);
				case SingletonConstructor _:
					return callable;
				case Module module:
					return this.EvalFun(module.GetMember("init", scope).ToFunction(scope), scope);
				default:
					throw new LumenException(Exceptions.NOT_A_FUNCTION.F(callable.Type), null, this.Line, this.File);
			}
		}

		private Value EvalFun(Fun function, Scope e) {
			Scope innerScope = new Scope(e);

			try {
				Boolean isPartial = this.ArgumentsExpression.Any(i => i is IdExpression id && id.id == "_");

				if (isPartial) {
					return this.MakePartial(function, e);
				}

				Value[] arguments = this.ArgumentsExpression.Select(i => i.Eval(e)).ToArray();

				return this.ProcessCall(e, innerScope, function, arguments);
			}
			catch (GotoE gotoe) {
TAIL_RECURSION:
				try {
					return this.ProcessCall(e, innerScope, function, gotoe.result);
				}
				catch (GotoE gt) {
					gotoe = gt;
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
				if (lex.File == null) {
					lex.File = this.File;
				}

				if (lex.Line == -1) {
					lex.Line = this.Line;
				}

				if (lex.functionName == null) {
					if (parent.ExistsInThisScope("rec")) {
						lex.functionName = (parent["rec"] as Fun).Name;
					}
				}
				else {
					if (parent.ExistsInThisScope("rec")) {
						Fun ff = parent["rec"] as Fun;
						lex.AddToCallStack(ff.Name, this.File, this.Line);
					}
					else {
						lex.AddToCallStack(null, this.File, this.Line);
					}
				}

				throw;
			}
		}

		private Value MakePartial(Fun function, Scope parent) {
			List<IPattern> arguments = new List<IPattern>();

			List<Expression> expressions = new List<Expression>();
			Int32 x = 0;
			foreach (Expression exp in this.ArgumentsExpression) {
				if (exp is IdExpression ide && ide.id == "_") {
					arguments.Add(new NamePattern("#x" + x));
					expressions.Add(new IdExpression("#x" + x, ide.line, ide.file));
					x++;
				}
				else {
					expressions.Add(new ValueE(exp.Eval(parent)));
				}
			}

			return new UserFun(arguments, new Applicate(new ValueE(function), expressions, this.Line, this.File)) {
				Name = function.Name + "'"
			};
		}

		public Expression Closure(ClosureManager manager) {
			return new Applicate(this.Callable.Closure(manager), this.ArgumentsExpression.Select(i => i.Closure(manager)).ToList(), this.Line, this.File);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return "(" + this.Callable.ToString() + " " + String.Join(" ", this.ArgumentsExpression) + ")";
		}
	}
}