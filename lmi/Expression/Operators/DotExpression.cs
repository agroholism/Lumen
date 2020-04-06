using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang;

using String = System.String;

namespace Lumen.Lmi {
	// expression.memberName
	internal class DotOperator : Expression {
		internal Expression expression;
		internal String memberName;
		private readonly String fileName;
		private readonly Int32 line;

		public DotOperator(Expression expression, String nameVariable, String fileName, Int32 line) {
			this.expression = expression;
			this.memberName = nameVariable;
			this.fileName = fileName;
			this.line = line;
		}

		public Value Eval(Scope e) {
			// _.something ~~ (x -> x.something)
			if (this.expression is IdExpression idExpression && idExpression.id == "_") {
				return new UserFun(
					new List<IPattern> { new NamePattern("<de-generated-arg>") },
					new DotOperator(
						new IdExpression("<de-generated-arg>", this.line, this.fileName), this.memberName, this.fileName, this.line), "<de-generated-fun>");
			}
			
			try {
				Value value = this.expression.Eval(e);

				if (value is Module module) {
					return module.GetMember(this.memberName, e);
				}

				if (value is IType itype) {
					return itype.GetMember(this.memberName, e);
				}

				if (value is Instance instance) {
					return instance.GetField(this.memberName, e);
				}

				// internal unification //

				//if(value.Type.TryGetMember("<get>" + this.memberName, out Value property) && property is Fun funp) {
				//	return funp.Run(new Scope(e), value);
				//}

				//////////////////////////

				throw new LumenException(Exceptions.INSTANCE_OF_DOES_NOT_CONTAINS_FIELD.F(value.Type, memberName));
			}
			catch (LumenException hex) {
				if (hex.file == null) {
					hex.file = this.fileName;
				}

				if (hex.line == -1) {
					hex.line = this.line;
				}

				throw;
			}
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public Expression Closure(ClosureManager manager) {
			return new DotOperator(this.expression.Closure(manager), this.memberName, this.fileName, this.line);
		}

		public override String ToString() {
			return this.expression.ToString() + "." + this.memberName;
		}
	}
}

// Если expression - модуль или тип или конструктор: получаем его член
// Если expression - инстанс конструктора: получаем его поле
// Если не инстанс конструктора: выбрасываем исключение