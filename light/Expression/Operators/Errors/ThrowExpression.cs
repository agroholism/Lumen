using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class RaiseE : Expression {
		private readonly Expression expression;
		private readonly String file;
		private readonly Int32 line;

		public Expression Optimize(Scope scope) {
			return this;
		}
		public RaiseE(Expression mess, String fileName, Int32 line) {
			this.expression = mess;
			this.file = fileName;
			this.line = line;
		}

		public Value Eval(Scope e) {
			Value v = this.expression.Eval(e);

			if (v is Lumen.Lang.Std.Exception hex) {
				hex.line = this.line;
				hex.file = this.file;
				if (hex.stack == null) {
					hex.stack = e;
				}

				e.Set("$!", hex); // to global
				throw v as Lumen.Lang.Std.Exception;
			}

			Lumen.Lang.Std.Exception exception = new Lumen.Lang.Std.Exception(this.expression.Eval(e).ToString(), stack: e) {
				line = this.line,
				file = this.file
			};

			e.Set("$!", exception);
			throw exception;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new RaiseE(this.expression?.Closure(visible, thread), this.file, this.line);
		}
	}
}