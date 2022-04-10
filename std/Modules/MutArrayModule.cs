﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class MutArrayModule : Module {
		public MutArrayModule() {
			this.Name = "MutArray";

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
				List<IValue> exemplare = e.Get("array").ToList(e);

				List<IValue> result = new List<IValue>();

				Int32 index = Index((Int32)e.Get("index").ToDouble(e), exemplare.Count);

				if (index < 0 || index >= exemplare.Count) {
					throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
				}

				exemplare[index] = e.Get("value");

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("array"),
					new NamePattern("index"),
					new NamePattern("value")
				}
			});
			#endregion

			this.SetMember("empty", new LambdaFun((scope, args) => {
				return new MutArray();
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});


			this.SetMember("replicate", new LambdaFun((scope, args) => {
				return new MutArray(Enumerable.Repeat(scope["init"], scope["len"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("len")
				}
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new MutArray();
			}) {
				Parameters = new List<IPattern> { }
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);

				List<IValue> clone = new List<IValue>();
				clone.AddRange(array);

				return new MutArray(clone);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("filter", new LambdaFun((scope, args) => {
				Fun predicate = scope["predicate"].ToFunction(scope);
				IEnumerable<IValue> values = scope["self"].ToFlow(scope);

				return new MutArray(values.Where(i => predicate.Call(new Scope(scope), i).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("predicate"),
					new ExactTypePattern("self", this),
				}
			});

			this.SetMember("add", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);

				array.Add(scope["element"]);

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("element"),
					new NamePattern("self"),
				}
			});

			this.SetMember("addAll", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);

				array.AddRange(scope["elements"].ToFlow(scope));

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
						new NamePattern("self"),
					new NamePattern("elements")
				}
			});

			this.SetMember("remove", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);
				IValue element = scope["element"];

				while (array.Contains(element)) {
					array.Remove(element);
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
				new NamePattern("self") , new NamePattern("element")
				}
			});

			this.SetMember("removeFirst", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);
				IValue element = scope["element"];

				array.Remove(element);

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self"), new NamePattern("element")
				}
			});

			this.SetMember("removeLast", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);
				IValue element = scope["element"];

				Int32 lastIndex = array.LastIndexOf(element);
				if (lastIndex > -1) {
					array.RemoveAt(lastIndex);
				}

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("element")
				}
			});

			this.SetMember("removeIf", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);

				Fun predicate = scope["predicate"].ToFunction(scope);
				array.RemoveAll(x => predicate.Call(new Scope(scope), x).ToBoolean());

				return Const.UNIT;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("predicate")
				}
			});

			this.SetMember("removeAt", new LambdaFun((scope, args) => {
				List<IValue> array = scope["self"].ToList(scope);

				Int32 predicate = scope["at"].ToInt(scope);
				var atval = array[predicate];
				array.RemoveAt(predicate);

				return atval;
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("at"),
					new NamePattern("self")
				}
			});

			this.SetMember("toText", new LambdaFun((e, args) => {
				return new Text(e["self"].ToString());
			}) {
				Parameters = Const.Self
			});

			this.SetMember("sort", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				value.Sort();

				return new MutArray(value);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("sortBy", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				Fun mutator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					IValue first = mutator.Call(new Scope(scope), i);
					IValue second = mutator.Call(new Scope(scope), j);
					return first.CompareTo(second);
				});

				return new MutArray(value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("other")
				}
			});

			this.SetMember("sortWith", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				Fun comparator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					return (Int32)Converter.ToDouble(comparator.Call(new Scope(scope), i, j), scope);
				});

				return new MutArray(value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("other")
				}
			});

			this.SetMember("sortDescending", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				value.Sort((i, j) => j.CompareTo(i));

				return new MutArray(value);
			}) {
				Parameters = Const.Self
			});

			this.SetMember("sortDescendingBy", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				Fun mutator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					IValue first = mutator.Call(new Scope(scope), i);
					IValue second = mutator.Call(new Scope(scope), j);
					return second.CompareTo(first);
				});

				return new MutArray(value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("other")
				}
			});

			this.SetMember("sortDescendingWith", new LambdaFun((scope, args) => {
				List<IValue> value = new List<IValue>();
				value.AddRange(scope["self"].ToList(scope));

				Fun comparator = scope["other"].ToFunction(scope);
				value.Sort((i, j) => {
					return -(Int32)Converter.ToDouble(comparator.Call(new Scope(scope), i, j), scope);
				});

				return new MutArray(value);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("other")
				}
			});

			this.SetMember("contains", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);
				return new Logical(self.Contains(scope["elem"]));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("self") , new NamePattern("elem")
				}
			});

			this.SetMember("indexOf", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);
				Int32 result = self.IndexOf(scope["other"]);

				return result == -1 ? Prelude.None : (IValue)Helper.CreateSome((Number)result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("lastIndexOf", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);
				Int32 result = self.LastIndexOf(scope["other"]);

				return result == -1 ? Prelude.None : (IValue)Helper.CreateSome((Number)result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("findIndex", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Int32 result = self.FindIndex(i => other.Call(new Scope(scope), i).ToBoolean());

				return result == -1 ? Prelude.None : (IValue)Helper.CreateSome((Number)result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("find", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				IValue result = self.Find(i => other.Call(new Scope(scope), i).ToBoolean());

				return result == null ? Prelude.None : (IValue)Helper.CreateSome(result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("findLast", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				IValue result = self.FindLast(i => other.Call(new Scope(scope), i).ToBoolean());

				return result == null ? Prelude.None : (IValue)Helper.CreateSome(result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("findLastIndex", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);

				Fun other = scope["other"].ToFunction(scope);

				Int32 result = self.FindLastIndex(i => other.Call(new Scope(scope), i).ToBoolean());

				return result == -1 ? Prelude.None : (IValue)Helper.CreateSome((Number)result);
			}) {
				Parameters = Const.SelfOther
			});

			this.SetMember("clear", new LambdaFun((scope, args) => {
				List<IValue> self = scope["self"].ToList(scope);
				self.Clear();
				return Const.UNIT;
			}) {
				Parameters = Const.Self
			});

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				return new MutArray(scope["x"].ToFlow(scope));
			}) {
				Name = "fromSeq",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("toSeq", new LambdaFun((e, args) => {
				return new Flow(e["array"].ToList(e));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("array")
				}
			});

			this.AppendImplementation(Prelude.Default);
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
