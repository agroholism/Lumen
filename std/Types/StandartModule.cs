using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang.Std {
	public sealed class StandartModule : Module {
		#region Fields
		public static Record BigNum { get; } = new RBigNum();

		public static Record Vector { get; } = new RVec();
		public static Record Function { get; } = new FunctionClass();
		public static Record Num { get; } = new RNum();
		public static Record Expando { get; } = new ExpandoType();
		public static Record Map { get; } = new MapClass();
		public static Record Null { get; } = new RVoid();
		public static Record String { get; } = new StringClass();
		public static Record Enumerator { get; } = new EnumeratorType();
		public static Record File { get; } = new FileType();

		public static Record Exception { get; } = new ExceptionType();

		public static Record Boolean { get; } = new RBoolean();
		public static Record DateTime { get; } = new DateTimeClass();
		public static Record _Type { get; } = new TypeType();

		public static Fun CONSTANT = new LambdaFun((e, args) => throw new Exception("is constant"));

		public static StandartModule __Kernel__ { get; } = new StandartModule();

		#endregion
		private static readonly Random mainRandomObject = new Random();
		public static Dictionary<String, Dictionary<String, Value>> LoadedModules { get; } = new Dictionary<String, Dictionary<String, Value>>();

		private StandartModule() {
			Set("std", this);

			Set("vec", Vector);
			Set("seq", Enumerator);
			Set("num", Num);
			Set("bignum", BigNum);
			Set("bool", Boolean);
			Set("str", String);
			Set("fun", Function);
			Set("exception", Exception);

			Set("map", Map);

			Set("true", Const.TRUE);
			Set("false", Const.FALSE);
			Set("void", Const.VOID);

			Set("optional", new Optional(Num));

			Set("const", CONSTANT);

			Set("PI", (Num)Math.PI);
			Set("E", (Num)Math.E);

			Set("NL", (Str)Environment.NewLine);
			Set("EL", (Str)System.String.Empty);

			Set("fwrite", new LambdaFun((e, args) => {
				System.IO.File.WriteAllText(args[0].ToString(e), args[1].ToString(e));

				return Const.VOID;
			}));

			Set("generic", new LambdaFun((e, args) => {
				LambdaFun f = new LambdaFun((ex, argsx) => Const.VOID);

				f.Set("@gm", Const.TRUE, e);
				f.Set("x", new Vec(args.ToList()), e);

				return f;
			}));

			Set("enum", new LambdaFun((e, args) => {
				Fun f = args[0] as Fun;

				Scope s = new Scope(e) {
					This = f
				};

				RecordValue ret = f.Run(s) as RecordValue;

				Double d = 0;

				foreach (KeyValuePair<String, Value> i in ret.items) {
					if (!i.Key.StartsWith("@")) {
						RecordValue r = new RecordValue(f);
						r.Set("name", new Str(i.Key), e);
						if (i.Value is Void) {
							r.Set("value", (Num)d, e);
							d++;
						}
						else {
							r.Set("value", i.Value, e);
							d = i.Value.ToDouble(e);
						}
						f.Set(i.Key, r, e);
					}
				}

				f.Set(Op.LT, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool(first.Get("value", ex).ToDouble(ex) < sec.Get("value", ex).ToDouble(ex));
				}), e);

				f.Set(Op.GT, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool(first.Get("value", ex).ToDouble(ex) > sec.Get("value", ex).ToDouble(ex));
				}), e);

				f.Set(Op.EQL, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool(first.Get("value", ex).ToDouble(ex) == sec.Get("value", ex).ToDouble(ex));
				}), e);

				f.Set(Op.LTEQ, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool(first.Get("value", ex).ToDouble(ex) <= sec.Get("value", ex).ToDouble(ex));
				}), e);

				f.Set(Op.GTEQ, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool(first.Get("value", ex).ToDouble(ex) < sec.Get("value", ex).ToDouble(ex));
				}), e);

				f.Set("str", new LambdaFun((ex, argsx) => {
					return new Str($"{f.Get("@name", ex)}.{(ex.This as IObject).Get("name", ex)}");
				}), e);

				f.Set("inc", new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;

					Double z = first.ToDouble(ex) + 1;

					foreach (KeyValuePair<String, Value> i in f.Attributes) {
						if (i.Value is IObject iobj && iobj.Type == f && i.Value.ToDouble(ex) == z) {
							return i.Value;
						}
					}

					return Const.VOID;
				}), e);

				f.Set("dec", new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;

					Double z = first.ToDouble(ex) - 1;

					foreach (KeyValuePair<String, Value> i in f.Attributes) {
						if (i.Value is IObject iobj && iobj.Type == f && i.Value.ToDouble(ex) == z) {
							 return i.Value;
						}
					}

					return (ex.This as IObject).Get("value", ex);
				}), e);

				f.Set(Op.DOTI, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject y = argsx[0] as IObject;

					Double to = y.ToDouble(ex);

					IEnumerable<Value> Get() {
						Double z = first.ToDouble(ex);

						while (z <= to) {
							foreach (KeyValuePair<String, Value> i in f.Attributes) {
								if (i.Value is IObject iobj && iobj.Type == f && i.Value.ToDouble(ex) == z) {
									yield return i.Value;
								}
							}
							z++;
						}
					}

					return new Enumerator(Get());
				}), e);

				f.Set(Op.DOTE, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject y = argsx[0] as IObject;

					Double to = y.ToDouble(ex);

					IEnumerable<Value> Get() {
						Double z = first.ToDouble(ex);

						while (z < to) {
							foreach (KeyValuePair<String, Value> i in f.Attributes) {
								if (i.Value is IObject iobj && iobj.Type == f && i.Value.ToDouble(ex) == z) {
									yield return i.Value;
								}
							}
							z++;
						}
					}

					return new Enumerator(Get());
				}), e);

				f.Set("num", new LambdaFun((ex, argsx) => {
					return (ex.This as IObject).Get("value", ex);
				}), e);

				f.Set("by_value", new LambdaFun((ex, argsx) => {
					Double z = argsx[0].ToDouble(ex);

					foreach (KeyValuePair<String, Value> i in f.Attributes) {
						if (i.Value is IObject iobj && iobj.Type == f && i.Value.ToDouble(ex) == z) {
							 return i.Value;
						}
					}

					return Const.VOID;
				}), e);

				return f;
			}));

			Set("module", new LambdaFun((e, args) => {
				Fun f = args[0] as Fun;

				f.Set("@module", Const.TRUE, e);

				return f;
			}));
			Set("constructor", new LambdaFun((e, args) => {
				Fun f = args[0] as Fun;

				LambdaFun ctor = new LambdaFun((innerScope2, args2) => {
					if (innerScope2.This == null) {
						innerScope2.This = new RecordValue {
							Prototype = innerScope2["self"] as IObject
						};
					}

					f.Run(innerScope2, args2);
					return innerScope2.This;
				}) {
					Attributes = f.Attributes
				};

				ctor.Set("@constructor", Const.TRUE, e);

				return ctor;
			}));
			Set("derived", new LambdaFun((e, args) => {
				return new LambdaFun((innerScope, arguments) => {
					// old constructor
					Fun f = arguments[0] as Fun;

					// base constructor
					IObject obj = args[0] as IObject;

					Fun getConstructor(IObject iObject) {
						if (iObject is Fun fun) {
							return fun;
						}

						return iObject.Get("@constructor", innerScope) as Fun;
					}

					LambdaFun res = new LambdaFun((innerScope2, args2) => {
						// convert into real ctor
						Fun ctor = getConstructor(obj);

						RecordValue exemplare = new RecordValue();

						IObject self = innerScope2["self"] as IObject;

						innerScope2["base"] = new LambdaFun((is3, arg3) => {
							is3.This = exemplare;
							ctor.Run(is3, arg3);
							// can be changed
							exemplare.Prototype = self;
							return Const.VOID;
						});

						exemplare.Prototype = self;
						innerScope2.This = exemplare;
						f.Run(innerScope2, args2);
						return exemplare;
					}) {
						Attributes = f.Attributes
					};

					res.Set("@prototype", obj, innerScope);

					if (obj.TryGet("on_deriving", out Value fn)) {
						if (fn is Fun fun) {
							fun.Run(new Scope(innerScope), res);
						}
					}

					return res;
				});
			}));

			Set("comparable", new LambdaFun((e, args) => {
				Fun f = args[0] as Fun;

				f.Set(Op.LT, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;

					return new Bool((first.Type.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) < 0);
				}), e);

				f.Set(Op.GT, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool((first.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) > 0);
				}), e);

				f.Set(Op.EQL, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool((first.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) == 0);
				}), e);

				f.Set(Op.NOT_EQL, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool((first.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) == 0);
				}), e);


				f.Set(Op.LTEQ, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool((first.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) <= 0);
				}), e);

				f.Set(Op.GTEQ, new LambdaFun((ex, argsx) => {
					IObject first = ex.This as IObject;
					IObject sec = argsx[0] as IObject;

					return new Bool((first.Get(Op.SHIP, ex) as Fun).Run(ex, argsx).ToDouble(ex) >= 0);
				}), e);

				return f;
			}));

			Set("iterator", new LambdaFun((e, args) => {
				Fun f = args[0] as Fun;

				IEnumerable<Value> Iterate() {
					while (true) {
						Value v = null;

						try {
							v = f.Run(new Scope(e));
						}
						catch {

						}

						if (v == null) {
							yield break;
						}

						yield return v;
					}
				}

				return new Enumerator(Iterate());
			}));

			Set("random", new LambdaFun((e, args) => {
				Value up = e["up"];
				Value low = e["low"];

				return (Num)mainRandomObject.Next((Int32)Converter.ToDouble(up, e), (Int32)Converter.ToDouble(low, e));
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("up"),
						new FunctionArgument("low")
				}
			});

			Set("sum", new LambdaFun((scope, args) => {
				List<Value> prompt = scope["message"].ToList(scope);

				return prompt.Aggregate((x, y) => {
					return (x.Type.Get(Op.PLUS, scope) as Fun).Run(new Scope(scope) { This = x }, y);
				});
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("message", (Str)"") }
			});

			Set("print", new LambdaFun((scope, args) => {
				String separator = scope["sep"].ToString(scope);
				String onend = scope["onend"].ToString(scope);

				Value temp = scope["params"];

				String result = System.String.Join(separator, temp.ToIterator(scope).Select(i =>
					i.ToString(scope))) + (onend);

				Value file = scope["file"];

				if (file is Void) {
					Console.Write(result);
				}
				else if (file is Str) {
					Boolean isWriteMode = Converter.ToBoolean(scope["write?"]);

					// add encoding

					try {
						if (isWriteMode) {
							System.IO.File.WriteAllText(file.ToString(), result, Encoding.Default);
						}
						else {
							System.IO.File.AppendAllText(file.ToString(), result, Encoding.Default);
						}
					}
					catch { } // 
				}


				return Const.VOID;
			}) {
				Arguments = new List<FunctionArgument> {
						new FunctionArgument("*params"),
						new FunctionArgument("sep", (Str)" "),
						new FunctionArgument("onend", (Str)Environment.NewLine),
						new FunctionArgument("file", Const.VOID),
						new FunctionArgument("write?", Const.TRUE)
					}
			});
			Set("input", new LambdaFun((scope, args) => {
				String prompt = scope["message"].ToString(scope);
				if (prompt != "") {
					Console.Write(prompt);
				}
				return (Str)Console.ReadLine();
			}) {
				Arguments = new List<FunctionArgument> { new FunctionArgument("message", (Str)"") }
			});
		}

		public static Value Call(Value callable, Scope scope, params Value[] args) {
			if (callable is Fun f) {
				return f.Run(scope, args);
			}

			/*	if (callable.Type.AttributeExists("()")) {
					Scope s = new Scope(scope) {
						This = callable
					};
					return Call(callable.Type.GetAttribute("()", scope), s, args);
				}*/

			return null;
		}
	}

	public class Optional : SystemFun {
		public override List<FunctionArgument> Arguments { get; set; }

		static Dictionary<Value, Optional> cache = new Dictionary<Value, Optional>();

		static LambdaFun lf = new LambdaFun((e, args) => {
			Optional result = new Optional(args[0]);
			return result;
		});

		public Optional(Value value) {
			if (cache.TryGetValue(value, out Optional res)) {
				this.Attributes["generic_type"] = res.Attributes["generic_type"];
				return;
			}

			this.Attributes["generic_type"] = value;
			this.Attributes["@generic"] = lf;

			cache[value] = this;
		}

		public static Optional Create(Value value) {
			if (value is Optional) {
				return value as Optional;
			}

			return new Optional(value);
		}

		public override Value Run(Scope e, params Value[] args) {
			return args[0];
		}

		public override Boolean IsParentOf(Value value) {
			if (value is Void) {
				return true;
			}

			if (this.TryGet("generic_type", out Value result)) {
				return (result as IObject).IsParentOf(value);
			}

			return false;
		}

		public override String ToString() {
			return "std.optional[" + this.Attributes["generic_type"].ToString() + "]";
		}
	}
}
