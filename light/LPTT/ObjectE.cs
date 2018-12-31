using System;
using System.Collections.Generic;

using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class ObjectE : Expression {
		public Expression expression;
		public Expression p;

		public ObjectE(Expression expression) {
			this.expression = expression;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new ObjectE(this.expression.Closure(visible, thread));
		}
		public Expression Optimize(Scope scope) {
			return this;
		}

		public ObjectE(Expression expression, Expression p) {
			this.expression = expression;
			this.p = p;
		}

		public Value Eval(Scope e) {
			Expando result = null;

			if (this.p == null) {
				result = new Expando();
			}
			else {
				result = new Expando(this.p.Eval(e));
			}

			Scope x = new Scope(e);

			if (this.expression is BlockE) {
				foreach (Expression exp in (this.expression as BlockE).expressions) {
					if (exp is Assigment ass) {
						result.Set(ass.id, ass.exp.Eval(e), AccessModifiers.PUBLIC, e);
					}
					else if (exp is FunctionDefineStatement fds) {
						result.Set(fds.NameFunction, new AnonymeDefine(fds.Args, fds.Body, fds.returnedType, fds.otherContacts).Eval(e), AccessModifiers.PUBLIC, e);
					}
				}
			}
			else {
				if (this.expression is Assigment ass) {
					result.Set(ass.id, ass.exp.Eval(e), AccessModifiers.PUBLIC, e);
				}
				else if (this.expression is FunctionDefineStatement fds) {
					result.Set(fds.NameFunction, new AnonymeDefine(fds.Args, fds.Body, fds.returnedType, fds.otherContacts).Eval(e), AccessModifiers.PUBLIC, e);
				}
			}
			return result;
		}
	}
}