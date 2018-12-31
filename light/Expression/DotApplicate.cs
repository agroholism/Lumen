using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class DotApplicate : Expression {
		internal Expression res;
		internal List<Expression> exps;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public DotApplicate(Expression res, List<Expression> exps) {
			this.res = res;
			this.exps = exps;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new DotApplicate(this.res.Closure(visible, thread), this.exps.Select(i => i.Closure(visible, thread)).ToList());
		}

		public Value Eval(Scope e) {
			Value obj = ((DotExpression)this.res).expression.Eval(e);

			if (obj is Expando) {
				Value v = ((Expando)obj).Get(((DotExpression)this.res).nameVariable, AccessModifiers.PUBLIC, e);

				if (v is Fun) {
					List<Value> Objects = new List<Value>();

					Scope x = new Scope(e);
					x.Set("this", obj);
					x.Set("self", v);

					foreach (Expression i in this.exps) {
						if (i is Assigment) {
							x.Set(((Assigment)i).id, ((Assigment)i).exp.Eval(e));
						}
						else {
							if (i is SpreadE) {
								foreach (Value j in Converter.ToList(i.Eval(e))) {
									Objects.Add(j);
								}
							}
							else {
								Objects.Add(i.Eval(e));
							}
						}
					}

					return ((Fun)v).Run(x, Objects.ToArray());
				}
				return new Null();
			}
			else if (obj is Module module) {
				if (!module.Contains(((DotExpression)this.res).nameVariable)) {
					throw new Lumen.Lang.Std.Exception("модуль не содержит функцию '" + ((DotExpression)this.res).nameVariable + "'", stack: e);
				}

				Value v = module.Get(((DotExpression)this.res).nameVariable);

				if (v is Fun func) {
					List<Value> Objects = new List<Value>();

					Scope x = new Scope(module.isNotScope ? e : module.scope);

					foreach (Expression i in this.exps) {
						if (i is Assigment) {
							x.Set(((Assigment)i).id, ((Assigment)i).exp.Eval(e));
						}
						else {
							if (i is SpreadE) {
								foreach (Value j in Converter.ToList(i.Eval(e), e)) {
									Objects.Add(j);
								}
							}
							else {
								Objects.Add(i.Eval(e));
							}
						}
					}
					//	MessageBox.Show((Objects == null).ToString());
					return func.Run(x, Objects.ToArray());
				}
				return new Null();
			}
			else if (obj is Record type) {
				Value fun = null;
				if (!type.Contains(((DotExpression)this.res).nameVariable)) {
					if (type.AttributeExists(((DotExpression)this.res).nameVariable)) {
						Fun attr = (Fun)type.GetAttribute(((DotExpression)this.res).nameVariable, e);
						fun = new LambdaFun((ex, args) => {
							Value ths = args[0];
							if (ths.Type != type) {
								throw new Lumen.Lang.Std.Exception("wrong type",stack: ex);
							}
							ex.Set("this", ths);
							Value[] newArgs = new Value[args.Length - 1];
							Array.Copy(args, 1, newArgs, 0, args.Length - 1);
							return attr.Run(ex, newArgs);
						});
					}
				}
				else {
					fun = type.Get(((DotExpression)this.res).nameVariable, e);
				}

				if (fun is Fun) {
					List<Value> args = new List<Value>();

					Scope x = new Scope(e) { ["this"] = obj };

					foreach (Expression i in this.exps) {
						if (i is Assigment) {
							x.Set(((Assigment)i).id, ((Assigment)i).exp.Eval(e));
						}
						else {
							if (i is SpreadE) {
								foreach (dynamic j in Converter.ToList(i.Eval(e))) {
									args.Add(j);
								}
							}
							else {
								args.Add(i.Eval(e));
							}
						}
					}

					return ((Fun)fun).Run(x, args.ToArray());
				}
				return new Null();
			}
			else {
				String name = ((DotExpression)this.res).nameVariable;
				Lumen.Lang.Std.Record cls = obj.Type;

				if (!cls.AttributeExists(name)) {
					try {
						return new Applicate(this.res, this.exps, -1).Eval(e);
					}
					catch {
						throw new Lumen.Lang.Std.Exception("тип не содержит функцию '" + name + "'", stack: e);
					}
				}

				Fun v = cls.GetAttribute(((DotExpression)this.res).nameVariable, e) as Fun;

				List<Value> Objects = new List<Value>();

				Scope x = new Scope(e) { ["this"] = obj };

				foreach (Expression i in this.exps) {
					if (i is Assigment) {
						x.Set(((Assigment)i).id, ((Assigment)i).exp.Eval(e));
					}
					else if (i is VariableDeclaration dec) {
						Objects.Add((KString)dec.id);
					}
					else {
						if (i is SpreadE) {
							foreach (dynamic j in Converter.ToList(i.Eval(e), e)) {
								Objects.Add(j);
							}
						}
						else {
							Objects.Add(i.Eval(e));
						}
					}
				}

				if (!Provider.isRelease) {
					String nameFunction = "";
					if (v is UserFun sfn) {
						if (sfn.Attributes.ContainsKey("name")) {
							nameFunction = sfn.Attributes["name"].ToString(e);
						}

						if (nameFunction == "") {
							nameFunction = cls.ToString() + "." + v.ToString();
						}

						if (nameFunction == "" || nameFunction.EndsWith(".")) {
							nameFunction = cls.ToString(e) + "." + name;
						}
					}
					else if (v is LambdaFun lfun) {
						if (lfun.Attributes.ContainsKey("name")) {
							nameFunction = lfun.Attributes["name"].ToString(e);
						}

						if (nameFunction == "") {
							nameFunction = cls.ToString(e) + "." + v.ToString(e);
						}

						if (nameFunction == "" || nameFunction.EndsWith(".")) {
							nameFunction = cls.ToString(e) + "." + name;
						}
					}
					else {
						nameFunction = v.ToString(e);

						if (nameFunction == "" || nameFunction.EndsWith(".")) {
							nameFunction = cls.ToString(e) + "." + name;
						}
					}

					Stopwatch s = new Stopwatch();
					Int32 before = Provider.profileResults.Count;
					s.Start();
					Value result = v.Run(x, Objects.ToArray());
					s.Stop();
					Int32 after = Provider.profileResults.Count;
					Provider.profileResults.Add(new ProfileResult {
						div = after - before,
						incl_time = s.ElapsedMilliseconds,
						excl_time = s.ElapsedMilliseconds,
						name = nameFunction
					});
					return result;
				}
				else {
					return v.Run(x, Objects.ToArray());
				}
			}
		}

		public override String ToString() {
			if(this.res is DotExpression de && de.nameVariable == "[]") {
				return de.expression + "[" + String.Join(", ", this.exps) + "]";
			}
			return this.res.ToString() + "(" + String.Join(", ", this.exps) + ")";
		}
	}
}