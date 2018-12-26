using System;
using System.Collections.Generic;
using StandartLibrary;
using StandartLibrary.Expressions;

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
				hightestExpression = dece.hightestExpression;
			} else {
				hightestExpression = func;
			}
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			if (func is FunctionDefineStatement fds) {
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
			else if (func is AnonymeDefine ad) {
				Value v = func.Eval(e);
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
				return v;
			}
			else if (func is StructE classCreator) {
				classCreator.Eval(e);
				Value v = e[classCreator.name];
			/*	Value v = new HObject {
					["final?", e] = (Bool)false,
				//	["fields", e] = new KList(classCreator.)
				};
				*/
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
				return v;
			}
			return Const.NULL;
		}
	}
}