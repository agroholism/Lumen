using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class MapModule : Module {
		internal MapModule() {
			this.Name = "Kernel.Map";

			this.SetMember("get", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e["self"]).value;
				if (dict.TryGetValue(e["key"], out Value result)) {
					return Helper.CreateSome(result);
				}
				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("key")
				}
			});

			this.SetMember(Op.GETI, new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e["self"]).value;
				if (dict.TryGetValue(e["key"].ToList(e)[0], out Value result)) {
					return result;
				}
				throw new LumenException("данный ключ отсутствет в словаре");
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("key")
				}
			});

			this.SetMember(Op.SETI, new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("self")).value;

				dict[e["key"]] = e["value"];

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("self"),
					new NamePattern("key"),
					new NamePattern("value"),
				}
			});

			this.SetMember("init", new LambdaFun((e, args) => {
				Value value = e["init"];
				Map result = new Map();

				if (value.Type.HasMixin(Prelude.Collection)) {
					foreach (Value i in value.ToStream(e)) {
						if (i is Instance ins && ins.Type == PairModule.ctor) {
							result.value[ins.items[0]] = ins.items[1];
						}
						else {
							var stream = i.ToStream(e);
							var key = stream.First();
							var val = stream.Last();
							result.value[key] = val;
						}
					}
				}

				return result;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("init")
				}
			});

			this.SetMember("getValues", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("m")).value;
				return new Array(dict.Values.ToList());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m")
				}
			});

			this.SetMember("getKeys", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e.Get("m")).value;
				return new Array(dict.Keys.ToList());
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m")
				}
			});

			/* LambdaFun to_l = new LambdaFun((e, args) => {
				 Expando obj = e.Get("this") as Expando;
				 List<Value> result = new List<Value> {
					 obj.Get("key", AccessModifiers.PUBLIC, e),
					 obj.Get("value", AccessModifiers.PUBLIC, e)
				 };
				 return new Array(result);
			 });
			 */

			this.SetMember("fromStream", new LambdaFun((e, args) => {
				Value value = e["stream"];
				Map result = new Map();

				if (value.Type.HasMixin(Prelude.Collection)) {
					foreach (var i in value.ToStream(e)) {
						if (i is Instance ins && ins.Type == PairModule.ctor) {
							result.value[ins.items[0]] = ins.items[1];
						}
						else {
							var stream = i.ToStream(e);
							var key = stream.First();
							var val = stream.Last();
							result.value[key] = val;
						}
					}
				}

				return result;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("toStream", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((Map)e["m"]).value;
				return new Stream(dict.Select(x => Helper.CreatePair(x)));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m")
				}
			});

			/*this.Set("to_s", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = Converter.ToMap(e.Get("this"), e);
                return new String("[" + String.Join(", ", dict) + "]");
            }));*/
			this.SetMember("contains", new LambdaFun((e, args) => {
				Value firstArg = e["m"];
				Value secondArg = e["key"];

				if (firstArg is Map map) {
					return (Bool)map.value.ContainsKey(secondArg);
				}
				else if (secondArg is Map map1) {
					return (Bool)map1.value.ContainsKey(firstArg);
				}

				return Const.FALSE;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("m"),
					new NamePattern("key"),
				}
			});

			this.IncludeMixin(Prelude.Collection);
		}

		internal class PairModule : Module {
			public static IType ctor;

			public PairModule() {
				this.Name = "Map.Pair";

				ctor = Helper.CreateConstructor("Pair", this, new List<string> { "fst", "snd" });

				this.SetMember("init", new LambdaFun((scope, args) => {
					return Helper.CreatePair(scope["fst"], scope["snd"]);
				}) {
					Arguments = new List<IPattern> {
						new NamePattern("fst"),
						new NamePattern("snd")
					}
				});
			}
		}
	}
}
