using Lumen.Lang.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	internal class ListModule : Module {
		public ListModule() {
			this.Name = "List";

			this.SetMember("init", new LambdaFun((scope, args) => {
				return new List(new LinkedList(scope["head"], scope["tail"].ToLinkedList(scope)));
			}) {
				Arguments = new List<IPattern> {
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

			this.SetMember(Op.GETI, new LambdaFun((scope, args) => {
				IEnumerable<Value> values = scope["values"].ToStream(scope);

				Value index = scope["indices"];

				if (index is Fun fun) {
					return new List(values.Where(x => fun.Run(new Scope(scope), x).ToBoolean()));
				}

				Int32 count = values.Count();

				if (index is Number) {
					Int32 intIndex = Index(index.ToInt(scope), count);

					if (intIndex < 0 || intIndex >= count) {
						throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
					}

					return values.ElementAt(intIndex);
				}

				return new List(index.ToStream(scope).Select(i => {
					if (i is Number) {
						Int32 index = Index(i.ToInt(scope), count);

						if (index < 0 || index >= count) {
							throw new LumenException(Exceptions.INDEX_OUT_OF_RANGE);
						}

						return values.ElementAt(index);
					}
					else {
						throw new LumenException(Exceptions.TYPE_ERROR.F(Prelude.Number, i.Type));
					}
				}));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("indices")
				}
			});

			this.SetMember(Op.PLUS, new LambdaFun((scope, args) => {
				IType typeParameter = scope["values"].Type;
				IEnumerable<Value> values = scope["values"].ToStream(scope);
				IEnumerable<Value> valuesx = scope["values'"].ToStream(scope);

				return new List(values.Concat(valuesx));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("values"),
					new NamePattern("values'")
				}
			});

			this.SetMember("head", new LambdaFun((e, args) => {
				LinkedList v = e["l"].ToLinkedList(e);

				return v.Head == Const.UNIT ? Prelude.None : (Value)Helper.CreateSome(v.Head);
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("l")
				}
			});
			this.SetMember("tail", new LambdaFun((e, args) => {
				LinkedList v = e["l"].ToLinkedList(e);

				return LinkedList.IsEmpty(v) ? Prelude.None : (Value)Helper.CreateSome(new List(v.Tail));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("l")
				}
			});

			this.SetMember("filter", new LambdaFun((scope, args) => {
				Fun mapper = scope["pred"].ToFunction(scope);
				IEnumerable<Value> values = scope["list"].ToStream(scope);

				return new List(values.Where(i => mapper.Run(new Scope(scope), i).ToBoolean()));
			}) {
				Arguments = new List<IPattern> {
					new NamePattern("pred"),
					new NamePattern("list"),
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
                Arguments = new List<IPattern> {
                    new NamePattern("size"),
                    new NamePattern("list")
                }
            });

            this.SetMember("collect", new LambdaFun((scope, args) => {
                Value[] firstList = scope["list"].ToLinkedList(scope).ToArray();
                Fun fn = scope["fn"] as Fun;

                LinkedList result = new LinkedList();

                foreach (LinkedList i in firstList.Select(i => fn.Run(new Scope(scope), i).ToLinkedList(scope)).Reverse()) {
                    foreach (Value j in i.Reverse()) {
                        result = new LinkedList(j, result);
                    }
                }

                return new List(result);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("list"),
                    new NamePattern("fn")
                }
            });

            this.SetMember("sort", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                IOrderedEnumerable<Value> res = v.OrderBy(x => x);
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });

			this.SetMember("sortBy", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                Fun f = e["f"] as Fun;
                IOrderedEnumerable<Value> res = v.OrderBy(x => f.Run(new Scope(e), x));
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l"),
                    new NamePattern("f")
                }
            });

            this.SetMember("sortDescending", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                IOrderedEnumerable<Value> res = v.OrderByDescending(x => x);
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });

			this.SetMember("sortByDescending", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                Fun f = e["f"] as Fun;
                IOrderedEnumerable<Value> res = v.OrderByDescending(x => f.Run(new Scope(e), x));
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l"),
                    new NamePattern("f")
                }
            });

			this.SetMember("toText", new LambdaFun((e, args) => {
                return new Text(e["this"].ToString());
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            });
            this.SetMember("toStream", new LambdaFun((e, args) => {
                return new Stream(e["this"].ToLinkedList(e));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            });
            this.SetMember("toArray", new LambdaFun((e, args) => {
                return new Array(e["l"].ToLinkedList(e).ToList());
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });

			this.SetMember("fromStream", new LambdaFun((scope, args) => {
				return new List(scope["x"].ToStream(scope));
			}) {
				Name = "fromStream",
				Arguments = new List<IPattern> {
					new NamePattern("x")
				}
			});

			this.SetMember("clone", new LambdaFun((scope, args) => {
				return scope["self"];
			}) {
				Arguments = Const.Self
			});

			this.IncludeMixin(Prelude.Cloneable);
			this.IncludeMixin(Prelude.Collection);
        }
    }
}
