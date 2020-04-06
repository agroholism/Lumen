using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System.Linq;
using System;

namespace ldoc {
	internal class HeadTailPattern : IPattern {
		private String xName;
		private String xsName;
		public Boolean IsNotEval => false;
		public HeadTailPattern(String xName, String xsName) {
			this.xName = xName;
			this.xsName = xsName;
		}
		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}
		public Expression Closure(ClosureManager manager) {
			manager.Declare(new List<String> { this.xName, this.xsName });

			return this;
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public List<String> GetDeclaredVariables() {
			return new List<String> { this.xName, this.xsName };
		}

		public MatchResult Match(Value value, Scope scope) {
			List list = null;

			if (value is Lazy lazy) {
				Value val = lazy.Force();
				if (val is List) {
					list = val as List;
				}
				else if (val is LazyList lll) {
					scope.Bind(this.xName, lll.Current);
					scope.Bind(this.xsName, lll.Next);
					return MatchResult.True;
				}
				else {
					return new MatchResult {
						Success = false,
						Note = $"function wait a non empty List"
					};
				}
			}

			if (value is LazyList ll) {
				scope.Bind(this.xName, ll.Current);
				scope.Bind(this.xsName, ll.Next);
				return MatchResult.True;
			}

			if (value is List) {
				list = value as List;
			}

			if (list == null || LinkedList.IsEmpty(list.value)) {
				return new MatchResult {
					Success = false,
					Note = "function wait a non empty List"
				};
			}
			else {
				scope.Bind(this.xName, list.value.Head);
				scope.Bind(this.xsName, new List(list.value.Tail));
				return MatchResult.True;
			}
		}

		public override String ToString() {
			return $"{this.xName}::{this.xsName}";
		}
	}
}