using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class TCFExpression : Expression {
		private readonly IDictionary<Expression, Expression> exceptCases;
		private readonly Expression finallyExpression;
		private readonly Expression tryExpression;

		public Expression Optimize(Scope scope) {
			return new TCFExpression(this.tryExpression.Optimize(scope), this.exceptCases, this.finallyExpression);
		}

		public TCFExpression(Expression tryExpression, IDictionary<Expression, Expression> exceptCases, Expression finallyExpression) {
			this.tryExpression = tryExpression;
			this.exceptCases = exceptCases;
			this.finallyExpression = finallyExpression;
		}
		//
		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			try {
				return this.tryExpression.Eval(e);
			}
			catch (Lumen.Lang.Std.Exception uex) {
				e["$!"] = uex;
				foreach (KeyValuePair<Expression, Expression> i in this.exceptCases) {
					if (i.Key is UnknownExpression) {
						return i.Value.Eval(e);
					}

					Value value = i.Key.Eval(e);
					if(uex.type == value) {
						return i.Value.Eval(e);
					}
				}
			}
			catch (System.Exception ex) {

			}
			finally {
				if (this.finallyExpression != null) {
					this.finallyExpression.Eval(e);
				}
			}

			return Const.NULL;
		}

		public override String ToString() {
			StringBuilder result = new StringBuilder();

			result.Append("try " + this.tryExpression.ToString()).Append(Environment.NewLine);

			foreach(KeyValuePair<Expression, Expression> i in this.exceptCases) {
				result.Append("except " + i.Key + " " + i.Value).Append(Environment.NewLine);
			}

			return result.ToString();
		}
	}
}