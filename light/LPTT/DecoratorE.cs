using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class DecoratorE : Expression {
		internal List<Expression> expsOfDecos;
		internal Expression func;
		internal Expression hightestExpression;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public DecoratorE(List<Expression> expsOfDecos, Expression func) {
			this.expsOfDecos = expsOfDecos;
			this.func = func;
			if (func is DecoratorE dece) {
				this.hightestExpression = dece.hightestExpression;
			}
			else {
				this.hightestExpression = func;
			}
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			Fun val = null;

			/*if(val.TryGet("@gm", out Value res)) {
				if (this.func is FunctionDeclaration fds1) {
					Fun f =  new LambdaFun((ex, argsx) => {
						return Const.VOID;
					});

					f.Set("@generic", new LambdaFun((ex, argsx) => {
						Scope s = new Scope(e);

						Int32 index = 0;
						foreach(var i in val.Get("x", e).ToList(e)) {
							s[i.ToString(e)] = argsx[index];
							index++;
						}

						Fun r = this.func.Eval(s) as Fun;
						r.Attributes = f.Attributes;
						return r;
					}), e);

					e.Set(fds1.NameFunction, f);

					return Const.VOID;
				}
			}
			*/
			if (this.func is FunctionDeclaration fds) {
				this.func.Eval(e);

				Value v = e.Get(fds.NameFunction);
				for (Int32 i = this.expsOfDecos.Count - 1; i > -1; i--) {
					val = this.expsOfDecos[i].Eval(e) as Fun;

					v = val.Run(new Scope(e) { }, v);
				}
				e.Set(fds.NameFunction, v);

				return Const.VOID;
			}

			if (this.func is FunctionDefineDotStatement fdds) {
				this.func.Eval(e);

				IObject v = e.Get(fdds.helper[0]) as IObject;

				for (Int32 i = 1; i < fdds.helper.Count; i++) {
					v = v.Get(fdds.helper[i], e) as IObject;
				}

				Value f = v.Get(fdds.name, e) as Fun;

				for (Int32 i = this.expsOfDecos.Count - 1; i > -1; i--) {
					val = expsOfDecos[i].Eval(e) as Fun;

					f = val.Run(new Scope(e) { }, f);
				}

				v.Set(fdds.name, f, e);
				return Const.VOID;
			}

			if (this.func is VariableDeclaration vd) {
				this.func.Eval(e);

				e.SetAttribute(vd.id, val);
			}


			/*if (func is FunctionDefineStatement fds) {
				func.Eval(e);
				Value v = e.Get(fds.NameFunction);
				for (int i = expsOfDecos.Count - 1; i > -1; i--) {
					if (expsOfDecos[i] is IdExpression || expsOfDecos[i] is DotExpression)
						v = new Applicate(expsOfDecos[i], new List<Expression> { new ValueE(v) }, -1).Eval(e);
					else if (expsOfDecos[i] is Applicate app) {
						List<Expression> args = new List<Expression> { new ValueE(v) };
						foreach (var x in app.argse) {
							args.Add(x);
						}
						v = new Applicate(app.callable, args, -1).Eval(e);
					}
				}
				e.Set(fds.NameFunction, v);
				return Const.NULL;
			}
			else */
			if (this.func is AnonymeDefine ad) {
				Value v = this.func.Eval(e);
				for (Int32 i = this.expsOfDecos.Count - 1; i > -1; i--) {
					if (this.expsOfDecos[i] is IdExpression || this.expsOfDecos[i] is DotExpression) {
						v = new Applicate(this.expsOfDecos[i], new List<Expression> { new ValueE(v) }, -1).Eval(e);
					}
					else if (this.expsOfDecos[i] is Applicate app) {
						List<Expression> args = new List<Expression> { new ValueE(v) };
						foreach (Expression x in app.argse) {
							args.Add(x);
						}
						v = new Applicate(app.callable, args, -1).Eval(e);
					}
				}
				return v;
			}
			return Const.VOID;
		}
	}
}