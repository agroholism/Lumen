using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StandartLibrary;
using StandartLibrary.Expressions;

namespace Stereotype {
	[Serializable]
	internal class TCFExpression : Expression {
		private IDictionary<Expression, Expression> exceptCases;
		private Expression finallyExpression;
		private Expression tryExpression;

		public Expression Optimize(Scope scope) {
			return new TCFExpression(tryExpression.Optimize(scope), this.exceptCases, this.finallyExpression);
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
			catch (StandartLibrary.Exception uex) {
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
			var result = new StringBuilder();

			result.Append("try " + this.tryExpression.ToString()).Append(Environment.NewLine);

			foreach(var i in exceptCases) {
				result.Append("except " + i.Key + " " + i.Value).Append(Environment.NewLine);
			}

			return result.ToString();
		}
	}
}