using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class BlockE : Expression {
		public List<Expression> expressions;
		private IDictionary<String, Value> saver;
		internal Boolean saved = true;

		public BlockE() {
			expressions = new List<Expression>();
			saver = new Dictionary<string, Value>(0);
		}

		public BlockE(List<Expression> Expressions) {
			this.expressions = Expressions;
			saver = new Dictionary<string, Value>(0);
		}

		public void Add(Expression Expression) {
			expressions.Add(Expression);
		}

		public Value Eval(Scope e) {
			for (Int32 i = 0; i < this.expressions.Count - 1; i++) {
				if (saved && expressions[i] is VariableDeclaration decvar_) {
					if (e.IsExsists(decvar_.id)) {
						saver[decvar_.id] = e.Get(decvar_.id);
						Value value = decvar_.exp.Eval(e);
						Value type = null;

						if (decvar_.type != null)
							type = decvar_.type.Eval(e);

						e.variables[decvar_.id] = value;
					}
					else {
						this.saver[decvar_.id] = null;
						decvar_.Eval(e);
					}
				}
				else {
					this.expressions[i].Eval(e);
				}
			}

			Value v = Const.NULL;

			if (expressions.Count > 0) {
				if (saved && expressions[expressions.Count - 1] is VariableDeclaration decvar) {
					if (e.IsExsists(decvar.id)) {
						this.saver[decvar.id] = e.variables[decvar.id];
						Value value = decvar.exp.Eval(e);
						Value type = null;

						if (decvar.type != null) {
							type = decvar.type.Eval(e);
						}

						e.variables[decvar.id] = value;
					}
					else {
						this.saver[decvar.id] = null;
						decvar.Eval(e);
					}
				}
				else {
					v = this.expressions[this.expressions.Count - 1].Eval(e);
				}
			}

			foreach (KeyValuePair<String, Value> i in this.saver) {
				if (i.Value != null) {
					e.variables[i.Key] = i.Value;
				}
				else {
					e.Remove(i.Key);
				}
			}

			return v;
		}

		public override String ToString() {

			return "{ " + String.Join(Environment.NewLine, this.expressions.Select(i => "\t" + i.ToString())) + " }";
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new BlockE(this.expressions.Select(expression => expression.Closure(visible, thread)).ToList());
		}

		public Expression Optimize(Scope scope) {
			return new BlockE(this.expressions.Select(i => i.Optimize(scope)).ToList());
		}
	}
}
