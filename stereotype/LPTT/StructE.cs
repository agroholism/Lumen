using System;
using System.Linq;
using System.Collections.Generic;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;
using static Stereotype.FunctionDefineStatement;

namespace Stereotype {
	public class StructE : Expression {
		public String name;
		public Dictionary<String, Expression> fields;
		public List<Expression> self;
		public List<Expression> interfaces;
		public List<Expression> attrs;
		public Boolean isFinal;
		public List<String> closed;
		public Int32 line;
		public String file;
		public Boolean isRecord;
		private List<String> finalMembers;

		public Expression Optimize(Scope scope) {
			return this;
		}

		public StructE(String name, Dictionary<string, Expression> fields, List<Expression> self, List<Expression> attrs, List<Expression> interfaces, List<String> privates) {
			this.name = name;
			this.fields = fields;
			this.self = self;
			this.interfaces = interfaces;
			this.attrs = attrs;
			this.closed = privates;
		}

		public StructE(String name, Dictionary<String, Expression> fields, List<Expression> self, List<Expression> attrs, List<Expression> interfaces, List<String> privates, List<String> finalMembers, Int32 line, String file) : this(name, fields, self, attrs, interfaces, privates) {
			this.finalMembers = finalMembers;
			this.line = line;
			this.file = file;
		}

		public Value Eval(Scope e) {
			KType result = new KType();
			e.Set(this.name, result);

			result.meta = new TypeMetadata {
				Name = this.name
			};

			// Создание информации о полях экземпляров.
			List<String> names = new List<String>();

			Boolean isExtented = false;

			List<String> finals = new List<String>();

			Boolean isException = false;

			/*if (!isExtented) {
				result.meta.BaseType = StandartLibrary.StandartModule.Object;
			}*/

			List<FunctionArgument> arg = new List<FunctionArgument>();
			foreach (KeyValuePair<String, Expression> i in this.fields) {
				names.Add(i.Key);
				if (i.Value != null) {
					arg.Add(new FunctionArgument(i.Key, (Value)i.Value.Eval(e)));
				}
				else {
					arg.Add(new FunctionArgument(i.Key));
				}
			}
			result.meta.Name = name;
			result.meta.Fields = names.ToArray();

		//	var def = null;// new Initialize(arg.args);

			//result.SetAttribute("initialize", def);

			Boolean flag = false;
			/*KType typ = result.meta.BaseType;
			while (!(StandartLibrary.StandartModule.Object.Equals(typ))) {
				if (typ is ExpandoType) {
					flag = true;
					break;
				}
				typ = typ.meta.BaseType;
			}

			if (!flag) {
				typ = result.meta.BaseType;
				while (!(StandartLibrary.StandartModule.Object.Equals(typ))) {
					if (typ is ExceptionType) {
						isException = true;
						break;
					}
					typ = typ.meta.BaseType;
				}
			}*/

			if (isException) {
				result.Set("new", new LambdaFun((scope, argsx) => {
					Lumen.Lang.Std.Exception obj = new Lumen.Lang.Std.Exception(argsx[0].ToString(scope), result);

					Scope ex = new Scope { ["this"] = obj };

				/*	if (obj.Type.meta.BaseType.AttributeExists("initialize")) {
						ex.Set("base", new LambdaFun((s, arguments) => {
							s.Set("this", result);
							return ((Fun)obj.Type.meta.BaseType.GetAttribute("initialize", e)).Run(scope, arguments);
						}));
					}
					*/
					if (obj.Type.AttributeExists("initialize"))
						((Fun)obj.Type.GetAttribute("initialize", e)).Run(ex, argsx);

					return obj;
				}));
			}
			/*else if (!flag) {
				result.Set("new", new Create(result));
			}*/
			else {
				result.Set("new", new LambdaFun((scope, argsx) => {
					Expando obj = new Expando() { type = result };

					Scope ex = new Scope { ["this"] = obj };

					/*if (obj.Type.meta.BaseType.Contains("this"))
						ex.Set("base", new LambdaFun((scopex, arguments) => {
							scopex.Set("this", result);
							return ((Fun)obj.Type.meta.BaseType.Get("this", e)).Run(scopex, arguments);
						}));
						*/
					((Fun)result.GetAttribute("this", e)).Run(ex, argsx);

					return obj;
				}));
			}
			/*
			foreach (Expression i in this.attrs) {
				if (i is FunctionDefineStatement x) {
					if (result.meta.BaseType.AttributeExists(x.NameFunction)) {
						if (finals.Contains(x.NameFunction)) {
							throw new StandartLibrary.Exception($"can't {x.NameFunction}", stack: e);
						}

						Fun f = result.meta.BaseType.GetAttribute(x.NameFunction, e) as Fun;
						Scope frepl = new TypedScope(e) {
							["base"] = new LambdaFun((scope, args) => f.Run(scope, args))
						};

						if (x.NameFunction == "initialize") {
							frepl["default"] = new LambdaFun((ex, argsx) => def.Run(ex, argsx));
						}

						result.SetAttribute(x.NameFunction, new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(frepl) as Fun);
						continue;
					}
					else if (x.NameFunction == "initialize") {
						Scope frepl = new TypedScope(e) {
							["default"] = new LambdaFun((ex, atgsx) => {
								return def.Run(ex, atgsx);
							})
						};
						result.SetAttribute("initialize", new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(frepl) as Fun);
						continue;

					}

				/*	if (result.AttributeExists(x.NameFunction) && result.GetAttribute(x.NameFunction, e) is UserFun uf) {
						Dispatcher dis = new Dispatcher();
						dis.Add(uf);
						dis.Add(new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(e) as UserFun);
						result.SetAttribute(x.NameFunction, dis);
						continue;
					}
					*
					UserFun fun = new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(e) as UserFun;
					fun.Set("name", (KString)(this.name + "." + x.NameFunction), AccessModifiers.PRIVATE, e);
					result.SetAttribute(x.NameFunction, fun);
				}
			}
	*/
			List<String> typeVariables = new List<string>();

			/*foreach (Expression i in this.self) {
				if (i is FunctionDefineStatement x) {
					if (result.meta.BaseType != null) {
						if (result.meta.BaseType.Contains(x.NameFunction)) {
							Scope frepl = new TypedScope(e) {
								["base"] = new LambdaFun((scope, args) => {
									return ((Fun)result.meta.BaseType.Get(x.NameFunction, e)).Run(scope, args);
								})
							};
							result.Set(x.NameFunction, new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(frepl));

							continue;
						}
					}
					result.Set(x.NameFunction, new AnonymeDefine(x.Args, x.Body, x.returnedType, x.otherContacts).Eval(e));
				}
				else if (i is ConstantDeclaration c) {
					result.Set(c.id, c.exp.Eval(e));
					typeVariables.Add(c.id);
				}
				else if (i is VariableDeclaration ae) {
					result.Set(ae.id, ae.exp.Eval(e));
				}
			}*/

			result.variables = typeVariables.ToArray();

			foreach (Value i in result.includedModules) {
				if (i is Module hi) {
					if (!hi.TypeImplemented(result)) {
						throw new Lumen.Lang.Std.Exception($"класс {this.name} не удовлетворяет требованиям модуля {hi.name}", stack: e);
					}
				}
			}

			if (isRecord) {
			//	result.SetAttribute("to_s", new ToSRec(this.fields.Select(i => i.Key).ToArray()));
			}

			//result.meta.Closed = this.closed.Union(result.meta.BaseType.meta.Closed).ToArray();

			return Const.NULL;
		}

		/*public class ToSRec : SystemFun {
			String[] names;

			public ToSRec(String[] names) {
				this.names = names;
			}

			public override Value Run(Scope e, params Value[] args) {
				String result = "";
				IObject ts = e.This as IObject;

				return (KString)("(" + String.Join(", ", names.Select(i => ts.Get(i, AccessModifiers.PRIVATE, e).ToString())) + ")");
			}
		}
		*/
		public Expression Closure(List<String> visible, Scope thread) {
			Dictionary<String, Expression> new_fields = new Dictionary<String, Expression>();

			foreach (KeyValuePair<String, Expression> i in this.fields) {
				new_fields[i.Key] = i.Value?.Closure(visible, thread);
			}

			visible.Add(this.name);

			return new StructE(this.name, new_fields, self.Select(i => i.Closure(visible, thread)).ToList(), this.attrs.Select(i => i.Closure(visible, thread)).ToList(), this.interfaces.Select(i => i.Closure(visible, thread)).ToList(), this.closed);
		}

		/*[Serializable]
		public class Initialize : SystemFun {
			List<FunctionArgument> args;

			public Initialize(List<FunctionArgument> args) {
				this.args = args;
			}

			public override Value Run(Scope kt, params Value[] args) {
				IObject th = (IObject)kt.Get("this");

				if (kt.ExistsInThisScope
					("base") && kt.Get("base") is Fun method) {
					method.Run(new TypedScope(kt) { ["this"] = th }, args);
				}

				Int32 index = 0;
				foreach (FunctionArgument i in this.args) {
					if (index < args.Length) {
						th.Set(i.name, args[index], AccessModifiers.PRIVATE, kt);
					}
					else {
						th.Set(i.name, (Value)i.defaultValue, AccessModifiers.PRIVATE, kt);
					}

					index++;
				}
				return Const.NULL;
			}
		}

		[Serializable]
		public class Create : SystemFun {
			KType obj;

			public Create(KType obj) {
				this.obj = obj;
			}

			public override List<FunctionArgument> Arguments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

			public override Value Run(Scope e, params Value[] args) {
				IObject result = null;

				int size = obj.meta.Fields.Length;

				switch (size) {
					case 0:
						result = new Object0(obj);
						break;
					case 1:
						result = new Object1(obj);
						break;
					case 2:
						result = new Object2(obj);
						break;
					case 3:
						result = new Object3(obj);
						break;
					case 4:
						result = new Object4(obj);
						break;
					default:
						result = new Objectn(size, obj);
						break;
				}

				Scope ex = new TypedScope(e) { ["this"] = result, ["<CTOR>"] = (Bool)true };

				if (obj.meta.BaseType.AttributeExists("initialize")) {
					ex.Set("base", new LambdaFun((scope, arguments) => {
						scope.Set("this", result);
						return ((Fun)obj.meta.BaseType.GetAttribute("initialize", e)).Run(scope, arguments);
					}));
				}

				((Fun)obj.GetAttribute("initialize", e)).Run(ex, args);

				return result;
			}
		}*/
	}
}