using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	public class Block : Expression {
		public List<Expression> expressions;

		public Block() {
			this.expressions = new List<Expression>();
		}

		public Block(List<Expression> Expressions) {
			this.expressions = Expressions;
		}

		public void Add(Expression Expression) {
			this.expressions.Add(Expression);
		}

		public Value Eval(Scope e) {
			Dictionary<String, Value> savedBindings = new Dictionary<String, Value>();

			for (Int32 i = 0; i < this.expressions.Count - 1; i++) {
				if (this.expressions[i] is BindingDeclaration binding) {
					List<String> declared = binding.pattern.GetDeclaredVariables();
					foreach (String x in declared) {
						if (e.IsExistsInThisScope(x)) {
							savedBindings[x] = e[x];
							e.Remove(x);
						}
					}
				}

				this.expressions[i].Eval(e);
			}

			Value result = Const.UNIT;

			if (this.expressions.Count > 0) {
				result = this.expressions[this.expressions.Count - 1].Eval(e);
			}

			foreach (KeyValuePair<String, Value> item in savedBindings) {
				e[item.Key] = item.Value;
			}

			return result;
		}

		public override String ToString() {
			return String.Join(Environment.NewLine, this.expressions);

		}

		public Expression Closure(ClosureManager manager) {
			return new Block(this.expressions.Select(expression => expression.Closure(manager)).ToList());
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			for (Int32 i = 0; i < this.expressions.Count - 1; i++) {
				IEnumerable<Value> x = this.expressions[i].EvalWithYield(scope);

				foreach (Value it in x) {
					if (it is GeneratorExpressionTerminalResult) {
						continue;
					}

					yield return it;
				}
			}

			IEnumerable<Value> z = this.expressions[this.expressions.Count - 1].EvalWithYield(scope);
			foreach (Value it in z) {
				yield return it;
			}
		}
	}
}
