using System.Collections.Generic;
using Lumen.Lang.Expressions;
using Lumen.Lang;
using System;

namespace Lumen.Lang.Patterns {
	internal class HeadTailPattern : IPattern {
		private IPattern xName;
		private IPattern xsName;

		public HeadTailPattern(IPattern xName, IPattern xsName) {
			this.xName = xName;
			this.xsName = xsName;
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

		public List<String> GetDeclaredVariables() {
			List<String> result = this.xName.GetDeclaredVariables();
			result.AddRange(this.xsName.GetDeclaredVariables());
			return result;
		}

		public IPattern Closure(ClosureManager manager) {
			manager.Declare(this.GetDeclaredVariables());

			return this;
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