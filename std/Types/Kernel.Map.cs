using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumen.Lang.Std {
	internal class MapClass : Record {
		internal MapClass() {
			this.meta = new TypeMetadata {
				Name = "Kernel.Map",
				//BaseType = StandartModule.Object
			};

			SetAttribute("[]", new LambdaFun((e, args) => {
				Record sub = args[0] as Record;
				Record V = args[1] as Record;
				Record type = new Record {
					//type.meta.BaseType = this;
					meta = new TypeMetadata {
						Name = "Kernel.Map[" + args[0] + "]"
					}
				};
				type.SetAttribute("initialize", new LambdaFun((scope, argsx) => {
				//	(scope.Get("this") as IObject).Set("_dictionary", new Map(new Dictionary<Value, Value>()), AccessModifiers.PRIVATE, scope);
					return Const.VOID;
				}));
				/*type.Set("new", new LambdaFun((scope, argsx) => {
					var exe = new Object1(type);
					exe.element = new Map(new Dictionary<Value, Value>());
					return exe;
				}));*/
			/*	type.SetAttribute("to_m", new LambdaFun((scope, argsx) => {
					return (scope["this"] as IObject).Get("_dictionary", AccessModifiers.PRIVATE, scope);
				}));*/
				SetAttribute("[]", new LambdaFun((ex, argsx) => {
					IDictionary<Value, Value> dict = Converter.ToMap(ex.Get("this"), ex);
					if (dict.TryGetValue(args[0], out Value result)) {
						return result;
					}
					throw new Exception("данный ключ отсутствует в словаре", stack: e);
				}));
				SetAttribute("add!", new LambdaFun((ex, argsx) => {
					IDictionary<Value, Value> dict = Converter.ToMap(ex.Get("this"), ex);
					if (argsx[0].Type != sub || argsx[1].Type != V) {
						throw new Exception("bad type", stack: e);
					}

					dict.Add(argsx[0], argsx[1]);

					return Const.VOID;
				}));
				return type;
			}));

			SetAttribute("[]", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				if (dict.TryGetValue(args[0], out Value result)) {
					return result;
				}
				throw new Exception("данный ключ отсутствует в словаре", stack: e);
			}));

			SetAttribute("get_keys", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				return new Vec(dict.Keys.ToList());
			}));

			SetAttribute("get_values", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				return new Vec(dict.Values.ToList());
			}));

			LambdaFun to_l = new LambdaFun((e, args) => {
				Expando obj = e.Get("this") as Expando;
				List<Value> result = new List<Value> {
					obj.Get("key", AccessModifiers.PUBLIC, e),
					obj.Get("value", AccessModifiers.PUBLIC, e)
				};
				return new Vec(result);
			});


			SetAttribute("to_i", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				return new Enumerator(dict.Select(x => new Expando { ["key", e] = x.Key, ["value", e] = x.Value, ["to_l", e] = to_l }));
			}));

			SetAttribute("to_s", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = Converter.ToMap(e.Get("this"), e);
				return new Str("[" + String.Join(", ", dict) + "]");
			}));
			SetAttribute("contains", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				return (Bool)dict.ContainsKey(args[0]);
			}));
			SetAttribute("get?", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("this")).value;
				IObject obj = args[1] as IObject;
				if (dict.TryGetValue(args[0], out Value res)) {
					(obj.Get("=", e) as Fun).Run(new Scope(e), res);
					return (Bool)true;
				}
				(obj.Get("=", e) as Fun).Run(new Scope(e), Const.VOID);
				return (Bool)false;
			}));
		}
	}
}
