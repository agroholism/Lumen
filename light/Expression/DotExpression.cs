using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class DotExpression : Expression {
		internal Expression expression;
		internal String nameVariable;
		private readonly String fileName;
		private readonly Int32 line;
		public Expression Optimize(Scope scope) {
			return this;
		}
		public DotExpression(Expression expression, String nameVariable) {
			this.expression = expression;
			this.nameVariable = nameVariable;
		}

		public DotExpression(Expression expression, String nameVariable, String fileName, Int32 line) : this(expression, nameVariable) {
			this.fileName = fileName;
			this.line = line;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return new DotExpression(this.expression.Closure(visible, thread), this.nameVariable, this.fileName, this.line);
		}

		public Value Eval(Scope e) {
			if (this.expression is IdExpression conste && conste.id == "_") {
				return new UserFun(
					new List<FunctionArgument> { new FunctionArgument("x") }, new DotExpression(new IdExpression("x", this.line, this.fileName), this.nameVariable));
			}

			try {
				Value a = this.expression.Eval(e);

				if (a is Module) {
					return ((Module)a).Get(this.nameVariable);
				}

				if (a is Record cls) {
					if (!cls.Contains(this.nameVariable)) {
						if (cls.AttributeExists(this.nameVariable)) {
							Fun fun = (Fun)cls.GetAttribute(this.nameVariable, e);
							LambdaFun result = new LambdaFun((ex, args) => {
								Value ths = args[0];
								ex.Set("this", ths);
								Array.Copy(args, 1, args, 0, args.Length - 1);
								return fun.Run(ex, args);
							});

							if (fun is LambdaFun lfun) {
								result.Attributes = lfun.Attributes;
							}
							else if (fun is UserFun sfun) {
								result.Attributes = sfun.Attributes;
							}

							return result;
						}
					}
					return ((Lumen.Lang.Std.Record)a).Get(this.nameVariable, e);
				}

				if (a is Expando) {
					return ((Expando)a).Get(this.nameVariable, AccessModifiers.PUBLIC, e);
				}

				Record type = a.Type;

				Int32 Index(List<FunctionArgument> pairs, String searched) {
					Int32 result = 0;

					foreach (FunctionArgument pair in pairs) {
						if (pair.name == searched) {
							return result;
						}
						result++;
					}

					return -1;
				}

				if (a is IObject obj) {
					if (Index(type.meta.Fields, this.nameVariable) != -1 || type.AttributeExists("get_" + this.nameVariable)) {
						AccessModifiers mode = AccessModifiers.PUBLIC;
						/*if ((this.expression is IdExpression id && id.id == "this") || (e.IsExsists("this") && e.Get("this").Type.Match(obj))) {
							mode = AccessModifiers.PRIVATE;
						}*/
						return obj.Get(this.nameVariable, mode, e);
					}
					else if (type.AttributeExists(this.nameVariable)) {
						Fun fun = (Fun)type.GetAttribute(this.nameVariable, e);
						LambdaFun result = new LambdaFun((ex, args) => {
							Value ths = args[0];
							ex.Set("this", ths);
							Array.Copy(args, 1, args, 0, args.Length - 1);
							return fun.Run(ex, args);
						});

						if (fun is LambdaFun lfun) {
							result.Attributes = lfun.Attributes;
						}
						else if (fun is UserFun sfun) {
							result.Attributes = sfun.Attributes;
						}

						return result;
					}
					return obj.Get(this.nameVariable, AccessModifiers.PRIVATE, e);
				}

				if (type.AttributeExists("get_" + this.nameVariable)) {
					if (type.GetAttribute("get_" + this.nameVariable, e) is Fun property) {
						return property.Run(new Scope { ["this"] = a });
					}
				}

				return Const.NULL;
			} catch (Lumen.Lang.Std.Exception hex) {
				if (hex.file == null) {
					hex.file = this.fileName;
				}

				if (hex.line == -1) {
					hex.line = this.line;
				}

				throw;
			}
		}

		public override String ToString() {
			return this.expression.ToString() + "." + this.nameVariable;
		}
	}
}