using System;
using System.Collections.Generic;

using Lumen.Lang.Expressions;

namespace Lumen.Lang {
	public class Class : Module {
		public String TypeParameter { get; protected set; } = "T";
		public List<Expression> Declarations { get; } = new List<Expression>();

		public Class() : base() {

		}

		public Class(String name, String typeParameter) : this() {
			this.Name = name;
			this.TypeParameter = typeParameter;
		}

		public virtual void OnImplement(Module target) {
			Scope scope = new Scope {
				[this.TypeParameter] = target,
				[this.Name] = this
			};

			foreach (Expression decl in this.Declarations) {
				ClosureManager manager = new ClosureManager(scope);
				decl.Closure(manager).Eval(scope);
			}

			foreach (KeyValuePair<String, Value> member in scope.variables) {
				if (member.Value != this && member.Value != target) {
					target.SetMemberIfAbsent(member.Key, member.Value);
				}
			}
		}
	}

	public class SystemClass : Class {
		public override void OnImplement(Module target) {
			
		}
	}
}
