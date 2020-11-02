﻿using System;

namespace Lumen.Lang {
	public class SingletonConstructor : BaseValueImpl, IType {
		public String Name { get; set; }

		public override IType Type => this.Parent;

		public Module Parent { get ; set ; }

		public SingletonConstructor(String name, Module parent) {
			this.Parent = parent;
			this.Name = name;
		}

		public Value GetMember(String name, Scope scope) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			throw new LumenException("fne");
		}

		public void SetMember(String name, Value value, Scope scope) {

		}

		public override String ToString() {
			return this.Name;
		}

		public Boolean TryGetMember(String name, out Value result) {
			if(this.Parent.TryGetMember(name, out result)) {
				return true;
			}

			foreach (Module i in this.Parent.Mixins) {
				if (i.TryGetMember(name, out result)) {
					return true;
				}
			}

			return this.Type.TryGetMember(name, out result);
		}

		public Boolean IsParentOf(Value value) {
			return value == this;
		}

		public Boolean HasMixin(Module typeClass) {
			return this.Parent.HasMixin(typeClass);
		}
	}
}
