using System;
using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace ldoc {
	internal class DotApplicate : Expression {
		internal DotExpression Callable { get; private set; }
		internal List<Expression> ArgumentsExperssions { get; private set; }
		internal Int32 Line { get; private set; }
		internal String File { get; private set; }

		public DotApplicate(DotExpression callable, List<Expression> argumentsExpression, Int32 line, String file) {
			this.Callable = callable;
			this.ArgumentsExperssions = argumentsExpression;
			this.Line = line;
			this.File = file;
		}

		public Value Eval(Scope e) {
			Value container = this.Callable.expression.Eval(e);

			Value function;

			String functionName = this.Callable.nameVariable;

			if (container is Module module) {
				if (!module.Contains(functionName)) {
					throw new LumenException(Exceptions.MODULE_DOES_NOT_CONTAINS_FUNCTION.F(functionName, module.Name));
				}

				function = module.GetMember(functionName, e);

				return new Applicate(new ValueE(function), this.ArgumentsExperssions, -1, "").Eval(e);
			}

			if (container is Constructor ctor) {
				if (ctor.TryGetMember(functionName, out var res)) {
					return new Applicate(new ValueE(res), this.ArgumentsExperssions, -1, "").Eval(e);
				}

				e.Bind("@a", ctor);

				throw new LumenException(Exceptions.MODULE_DOES_NOT_CONTAINS_FUNCTION.F(functionName, ctor.Name));
			}

			if (container is Instance iob) {
				Value v = iob.GetField(functionName, e);

				if (v is Fun fn) {
					Scope innerScope = new Scope(e) { ["self"] = fn };
					List<Value> argse = new List<Value> { container };
					argse.AddRange(this.EvalArguments(e));

					return fn.Run(innerScope, argse.ToArray());
				}

				throw new LumenException(Exceptions.NOT_A_FUNCTION.F(this.Callable));
			}

			IType cls = container.Type;

			if (!cls.TryGetMember(functionName, out Value prf)) {
				try {
					return new Applicate(this.Callable, this.ArgumentsExperssions, -1, "").Eval(e);
				}
				catch {
					throw new LumenException(Exceptions.MODULE_DOES_NOT_CONTAINS_FUNCTION.F(functionName, cls));
				}
			}

			Fun functio = cls.GetMember(functionName, e) as Fun;

			Scope innerScop = new Scope(e) { ["self"] = functio };
			/*var tc = cls.GetTypeClass(functionName);

			if (tc != null)
				innerScop.Bind(tc.TypeParameter, module);*/
			List<Value> Objects = new List<Value> { container };

			Objects.AddRange(this.EvalArguments(e));

			return functio.Run(innerScop, Objects.ToArray());
		}

		private List<Value> EvalArguments(Scope scope) {
			List<Value> arguments = new List<Value>();

			foreach (Expression i in this.ArgumentsExperssions) {
				if (i is SpreadE) {
					foreach (Value j in Converter.ToList(i.Eval(scope), scope)) {
						arguments.Add(j);
					}
				}
				else {
					arguments.Add(i.Eval(scope));
				}
			}

			return arguments;
		}

		public Expression Closure(ClosureManager manager) {
			return new DotApplicate(this.Callable.Closure(manager) as DotExpression, this.ArgumentsExperssions.Select(i => i.Closure(manager)).ToList(), this.Line, this.File);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			return this.Callable.ToString() + String.Join(" ", this.ArgumentsExperssions);
		}
	}
}