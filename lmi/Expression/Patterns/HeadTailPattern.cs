using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Lmi {
	internal class HeadTailPattern : IPattern {
		private IPattern xName;
		private IPattern xsName;

		public Boolean IsNotEval => false;

		public HeadTailPattern(IPattern xName, IPattern xsName) {
			this.xName = xName;
			this.xsName = xsName;
		}

		public Expression Closure(ClosureManager manager) {
			manager.Declare(this.GetDeclaredVariables());

			return this;
		}

		public Value Eval(Scope e) {
			throw new NotImplementedException();
		}

		public IEnumerable<Value> EvalWithYield(Scope scope) {
			this.Eval(scope);
			yield break;
		}

		public List<String> GetDeclaredVariables() {
			List<String> result = this.xName.GetDeclaredVariables();
			result.AddRange(this.xsName.GetDeclaredVariables());
			return result;
		}

		public MatchResult Match(Value value, Scope scope) {
			List list = null;

			if (value is List) {
				list = value as List;
			}

			if (list == null || LinkedList.IsEmpty(list.Value)) {
				return new MatchResult {
					Success = false,
					Note = "function wait a non empty List"
				};
			}
			else {
				MatchResult x = this.xName.Match(list.Value.Head, scope);
				if (!x.Success) {
					return x;
				}

				return this.xsName.Match(new List(list.Value.Tail), scope);
			}
		}

		public override String ToString() {
			return $"{this.xName}::{this.xsName}";
		}
	}
}