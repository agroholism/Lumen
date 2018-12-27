using System;
using System.Collections.Generic;
using System.Reflection;

using Lumen.Lang.Expressions;
using Lumen.Lang.Std;

namespace Stereotype {
	internal class DynamicLoad : Expression {
		private string name;

		public Expression Optimize(Scope scope) {
			return this;
		}
		public DynamicLoad(string name) {
			this.name = name;
		}

		public Expression Closure(List<String> visible, Scope thread) {
			return this;
		}

		public Value Eval(Scope e) {
			/* if (name.Contains("."))
			 {
				 string[] a = name.Split('.');
				 if (e.IsExsists(a[0]))
				 {
					 var v = e.Get(a[0]);
					 for (int i = 1; i < a.Length - 1; i++)
					 {
						 v = ((Core.Module)v).Get(a[i]);
					 }

					 ((Core.Module)v).Set(a[a.Length - 1], new DynamicModule(Assembly.Load(name), name));
				 }
				 else
				 {
					 e.Set(a[0], new DynamicModule(Assembly.Load(name)));
				 }
			 }
			 else*/
		//	e.Set(name, new NETModule(Assembly.Load(name), name));
			return Const.NULL;
		}
	}

	internal class NETModule : Lumen.Lang.Std.IObject {
		public Assembly module;
		public String name;
		readonly Dictionary<String, Value> cache = new Dictionary<string, Value>();

		public Record Type => Lumen.Lang.Std.StandartModule._Type;

		public NETModule(Assembly asm, String name) {
			this.module = asm;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			String fullName = this.name == null ? name : this.name + "." + name;

			Value val = null;
			if (this.cache.TryGetValue(fullName, out val)) {
				return val;
			}
			else {
				var t = this.module.GetType(fullName);
				return new NETType(t);
			}
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			throw new NotImplementedException();
		}

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}
	}

	internal class Dynamic : Value {
		public object Value;

		public Record Type => new NETType(this.Value.GetType());

		public Dynamic(Object Value) {
			this.Value = Value;
		}

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			throw new NotImplementedException();
		}
	}

	internal class NETType : Record {
		Type type;

		public NETType(Type type) {
			this.type = type;
		}

		public Value Get(String Name) {
			if (type.IsEnum)
				return new Dynamic(type.GetField(Name).GetValue(type));

			if (Name == "()") {
				return new LambdaFun((e, args) => {
					Type[] Types = new Type[args.Length];

					for (int i = 0; i < args.Length; i++)
						Types[i] = args[i].GetType();

					dynamic x;

					x = type.GetConstructor(Types);

					x = x.Invoke(args);

					return new Dynamic(x);
				});
			}

			/*	foreach (Type i in Type.GetNestedTypes())
					if (i.Name == Name)
						return new DynamicType(i);
	*/
			try {
				var x = type.GetMethod(Name);
				if (x != null)
					return new LambdaFun((e, args) => {
						object[] margs = new object[args.Length];
						for (int i = 0; i < args.Length; i++)
							margs[i] = /*C.Unpack(*/args[i]/*)*/;
						return new Dynamic(x.Invoke(null, margs));
					});
				else {
					var a = type.GetProperty(Name);
					/*if (a == null)
						return new FunDelegate(type, Name);
					else return new Dynamic(a.GetValue(type, new object[] { }));*/
				}
			}
			catch {
				/*var a = type.GetProperty(Name);
				if (a == null)
					return new FunDelegate(type, Name);
				else return new Dynamic(a.GetValue(type, new object[] { }));*/
			}
			return null;
		}

		/*public void Set(string Name, dynamic Value) {
			type.GetProperty(Name).SetValue(type, Value);
		}*/
	}

	/* internal class DynamicModule : Core.Module
	 {
		 public Assembly Namespace;
		 private string path;

		 public DynamicModule(Assembly Namespace)
		 {
			 this.Namespace = Namespace;
		 }

		 public DynamicModule(Assembly Namespace, string path)
		 {
			 this.Namespace = Namespace;
			 this.path = path;
		 }

		 public override Value Get(string Name)
		 {
			 if (path == null)
			 {
				 Type a = Namespace.GetType(Name);

				 if (a != null)
					 return new DynamicType(a);

				 if (Contains(Name))
					 return variables[Name];

				 foreach (Type i in Namespace.ExportedTypes)
					 if (i.FullName.StartsWith(Name + "."))
						 return new DynamicModule(Namespace, Name);
			 }
			 else
			 {
				 Type a = Namespace.GetType(path + "." + Name);

				 if (a != null)
					 return new DynamicType(a);

			   /*  foreach (Type i in Namespace.ExportedTypes)
					 if (i.FullName.StartsWith(path + "." + Name + "."))
						 return new DynamicModule(Namespace, path + "." + Name);
			 }

			 throw new HException("Not a type " + Name, "NATE");

		 }
	 }
 */
	/*internal class FunDelegate : SystemFun {
		private System.Type type;
		private string name;

		public FunDelegate(System.Type type, string name) {
			this.type = type;
			this.name = name;
		}

		public override Value Run(Scope ex, params Value[] args) {
			System.Type[] ttt = new System.Type[args.Length];
			for (int i = 0; i < args.Length; i++)
				ttt[i] = args[i].GetType();

			var e = type.GetMethod(name, ttt);

			if (e == null)
				e = this.type.GetMethod(this.name, new System.Type[0]);

			if (e == null) {
				System.Type[] aa = new System.Type[ttt.Length - 1];
				for (int i = 1; i < ttt.Length; i++)
					aa[i - 1] = ttt[i];
				e = type.GetMethod(name, aa);
			}

			if (e.IsStatic) {
				var a = e.GetParameters();
				for (int i = 0; i < a.Length; i++) {
					if (a[i].ParameterType != args[i].GetType()) {
						if (a[i].ParameterType.IsAssignableFrom(args[i].GetType()))
							continue;
						throw new StandartLibrary.Exception("Функция " + e.Name + " ожидала аргумент типа " + a[i].ParameterType + " != " + args[i].GetType() + "; позиция: " + (i + 1) + "; имя аргумента: " + a[i].Name);
					}
				}

				var res = e.Invoke(null, args);
				if (res != null)
					return new Dynamic(res);
				else return new Null();
			}
			else {
				if (e.ReflectedType != args[0].GetType() && !(e.ReflectedType.IsAssignableFrom(args[0].GetType())))
					throw new StandartLibrary.Exception("Функция " + e.Name + " ожидала аргумент типа " + e.ReflectedType + " != " + args[0].GetType());

				object[] Args = new object[args.Length - 1];
				var x = e.GetParameters();

				for (int i = 1; i < args.Length; i++)
					if (x[i - 1].ParameterType != args[i].GetType()) {
						if (x[i - 1].ParameterType.IsAssignableFrom(args[i].GetType())) {
							Args[i - 1] = args[i];
							continue;
						}
						throw new StandartLibrary.Exception("Функция " + e.Name + " ожидала аргумент типа " + x[i - 1].ParameterType + " != " + args[i].GetType() + "; позиция: " + (i + 1) + "; имя аргумента: " + x[i - 1].Name);
					}
					else {
						Args[i - 1] = args[i];
					}

				var a = e.Invoke(args[0], Args);
				if (a != null)
					return new Dynamic(a);
				else return new Null();
			}
		}
	}
	/*
  internal class DynamicType : Value {
	  internal System.Type type;

	  KType Value.Type => throw new NotImplementedException();

	  public string GetDoc() {
		  return "";
	  }

	  public DynamicType(System.Type t) {
		  type = t;
	  }

	  public Value Clone() {
		  throw new NotImplementedException();
	  }

	  public dynamic Get(string Name) {
		  return 0;
		  /*
		  if (Type.IsEnum)
			  return new Dynamic(Type.GetField(Name).GetValue(Type));

		  if (Name == "()")
			  return new DFun((args) =>
			  {
				  Type[] Types = new Type[args.Length];

				  for (int i = 0; i < args.Length; i++)
					  Types[i] = args[i].GetType();

				  dynamic x;

				  x = Type.GetConstructor(Types);

				  x = x.Invoke(args);

				  return new Dynamic(x);
			  });

		  foreach (Type i in Type.GetNestedTypes())
			  if (i.Name == Name)
				  return new DynamicType(i);

		  try
		  {
			  var x = Type.GetMethod(Name);
			  if(x != null)
				  return new LFun((e, args) => {
					  object[] margs = new object[args.Length];
					  for (int i = 0; i < args.Length; i++)
						  margs[i] = C.Unpack(args[i]);
					  return new Dynamic(x.Invoke(null, margs));
				  });
			  else
			  {
				  var a = Type.GetProperty(Name);
				  if (a == null)
					  return new FunDelegate(Type, Name);
				  else return new Dynamic(a.GetValue(Type));
			  }
		  }
		  catch
		  {
			  var a = Type.GetProperty(Name);
			  if (a == null)
				  return new FunDelegate(Type, Name);
			  else return new Dynamic(a.GetValue(Type));
		  }
		  */
}