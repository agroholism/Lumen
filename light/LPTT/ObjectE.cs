using System;
using System.Collections.Generic;

using Lumen;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	public class ObjectE : Expression {
		public Expression expression;
		private Dictionary<String, Expression> items;

		public Expression Closure(List<String> visible, Scope thread) {
			return this;//new ObjectE(this.expression.Closure(visible, thread));
		}
		public Expression Optimize(Scope scope) {
			return this;
		}

		public ObjectE(Dictionary<String, Expression> items, Expression expression) {
			this.items = items;
			this.expression = expression;
		}

		public ObjectE(Dictionary<String, Expression> items) {
			this.items = items;
		}

		public Value Eval(Scope e) {
			RecordValue result = new RecordValue();

			if(this.expression != null) {
				result.Prototype = this.expression.Eval(e) as IObject;
			}

			foreach(var i in this.items) {
				result.Set(i.Key, i.Value.Eval(e), e);
			}

			/*if (this.p == null) {
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
			}*/
			return result;
		}
	}
}