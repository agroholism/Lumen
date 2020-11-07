using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;
using System.Linq;

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
			List list = value as List;

			if (list == null || LinkedList.IsEmpty(list.Value)) {
				return new MatchResult (
					MatchResultKind.Fail,
					"not empty list expected, given " + 
						(list == null ? $"value of type {GetModule(value.Type)}" 
						: "[]")
				);
			}
			else {
				MatchResult x = this.xName.Match(list.Value.Head, scope);
				if (!x.IsSuccess) {
					return x;
				}

				return this.xsName.Match(new List(list.Value.Tail), scope);
			}
		}

		public Module GetModule(IType obj) {
			if (obj is Module m) {
				return m;
			}

			if (obj is Instance instance) {
				return (instance.Type as Constructor).Parent as Module;
			}

			if (obj is Constructor constructor) {
				return constructor.Parent;
			}

			if (obj is SingletonConstructor singleton) {
				return singleton.Parent;
			}

			return this.GetModule(obj.Type);
		}

		public override String ToString() {
			return $"{this.xName}::{this.xsName}";
		}
	}
}