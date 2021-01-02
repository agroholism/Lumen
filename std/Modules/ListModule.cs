using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Lang.Patterns;

namespace Lumen.Lang {
	internal class ListModule : Module {
		public ListModule() {
			this.Name = "List";

			this.SetMember("<init>", new LambdaFun((scope, args) => {
				return new List(new LinkedList(scope["head"], scope["tail"].ToLinkedList(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("head"),
					new NamePattern("tail")
				}
			});

			static Int32 Index(Int32 index, Int32 count) {
				if (index < 0) {
					return count + index;
				}
				return index;
			}

			this.SetMember(Constants.PLUS, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<Value> values = scope["values"].ToSeq(scope);
				IEnumerable<Value> valuesx = scope["values'"].ToSeq(scope);

				return new List(values.Concat(valuesx));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});
		
			this.SetMember("empty", new LambdaFun((scope, args) => {
				return new List();
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});
			
			this.SetMember("pure", new LambdaFun((scope, args) => {
				return new List(scope["x"]);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("replicate", new LambdaFun((scope, args) => {
				return new List(Enumerable.Repeat(scope["init"], scope["len"].ToInt(scope)));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("init"),
					new NamePattern("len")
				}
			});

			this.SetMember("head", new LambdaFun((e, args) => {
				LinkedList v = e["l"].ToLinkedList(e);

				return v.Head == Const.UNIT ? Prelude.None : (Value)Helper.CreateSome(v.Head);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("l")
				}
			});
			this.SetMember("tail", new LambdaFun((e, args) => {
				LinkedList v = e["l"].ToLinkedList(e);

				return LinkedList.IsEmpty(v) ? Prelude.None : (Value)Helper.CreateSome(new List(v.Tail));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("l")
				}
			});

			this.SetMember("filter", new LambdaFun((scope, args) => {
				Fun mapper = scope["pred"].ToFunction(scope);
				IEnumerable<Value> values = scope["list"].ToSeq(scope);

				return new List(values.Where(i => mapper.Call(new Scope(scope), i).ToBoolean()));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("list"),
					new NamePattern("pred"),
				}
			});

			// Number -> List 'T -> List List 'T
			this.SetMember("chunkBySize", new LambdaFun((scope, args) => {
				Value[] firstList = scope["list"].ToLinkedList(scope).ToArray();
				Double size = scope["size"].ToDouble(scope);

				LinkedList result = new LinkedList();

				Int32 i = firstList.Count();
				while (i > 0) {
					LinkedList subResult = new LinkedList();
					for (Int32 j = 0; j < size; j++, i--) {
						subResult = new LinkedList(firstList[i - 1], subResult);
					}
					result = new LinkedList(new List(subResult), result);
				}

				return new List(result);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("list"),
					new NamePattern("size")
				}
			});

			this.SetMember("collect", new LambdaFun((scope, args) => {
				Value[] firstList = scope["list"].ToLinkedList(scope).ToArray();
				Fun fn = scope["fn"] as Fun;

				LinkedList result = new LinkedList();

				foreach (LinkedList i in firstList.Select(i => fn.Call(new Scope(scope), i).ToLinkedList(scope)).Reverse()) {
					foreach (Value j in i.Reverse()) {
						result = new LinkedList(j, result);
					}
				}

				return new List(result);
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("list"),
					new NamePattern("fn")
				}
			});

			this.SetMember("toText", new LambdaFun((e, args) => {
				return new Text(e["this"].ToString());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("this")
				}
			});
			this.SetMember("toSeq", new LambdaFun((e, args) => {
				return new Seq(e["this"].ToLinkedList(e));
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("this")
				}
			});
			this.SetMember("toArray", new LambdaFun((e, args) => {
				return new MutArray(e["l"].ToLinkedList(e).ToList());
			}) {
				Parameters = new List<IPattern> {
					new NamePattern("l")
				}
			});

			this.SetMember("fromSeq", new LambdaFun((scope, args) => {
				return new List(scope["x"].ToSeq(scope));
			}) {
				Name = "fromSeq",
				Parameters = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Parameters = Const.Self
			});

			this.SetMember("default", new LambdaFun((scope, args) => {
				return new List();
			}) {
				Parameters = new List<IPattern>()
			});

			this.AppendImplementation(Prelude.Default);
			this.AppendImplementation(Prelude.Clone);
			this.AppendImplementation(Prelude.Container);
		}
	}
}
