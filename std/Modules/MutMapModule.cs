using System.Collections.Generic;
using System.Linq;

using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class MutMapModule : Type {
		internal MutMapModule() : base("MutMap") {
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
				MutMap result = new MutMap();

				if (value == Const.UNIT) {
					return result;
				}

				foreach (Value i in value.ToSeq(e)) {
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
				IDictionary<Value, Value> dict = ((MutMap)e.Get("m")).InternalValue;
				return new MutArray(dict.Values.ToList());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("m")
				}
			});

			this.SetMember("getKeys", new LambdaFun((e, args) => {
				IDictionary<Value, Value> dict = ((MutMap)e.Get("m")).InternalValue;
				return new MutArray(dict.Keys.ToList());
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

			this.SetMember("fromSeq", new LambdaFun((e, args) => {
				Value value = e["stream"];
				MutMap result = new MutMap();

				foreach (Value i in value.ToSeq(e)) {
					LinkedList stream = i.ToLinkedList(e);
					result.InternalValue[stream.Head] = stream.Tail.Head;
				}

				return result;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("stream")
				}
			});

			this.SetMember("toSeq", new LambdaFun((e, args) => {
				IDictionary<Value, Value> self = ((MutMap)e["self"]).InternalValue;
				return new Seq(self.Select(Helper.CreatePair));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self")
				}
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new MutMap();
			}) {
				Parameters = new List<IPattern> { }
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Collection);
		}
	}
}
