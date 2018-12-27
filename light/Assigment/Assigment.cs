using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	/// <summary> Assigment operator. </summary>
	public class Assigment : Expression {
		public Expression exp;
		public String id;
		public Int32 line;
		public String file;

		public Assigment(String id, Expression expression, Int32 line, String file) {
			this.id = id;
			this.exp = expression;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope scope) {
			if (scope.IsConstant(this.id)) {
				throw new Lumen.Lang.Std.Exception($"cannot change constant '{this.id}'", stack: scope) {
					line = this.line,
					file = this.file
				};
			}

			Value value = this.exp.Eval(scope);

			scope.Set(this.id, value);

			return value;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			if (!visible.Contains(this.id)) {
				visible.Add(this.id);
			}

			return new Assigment(this.id, this.exp.Closure(visible, thread), this.line, this.file);
		}

		public Expression Optimize(Scope scope) {
			if (scope is OptimizationScope os) {
				os.whileConstants.Remove(this.id);
			}

			return new Assigment(this.id, this.exp.Optimize(scope), this.line, this.file);
		}

		public override String ToString() {
			return $"{this.id} = {this.exp}";
		}
	}
}