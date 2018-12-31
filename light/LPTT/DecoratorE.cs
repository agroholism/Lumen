using System;
using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
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
			if(func is DecoratorE dece) {
				this.hightestExpression = dece.hightestExpression;
			} else {
				this.hightestExpression = func;
			}
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			Value val = this.expsOfDecos[0].Eval(e);
			if (this.func is FunctionDefineStatement fds) {
				this.func.Eval(e);
				Fun v = e.Get(fds.NameFunction) as Fun;

				if(val is Record type) {
					v.Attributes["returned"] = type;
				}
				else if (val == Const.NULL) {
					v.Attributes["returned"] = StandartModule.Null;
				}
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
			else if (this.func is StructE classCreator) {
				classCreator.Eval(e);
				Value v = e[classCreator.name];
			/*	Value v = new HObject {
					["final?", e] = (Bool)false,
				//	["fields", e] = new KList(classCreator.)
				};
				*/
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
			return Const.NULL;
		}
	}
}