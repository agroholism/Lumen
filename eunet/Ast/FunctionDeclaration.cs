using System;
using System.Collections.Generic;
using System.Linq;
using Argent.Xenon.Runtime;

namespace Argent.Xenon.Ast {
	internal class FunctionDeclaration : Expression {
		private String functionName;
		private Dictionary<String, Expression> args;
		private Expression blockE;
		private List<Expression> specifiers;

		public FunctionDeclaration(String functionName, Dictionary<String, Expression> args, Expression blockE) {
			this.functionName = functionName;
			this.args = args;
			this.blockE = blockE;
		}

		public FunctionDeclaration(String functionName, Dictionary<String, Expression> args, Expression blockE, List<Expression> specifiers) : this(functionName, args, blockE) {
			this.specifiers = specifiers;
		}
		// TODO
		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.functionName);

			ClosureManager man2 = manager.Clone();

			Dictionary<String, Expression> newArgs = new Dictionary<String, Expression>();
			foreach (KeyValuePair<String, Expression> i in this.args) {
				newArgs[i.Key] = i.Value.Closure(manager);
				man2.Declare(i.Key);
			}

			return new FunctionDeclaration(this.functionName, newArgs, this.blockE.Closure(man2), this.specifiers);
		}

		public XnObject Eval(Scope scope) {
			ClosureManager manager = new ClosureManager(scope);

			Dictionary<String, KsTypeable> args = new Dictionary<String, KsTypeable>();
			foreach (KeyValuePair<String, Expression> i in this.args) {
				manager.Declare(i.Key);
				args[i.Key] = i.Value.Eval(scope) as KsTypeable;
			}

			Expression body = this.blockE.Closure(manager);

			Function fun = new UserFunction {
				arguments = args,
				body = body
			};

			if (manager.HasYield) {
				fun = new SystemFunction((s, a) => {
					return new Sequence(body.EvalWithYield(s));
				}) {
					arguments = args
				};
			}

			XnObject origin = Nil.NilIns;

			if (scope.IsExists(this.functionName)) {
				origin = scope.Get(this.functionName);
			}
			else {
				scope.DeclareVariable(this.functionName, XnStd.FunctionType);
			}

			foreach (XnObject i in this.specifiers.Select(i => i.Eval(scope))) {
				if (i is Function decorator) {
					fun = decorator.Run(new Scope(), fun, origin) as Function;
				}
			}

			scope.Assign(this.functionName, fun);

			return fun;

		}

		public IEnumerable<XnObject> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
	}
}