using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System;
using System.Collections.Generic;

namespace Stereotype {
	public class ConstantDeclaration : Expression {
		public Expression exp;
		public Expression type;
		public String id;
		public String file;
		public Int32 line;

		internal ConstantDeclaration(String id, Expression type, Expression exp, Int32 line, String file) {
			this.id = id;
			this.type = type;
			this.exp = exp;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope e) {
			Value value = this.exp.Eval(e);

			e.Set(this.id, value);
			e.AddConstant(this.id);

			return value;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			visible.Add(this.id);
			return new ConstantDeclaration(this.id, this.type?.Closure(visible, thread), this.exp?.Closure(visible, thread), this.line, this.file);
		}

		public Expression Optimize(Scope scope) {
			return new ConstantDeclaration(this.id, this.type, this.exp, this.line, this.file);
		}

		public override String ToString() {
			return $"const {this.id}{(this.type == null ? "" : $": {this.type}")} = {this.exp}";
		}
	}
}