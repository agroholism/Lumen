using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	[Serializable]
	internal class Namespace : Expression {
		internal String name;
		internal List<Expression> expressions;

		public Namespace(String name, List<Expression> expressions) {
			this.name = name;
			this.expressions = expressions;
		}
		public Expression Optimize(Scope scope) {
			return this;
		}
		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			Scope x = new Scope(e);
			Module mod = new Module(x) {
				name = this.name
			};

			foreach (Expression expression in this.expressions) {
				if (expression is FunctionDefineStatement fun && fun.Body == null) {
					List<FunctionArgument> args = new List<FunctionArgument>();

					Record exte = null;

					foreach (ArgumentMetadataGenerator i in fun.Args) {
						FunctionArgument arg = i.EvalArgumnet(e);
					/*	if (arg.name == "this" && arg.type != null && arg.type is KType type) {
							exte = type;
							continue;
						}*/
						args.Add(arg);
					}

				/*	FunctionMetadata fmtd = new FunctionMetadata(args, fun.returnedType == null ? null : (Isable)fun.returnedType.Eval(e));
					mod.require.Add(fun.NameFunction, fmtd);*/
				}
				else {
					expression.Eval(x);
				}
			}

			Rename(mod, e);

			e.Set(this.name, mod);

			return Const.NULL;
		}

		public static void Rename(Module m, Scope e) {
			foreach (KeyValuePair<String, Value> i in m.scope.variables) {
				if (i.Value is Record type) {
					type.meta.Name = m.name.Split('.')[0] + "." + type.meta.Name;
					foreach(KeyValuePair<String, Fun> j in type.attributes) {
						if(j.Value is LambdaFun lfun) {
							if(lfun.Attributes.ContainsKey("name")) {
								lfun.Set("name", (KString)(type.meta.Name.Split('.')[0] + "." + lfun.Get("name", AccessModifiers.PRIVATE, e)), AccessModifiers.PRIVATE, e);
							}
						}
						else if (j.Value is UserFun sfun) {
							if (sfun.Attributes.ContainsKey("name")) {
								sfun.Set("name", (KString)(type.meta.Name.Split('.')[0] + "." + sfun.Get("name", AccessModifiers.PRIVATE, e)), AccessModifiers.PRIVATE, e);
							}
						}
					}
				}
				else if (i.Value is UserFun sfun) {
					sfun.Set("name", (KString)(m.name.Split('.')[0] + "." + sfun.Get("name", AccessModifiers.PRIVATE, e)), AccessModifiers.PRIVATE, e);
				}
				else if (i.Value is LambdaFun lfun) {
					lfun.Set("name", (KString)(m.name.Split('.')[0] + "." + lfun.Get("name", AccessModifiers.PRIVATE, e)), AccessModifiers.PRIVATE, e);
				}
				else if (i.Value is Module md) {
					md.name = m.name.Split('.')[0] + "." + md.name;
					Rename(md, e);
				}
			}
		}
	}
}