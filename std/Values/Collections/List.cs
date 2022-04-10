using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class List : BaseValueImpl {
		public LinkedList Value { get; private set; }

		public override IType Type => Prelude.List;

		public List() {
			this.Value = LinkedList.Empty;
		}

		public List(params IValue[] elements) {
			this.Value = LinkedList.Create(elements);
		}

		public List(IEnumerable<IValue> elements) {
			this.Value = LinkedList.Create(elements);
		}

		internal List(LinkedList value) {
			this.Value = value;
		}

		public override String ToString() {
			return "[" + String.Join(", ", this.Value) + "]";
		}

		public override Boolean Equals(Object obj) {
			if (obj is List list) {
				return this.Value.Equals(list.Value);
			}

			return false;
		}

		public override Int32 GetHashCode() {
			return -1584136870;
		}
	}

	public class LinkedList : IEnumerable<IValue> {
		public LinkedListNode First { get; private set; }

		public IValue Head => this.First.Value;
		public LinkedList Tail => new LinkedList(this.First.NextNode);

		public static LinkedList Empty { get; } = new LinkedList();

		public LinkedList() {
			this.First = new LinkedListNode(Const.UNIT, null);
		}

		public LinkedList(IValue value) {
			this.First = new LinkedListNode(value, null);
		}

		public LinkedList(IValue value, LinkedListNode nextNode) {
			this.First = new LinkedListNode(value, nextNode);
		}

		public LinkedList(LinkedListNode node) {
			this.First = new LinkedListNode(node.Value, node.NextNode);
		}

		public LinkedList(IValue head, LinkedList tail) {
			this.First = new LinkedListNode(head, tail.First);
		}

		public IEnumerator<IValue> GetEnumerator() {
			IEnumerable<IValue> Get() {
				LinkedListNode node = this.First;
				while (node.NextNode != null) {
					yield return node.Value;
					node = node.NextNode;
				}
			}

			return Get().GetEnumerator();
		}

		private void Add(IValue value) {
			this.First = new LinkedListNode(value, this.First);
		}

		public override Boolean Equals(Object obj) {
			if (obj is LinkedList linkedList) {
				if (ReferenceEquals(this, linkedList)) {
					return true;
				}

				if (this.Count() != linkedList.Count()) {
					return false;
				}

				return this.Zip(linkedList, (x, y) => x.Equals(y)).All(x => x);
			}

			return false;
		}

		public static LinkedList Map(Func<IValue, IValue> f, LinkedList list) {
			return Create(list.Select(f));
		}

		public static LinkedList Choose(Func<IValue, IValue> f, LinkedList list) {
			return Create(list.Select(f).Where(i => i != Prelude.None).Select(i => Prelude.DeconstructSome(i)));
		}

		public static LinkedList Append(LinkedList list1, LinkedList list2) {
			LinkedList result = new LinkedList {
				First = list2.First
			};

			foreach (IValue item in list1.Reverse()) {
				result.Add(item);
			}

			return result;
		}

		public static LinkedList Create(IEnumerable<IValue> components) {
			LinkedList result = new LinkedList();
			foreach (IValue i in components.Reverse()) {
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

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}

	public class LinkedListNode {
		public IValue Value { get; private set; }
		public LinkedListNode NextNode { get; private set; }

		public LinkedListNode(IValue value) {
			this.Value = value;
		}

		public LinkedListNode(IValue value, LinkedListNode nextNode) : this(value) {
			this.NextNode = nextNode;
		}
	}
}
