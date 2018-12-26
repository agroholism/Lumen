using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class IdExpression : Expression {
		public String id;
		public Int32 line;
		public String file;

		public IdExpression(String id, Int32 line, String file) {
			this.id = id;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope e) {
			if (!e.IsExsists(this.id)) {
				throw new Lumen.Lang.Std.Exception($"unknown identifiter '{this.id}'", stack: e) { file = this.file, line = this.line };
			}

			return e.Get(this.id);
		}

		public Expression Optimize(Scope scope) {
			if (scope is OptimizationScope os) {
				os.notUsed.Remove(this.id);

				if (os.whileConstants.Contains(this.id)) {
					return os.constsValues[this.id];
				}
			}

			return this;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			if (!(visible.Contains(this.id))) {
				if (!thread.IsExsists(this.id)) {
					throw new Lumen.Lang.Std.Exception($"unknown identifiter '{this.id}'", stack: thread) { file = this.file, line = this.line };
				}
				return new ValueE(thread.Get(this.id));
			}

			return this;
		}

		public override String ToString() {
			return this.id;
		}
	}
}
