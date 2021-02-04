using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class Class : Module, IMutableType {
		public Dictionary<String, Expression> Declarations { get; private set; }
		protected List<Class> Implementations { get; private set; }

		public IEnumerable<String> AvailableNames => throw new NotImplementedException();

		public Class(String name) : base(name) {
			this.Declarations = new Dictionary<String, Expression>();
			this.Implementations = new List<Class>();
		}

		public virtual void OnImplement(Module target) {
			Scope scope = new Scope {
				[this.Name] = this
			};

			foreach (KeyValuePair<String, Expression> decl in this.Declarations) {
				scope[decl.Key] = target;
				decl.Value.Eval(scope);
			}

			foreach (KeyValuePair<String, Value> member in scope.variables) {
				if (member.Value != this && member.Value != target) {
					target.SetMemberIfAbsent(member.Key, member.Value);
				}
			}
		}

		public Boolean IsParentOf(Value value) {
			return value.Type.HasImplementation(this);
		}

		public Boolean HasImplementation(Class typeClass) {
			throw new NotImplementedException();
		}

		public void AppendImplementation(Class cls) {
			throw new NotImplementedException();
		}
	}

	public class SystemClass : Class {
		public SystemClass(String name) : base(name) {

		}

		public override void OnImplement(Module target) {
			
		}
	}
}
