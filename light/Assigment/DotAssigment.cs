using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class DotAssigment : Expression {
		internal Expression rigth;
		internal DotExpression left;
		internal String file;
		internal Int32 line;

		internal DotAssigment(DotExpression left, Expression rigth, String file, Int32 line) {
			this.left = left;
			this.rigth = rigth;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope e) {
			Value value = this.rigth.Eval(e);
			String name = this.left.nameVariable;
			Value obj = this.left.expression.Eval(e);
			if (obj is IObject iobj) {
				iobj.Set(name, value, e);
			}
			else if (obj is Module module) {
				module.Set(name, value);
			}
			else if (obj is Expando hobj) {
				hobj.Set(name, value, AccessModifiers.PUBLIC, e);
			}
			else {
				var type = obj.Type;
				if (type.TryGet("set_" + name, out var prf) && prf is Fun property) {
					property.Run(new Scope(e) { ["this"] = obj }, value);
				}
				else {
					throw new Lumen.Lang.Std.Exception($"object of type '{type}' does not have a field/property '{name}'", stack: e) {
						file = this.file,
						line = this.line
					};
				}
			}

			return value;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new DotAssigment(this.left.Closure(visible, thread) as DotExpression, this.rigth.Closure(visible, thread), this.file, this.line);
		}

		public Expression Optimize(Scope scope) {
			return this;
		}

		public override String ToString() {
			return $"{this.left} = {this.rigth}";
		}
	}
}