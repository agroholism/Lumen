using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Stereotype {
	internal class InstanceCreateE : Expression {
		public Expression res;
		public Expression eigen;
		public List<Expression> inits;
		public Int32 line;
		public String fileName;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public InstanceCreateE(Expression res, List<Expression> inits, Int32 line, System.String fileName) {
			this.res = res;
			this.inits = inits;
			this.line = line;
			this.fileName = fileName;
		}

		public Expression Closure(System.Collections.Generic.List<String> visible, Scope thread) {
			return new InstanceCreateE(this.res.Closure(visible, thread), this.inits?.Select(i => i.Closure(visible, thread)).ToList(), this.line, this.fileName);
		}

		public Value Eval(Scope e) {
			if (this.res is Applicate a) {
				Value v = a.callable.Eval(e);

				if (v is Lumen.Lang.Std.Record x) {
					try {
						Value result = a.ExecuteCasual((Fun)x.Get("new", e), e);

						foreach(Expression i in this.inits) {
							if (i is Assigment ass) {
								new DotAssigment(new DotExpression(new ValueE(result), ass.id), ass.exp, this.fileName, this.line).Eval(e);
							}
							else if(i is ButE be && be.expression is Assigment) {
								ass = be.expression as Assigment;
								be.res.Eval(e);
								new DotAssigment(new DotExpression(new ValueE(result), ass.id), ass.exp, this.fileName, this.line).Eval(e);
							}
							else {
								new BinaryExpression(new ValueE(result), i, "+=", this.line, this.fileName).Eval(e);
							}
						}

						return result;
					} catch (Lumen.Lang.Std.Exception except) {
						if (except.line == -1) {
							except.line = this.line;
						}
						throw except;
					}
					
				} else if(v is Expando hob) {
					try {
						return a.ExecuteCasual((Fun)hob.Get("new", AccessModifiers.PUBLIC, e), e);
					}
					catch (Lumen.Lang.Std.Exception except) {
						if (except.line == -1) {
							except.line = this.line;
						}
						throw except;
					}
				}

				return Const.NULL;
			}
			else if (this.res is DotApplicate da) {
				return new Applicate(new DotExpression(da.res, "new"), da.exps, this.line).Eval(e);
			}

			return Const.NULL;
		}
	}
}