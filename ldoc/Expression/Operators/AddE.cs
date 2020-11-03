using System.Collections.Generic;
using System.Threading.Tasks;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Collections;
using System;

namespace ldoc {
	internal class AddE : Expression {
		private Expression result;
		private Expression right;
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public AddE(Expression result, Expression right) {
			this.result = result;
			this.right = right;
		}

		public Expression Closure(ClosureManager manager) {
			return new AddE(this.result.Closure(manager), this.right.Closure(manager));
		}

		public Value Eval(Scope e) {
			Value right = this.right.Eval(e);

			if (right is List list) {
				return new List(new LinkedList(this.result.Eval(e), list.Value));
			}

			if (right is Lazy lazy) {
				Value v = this.result.Eval(e);
				return new LazyList { Current = v, Next = lazy };
				/*LinkedList l = lazy.Force() as LinkedList;
				return new List(new LinkedList(this.result.Eval(e), l));*/
			}

			if (right is LazyList lazylist) {
				Value v = this.result.Eval(e);
				return new LazyList { Current = v, Next = new Lazy(null) { value = lazylist } };
				/*LinkedList l = lazy.Force() as LinkedList;
				return new List(new LinkedList(this.result.Eval(e), l));*/
			}

			throw new Exception("wait a list ::");
		}
	}

	public class LazyList : IEnumerable<Value>, Value {
		public Value Current { get; set; }
		public Lazy Next { get; set; }

		public IType Type => Prelude.List;

		public Value Clone() {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public IEnumerator<Value> GetEnumerator() {
			yield return this.Current;

			Lazy next = this.Next;
			while (next.Force() is LazyList ll) {
				yield return ll.Current;
				next = ll.Next;
			}
		}

		public String ToString(Scope e) {
			return "[lazy]";
		}

		public String ToString(String format, IFormatProvider formatProvider) {
			return "[lazy]";
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}