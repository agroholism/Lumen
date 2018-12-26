using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class VariableDeclaration : Expression {
		public String id;
		public Expression type;
		public Expression exp;
		public String doc;
		public String file;
		public Int32 line;

		public VariableDeclaration(String name, Expression type, Expression expression, String file, Int32 line) {
			this.id = name;
			this.type = type;
			this.exp = expression;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope e) {
			Value value = this.exp.Eval(e);
			e.Set(this.id, value);

			return value;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(this.id);
			return new VariableDeclaration(this.id, this.type?.Closure(visible, thread), this.exp?.Closure(visible, thread), this.file, this.line);
		}

		public Expression Optimize(Scope scope) {
			if (scope is OptimizationScope os) {
				os.whileConstants.Add(this.id);
				os.constsValues[this.id] = this.exp;

				if (os.isPrimary) {
					os.notUsed.Add(this.id);
				}
				else if (os.notUsed.Contains(this.id)) {
					return Nop.Instance;
				}
			}

			return new VariableDeclaration(this.id, this.type?.Optimize(scope), this.exp?.Optimize(scope), this.file, this.line);
		}

		public override String ToString() {
			return $"var {this.id}{(this.type == null ? "" : $": {this.type}")} = {this.exp}";
		}
	}
}