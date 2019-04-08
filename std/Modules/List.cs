using Lumen.Lang.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
    internal class ListModule : Module {
        public ListModule() {
            this.name = "prelude.List";

            LinkedList Flatten(IEnumerable<Value> list, Scope s) {
                LinkedList result = new LinkedList();

                foreach (Value i in list.Reverse()) {
                    foreach (Value j in i.ToLinkedList(s).Reverse()) {
                        result = new LinkedList(j, result);
                    }
                }

                return result;
            }

            #region operators
            // List 'T -> List 'U -> List 'R
            this.SetField(Op.PLUS, new LambdaFun((scope, args) => {
                LinkedList firstList = scope["xs"].ToLinkedList(scope);
                LinkedList secondList = scope["xs'"].ToLinkedList(scope);

                return new List(LinkedList.Append(firstList, secondList));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("xs"),
                    new NamePattern("xs'")
                }
            });
            #endregion

            // Averagable 'T => List 'T -> 'T
            this.SetField("average", new LambdaFun((scope, args) => {
                LinkedList firstList = scope["this"].ToLinkedList(scope);

                Value sum = firstList.Aggregate((x, y) => {
                    return (x.Type.GetField(Op.PLUS, scope) as Fun).Run(scope, x, y);
                });

                return (sum.Type.GetField(Op.SLASH, scope) as Fun).Run(scope, sum, new Number(firstList.Count()));
            }) {
                Arguments = Const.This
            });

			this.SetField(Op.STAR, new LambdaFun((scope, args) => {
				LinkedList firstList = scope["this"].ToLinkedList(scope);

				return new Text(String.Join(scope["other"].ToString(scope), firstList));
			}) {
				Arguments = Const.ThisOther
			});

			// Number -> List 'T -> List List 'T
			this.SetField("chunkBySize", new LambdaFun((scope, args) => {
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

            this.SetField("collect", new LambdaFun((scope, args) => {
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

            #region deriving
            // ('T -> 'U) -> List 'T -> List 'U
            LambdaFun fmap = new LambdaFun((e, args) => {
                Fun f = (Fun)e["fn"];
                LinkedList v = e["fc"].ToLinkedList(e);

                return new List(LinkedList.Map(i => f.Run(new Scope(e), i), v));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("fn"),
                    new NamePattern("fc"),
                }
            };

            this.SetField("map", fmap);

            // Functor
            this.SetField("fmap", fmap);

            // Applicative
            this.SetField("liftA", new LambdaFun((scope, args) => {
                Value obj = scope["f"];

                return new List(
                    Flatten(obj.ToLinkedList(scope).Select(i => 
                    fmap.Run(new Scope(scope), i, scope["m"])), scope));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("m"),
                }
            });

            #endregion

            this.SetField("filter", new LambdaFun((e, args) => {
                UserFun f = (UserFun)e["f"];
                Value l = e["xs"];
                LinkedList v = l.ToLinkedList(e);

                return new List(LinkedList.Create(v.Where(i => f.Run(new Scope(e), i).ToBoolean())));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("f"),
                    new NamePattern("xs")
                }
            });

            this.SetField("head", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);

                return v.Head;
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });
            this.SetField("tail", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);

                return new List(v.Tail);
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });

            this.SetField("sort", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                IOrderedEnumerable<Value> res = v.OrderBy(x => x);
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });
            this.SetField("sortBy", new LambdaFun((e, args) => {
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

            this.SetField("sortDescending", new LambdaFun((e, args) => {
                LinkedList v = e["l"].ToLinkedList(e);
                IOrderedEnumerable<Value> res = v.OrderByDescending(x => x);
                return new List(LinkedList.Create(res));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });
            this.SetField("sortByDescending", new LambdaFun((e, args) => {
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

            this.SetField("toText", new LambdaFun((e, args) => {
                return new Text(e["this"].ToString(e));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            });
            this.SetField("toSequence", new LambdaFun((e, args) => {
                return new Enumerator(e["this"].ToLinkedList(e));
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("this")
                }
            });
            this.SetField("toArray", new LambdaFun((e, args) => {
                return new Array(e["l"].ToLinkedList(e).ToList());
            }) {
                Arguments = new List<IPattern> {
                    new NamePattern("l")
                }
            });

            this.Derive(Prelude.Sequence);
            this.Derive(Prelude.Functor);
            this.Derive(Prelude.Applicative);
        }
    }

    public class LinkedListNode {
        public Value Value { get; private set; }
        public LinkedListNode NextNode { get; private set; }

        public LinkedListNode(Value value) {
            this.Value = value;
        }

        public LinkedListNode(Value value, LinkedListNode nextNode) : this(value) {
            this.NextNode = nextNode;
        }
    }

    public class LinkedList : IEnumerable<Value> {
        public LinkedListNode First { get; private set; }

        public Value Head => this.First.Value;
        public LinkedList Tail => new LinkedList(this.First.NextNode);

        public static LinkedList Empty { get; } = new LinkedList();

        public LinkedList() {
            this.First = new LinkedListNode(Const.UNIT, null);
        }

        public LinkedList(Value value) {
            this.First = new LinkedListNode(value, null);
        }

        public LinkedList(Value value, LinkedListNode nextNode) {
            this.First = new LinkedListNode(value, nextNode);
        }

        public LinkedList(LinkedListNode node) {
            this.First = new LinkedListNode(node.Value, node.NextNode);
        }

        public LinkedList(Value head, LinkedList tail) {
            this.First = new LinkedListNode(head, tail.First);
        }

        public IEnumerator<Value> GetEnumerator() {
            IEnumerable<Value> Get() {
                LinkedListNode node = this.First;
                while (node.NextNode != null) {
                    yield return node.Value;
                    node = node.NextNode;
                }
            }

            return Get().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        private void Add(Value value) {
            this.First = new LinkedListNode(value, this.First);
        }

        public static LinkedList Map(Func<Value, Value> f, LinkedList list) {
            return Create(list.Select(f));
        }

        public static LinkedList Choose(Func<Value, Value> f, LinkedList list) {
            return Create(list.Select(f).Where(i => i != Prelude.None).Select(i => Prelude.DeconstructSome(i, null)));
        }

        public static LinkedList Append(LinkedList list1, LinkedList list2) {
            LinkedList result = new LinkedList {
                First = list2.First
            };

            foreach (Value item in list1.Reverse()) {
                result.Add(item);
            }

            return result;
        }

        public static LinkedList Create(IEnumerable<Value> components) {
            LinkedList result = new LinkedList();
            foreach (Value i in components.Reverse()) {
                result.Add(i);
            }
            return result;
        }

        public static Boolean IsEmpty(LinkedList list) {
            return list.First.NextNode == null;
        }

        public static Int32 Count(LinkedList list) {
            return list.Count();
        }
    }
}
