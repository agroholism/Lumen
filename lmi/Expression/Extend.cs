using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang;
using Lumen.Lang.Expressions;

namespace Lumen.Lmi {
	internal class Extend : Expression {
		private Expression extendableExpression;
		private List<Expression> members;
		private List<Expression> derivings;
		private String file;
		private Int32 line;

		public Extend(Expression extendableExpression, List<Expression> members, List<Expression> derivings, String file, Int32 line) {
			this.extendableExpression = extendableExpression;
			this.members = members;
			this.derivings = derivings;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			Scope innerScope = new Scope(scope);

			foreach (Expression expression in this.members) {
				expression.Eval(innerScope);
			}

			Value extendable = this.extendableExpression.Eval(scope);

			if(extendable is Module module) {
				foreach (KeyValuePair<String, Value> i in innerScope.variables) {
					if(module.Contains(i.Key)) {
						Value value = module.GetMember(i.Key, scope);
						if(value is Fun function) {
							DispatcherFunction disp = new DispatcherFunction(function.Name, function, i.Value as Fun);
							module.SetMember(i.Key, disp);
							continue;
						}
						throw new LumenException(Exceptions.IDENTIFIER_IS_ALREADY_EXISTS_IN_MODULE.F(i.Key, module.Name), line: this.line, fileName: this.file);
					} 
					module.SetMember(i.Key, i.Value);
				}
			} else {
				throw new LumenException(Exceptions.CAN_NOT_EXTEND_VALUE_OF_TYPE.F(extendable.Type), line: this.line, fileName: this.file);
			}
			
			return Const.UNIT;
		}

		public Expression Closure(ClosureManager manager) {
			return new Extend(this.extendableExpression.Closure(manager), this.members.Select(i => i.Closure(manager)).ToList(), this.derivings.Select(i => i.Closure(manager)).ToList(), this.file, this.line);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public override String ToString() {
			String result = $"extend {this.extendableExpression} where{Environment.NewLine}";

			foreach (Expression i in this.members) {
				String[] inner = i.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				result += $"\t{String.Join(Environment.NewLine + "\t", inner)}{Environment.NewLine}";
			}

			return result;
		}
	}
}