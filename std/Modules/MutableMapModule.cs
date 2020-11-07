﻿using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class MutableMapModule : Module {
		internal MutableMapModule() {
			this.Name = "MutableMap";

			this.SetMember(Constants.GETI, new LambdaFun((scope, args) => {
				Dictionary<Value, Value> self = scope["self"].ToDictionary(scope);
				List<Value> indices = scope["indices"].ToList(scope);

				if (indices.Count == 1) {
					Value index = indices[0];

					if (self.TryGetValue(index, out Value result)) {
						return result;
					}
					else {
						throw new LumenException($"key '{index}' does not exists in map");
					}
				}

				throw new LumenException("function Map.getIndex supports only one argument");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("indices"),
					new NamePattern("self")
				}
			});

			this.SetMember("get", new LambdaFun((scope, args) => {
				Dictionary<Value, Value> self = scope["self"].ToDictionary(scope);

				if (self.TryGetValue(scope["key"], out Value result)) {
					return Helper.CreateSome(result);
				}

				return Prelude.None;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("key"),
					new NamePattern("self")
				}
			});

			this.SetMember(Constants.SETI, new LambdaFun((scope, args) => {
				Dictionary<Value, Value> self = scope["self"].ToDictionary(scope);
				List<Value> indices = scope["indices"].ToList(scope);

				if (indices.Count == 1) {
					self[indices[0]] = scope["value"];
					return Const.UNIT;
				}

				throw new LumenException("function Map.setIndex supports only one argument");
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("indices"),
					new NamePattern("value"),
					new NamePattern("self"),
				}
			});

			this.SetMember("<init>", new LambdaFun((e, args) => {
				Value value = e["initValue"];
				MutableMap result = new MutableMap();

				if (value == Const.UNIT) {
					return result;
				}

				foreach (Value i in value.ToStream(e)) {
					LinkedList stream = i.ToLinkedList(e);
					result.InternalValue[stream.Head] = stream.Tail.Head;
				}

				return result;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("initValue")
				}
			});

			this.SetMember("getValues", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((MutableMap)e.Get("m")).InternalValue;
				return new MutableArray(dict.Values.ToList());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m")
				}
			});

			this.SetMember("getKeys", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((MutableMap)e.Get("m")).InternalValue;
				return new MutableArray(dict.Keys.ToList());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				Dictionary<Value, Value> self = scope["self"].ToDictionary(scope);
				Value key = scope["key"];

				return new Logical(self.ContainsKey(key));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("key"),
					new NamePattern("self"),
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
				MutableMap result = new MutableMap();

				foreach (Value i in value.ToStream(e)) {
					LinkedList stream = i.ToLinkedList(e);
					result.InternalValue[stream.Head] = stream.Tail.Head;
				}

				return result;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("toStream", new LambdaFun((e, args) => {
				IDictionary<Value, Value> self = ((MutableMap)e["self"]).InternalValue;
				return new Stream(self.Select(Helper.CreatePair));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			/*this.Set("to_s", new LambdaFun((e, args) => {
                IDictionary<Value, Value> dict = Converter.ToMap(e.Get("this"), e);
                return new String("[" + String.Join(", ", dict) + "]");
            }));*/

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new MutableMap();
			}) {
				Parameters = new List<IPattern> { }
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Collection);
		}

		internal class PairModule : Module {
			public static IType ctor;

			public PairModule() {
				this.Name = "Map.Pair";

				ctor = Helper.CreateConstructor("Pair", this, new List<System.String> { "fst", "snd" });

				this.SetMember("init", new LambdaFun((scope, args) => {
					return Helper.CreatePair(scope["fst"], scope["snd"]);
				}) {
					Parameters = new List<IPattern> {
						new NamePattern("fst"),
						new NamePattern("snd")
					}
				});
			}
		}
	}
}