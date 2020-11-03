using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	internal class ArrayModule : Module {
		public ArrayModule() {
			this.Name = "Array";

			/*
Array.add+2404
Array.addAt
Array.addAll+2404
Array.addAllAt
Array.addUnique
Array.prepend
Array.prependAll
Array.remove+2404
Array.removeFirst+2404
Array.removeLast+2404
Array.removeIf+2404
Array.removeAt
Array.removeAll

Array.refs
			 */

			#region operators
			this.SetMember(Constants.SETI, new LambdaFun((e, args) => {
				List<Value> exemplare = e.Get("array").ToList(e);

				List<Value> result = new List<Value>();

				Int32 index = Index((Int32)e.Get("index").ToDouble(e), exemplare.Count);

				if (index < 0 || index >= exemplare.Count) {
					throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
				}

				exemplare[index] = e.Get("value");

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("array"),
					new NamePattern("index"),
					new NamePattern("value")
				}
			});
			#endregion

			this.SetMember("clone", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);

				List<Value> clone = new List<Value>();
				clone.AddRange(array);

				return new Array(clone);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("add", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);

				array.Add(scope["element"]);

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element"),
					new NamePattern("self")
				}
			});

			this.SetMember("addAll", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);

				array.AddRange(scope["elements"].ToStream(scope));

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elements"),
					new NamePattern("self")
				}
			});

			this.SetMember("remove", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);
				Value element = scope["element"];

				while (array.Contains(element)) {
					array.Remove(element);
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
				new NamePattern("element"),
					new NamePattern("self")
				}
			});

			this.SetMember("removeFirst", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);
				Value element = scope["element"];

				array.Remove(element);

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element") ,
					new NamePattern("self")
				}
			});

			this.SetMember("removeLast", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);
				Value element = scope["element"];

				Int32 lastIndex = array.LastIndexOf(element);
				if (lastIndex > -1) {
					array.RemoveAt(lastIndex);
				}

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("element"),
					new NamePattern("self")
				}
			});

			this.SetMember("removeIf", new LambdaFun((scope, args) => {
				List<Value> array = scope["self"].ToList(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);
				array.RemoveAll(x => predicate.Run(new Scope(scope), x).ToBoolean());

				return Const.UNIT;
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("predicate"),
					new NamePattern("self")
				}
			});

			this.SetMember("toText", new LambdaFun((e, args) => {
				return new Text(e["self"].ToString());
			}) {
				Arguments = Const.Self
			});

			this.SetMember("sort", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				value.Sort();

				return new Array(value);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("sortBy", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				Fun mutator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					Value first = mutator.Run(new Scope(scope), i);
					Value second = mutator.Run(new Scope(scope), j);
					return first.CompareTo(second);
				});

				return new Array(value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new NamePattern("self")
				}
			});

			this.SetMember("sortWith", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				Fun comparator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					return (Int32)Converter.ToDouble(comparator.Run(new Scope(scope), i, j), scope);
				});

				return new Array(value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new NamePattern("self")
				}
			});

			this.SetMember("sortDescending", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				value.Sort((i, j) => j.CompareTo(i));

				return new Array(value);
			}) {
				Arguments = Const.Self
			});

			this.SetMember("sortDescendingBy", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				Fun mutator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					Value first = mutator.Run(new Scope(scope), i);
					Value second = mutator.Run(new Scope(scope), j);
					return second.CompareTo(first);
				});

				return new Array(value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new NamePattern("self")
				}
			});

			this.SetMember("sortDescendingWith", new LambdaFun((scope, args) => {
				List<Value> value = new List<Value>();
				value.AddRange(scope["self"].ToList(scope));

				Fun comparator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					return -(Int32)Converter.ToDouble(comparator.Run(new Scope(scope), i, j), scope);
				});

				return new Array(value);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("other"),
					new NamePattern("self")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);
				return new Logical(self.Contains(scope["elem"]));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("elem"),
					new NamePattern("self")
				}
			});

			this.SetMember("indexOf", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);
				Int32 result = self.IndexOf(scope["other"]);

				return result == -1 ? Prelude.None : (Value)Helper.CreateSome((Number)result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("lastIndexOf", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);
				Int32 result = self.LastIndexOf(scope["other"]);

				return result == -1 ? Prelude.None : (Value)Helper.CreateSome((Number)result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("findIndex", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Int32 result = self.FindIndex(i => other.Run(new Scope(scope), i).ToBoolean());

				return result == -1 ? Prelude.None : (Value)Helper.CreateSome((Number)result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("find", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Value result = self.Find(i => other.Run(new Scope(scope), i).ToBoolean());

				return result == null ? Prelude.None : (Value)Helper.CreateSome(result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("findLast", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Value result = self.FindLast(i => other.Run(new Scope(scope), i).ToBoolean());

				return result == null ? Prelude.None : (Value)Helper.CreateSome(result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("findLastIndex", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Int32 result = self.FindLastIndex(i => other.Run(new Scope(scope), i).ToBoolean());

				return result == -1 ? Prelude.None : (Value)Helper.CreateSome((Number)result);
			}) {
				Arguments = Const.SelfOther
			});

			this.SetMember("clear", new LambdaFun((scope, args) => {
				List<Value> self = scope["self"].ToList(scope);
				self.Clear();
				return Const.UNIT;
			}) {
				Arguments = Const.Self
			});

			this.SetMember("fromStream", new LambdaFun((scope, args) => {
				return new Array(scope["x"].ToStream(scope));
			}) {
				Name = "fromStream",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("toStream", new LambdaFun((e, args) => {
				return new Stream(e["array"].ToList(e));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("array")
				}
			});

			this.AppendImplementation(Prelude.Collection);
		}

		private static Int32 Index(Int32 index, Int32 count) {
			if (index < 0) {
				return count + index;
			}
			return index;
		}
	}
}
