using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

namespace Lumen.Lmi {
	public class IdExpression : Expression {
		public String id;
		public String file;
		public Int32 line;

		public IdExpression(String id, String file, Int32 line) {
			this.id = id;
			this.file = file;
			this.line = line;
		}

		public Value Eval(Scope scope) {
			if (!scope.IsExsists(this.id)) {
				throw new LumenException(Exceptions.UNKNOWN_NAME.F(this.id), this.line, this.file) {
					Note = NameHelper.MakeNamesNote(scope.AvailableNames, this.id),
				};
			}

			return scope.Get(this.id);
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			yield return new GeneratorExpressionTerminalResult(this.Eval(scope));
		}

		public Expression Closure(ClosureManager manager) {
			if (!manager.IsDeclared(this.id)) {
				if (!manager.Scope.IsExsists(this.id)) {
					throw new LumenException(Exceptions.UNKNOWN_NAME.F(this.id), this.line, this.file) {
						Note = NameHelper.MakeNamesNote(manager.Scope.AvailableNames, this.id),
					};
				}

				return new ValueLiteral(manager.Scope.Get(this.id));
			}

			return this;
		}

		public override String ToString() {
			return this.id;
		}
	}
}
