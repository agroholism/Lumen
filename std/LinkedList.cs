using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
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

		public override Boolean Equals(Object obj) {
			if(obj is LinkedList linkedList) {
				if (ReferenceEquals(this, linkedList)) {
					return true;
				}

				if(this.Count() != linkedList.Count()) {
					return false;
				}

				return this.Zip(linkedList, (x, y) => x.Equals(y)).All(x => x);
			}

			return false;
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
}
