using System;

namespace Lumen.Lang {
	public class SingletonConstructor : BaseValueImpl, IConstructor {
		public String Name { get; set; }

		public override IType Type => this.Parent; 

		public Module Parent { get; set; }

		public SingletonConstructor(String name, Module parent) {
			this.Parent = parent;
			this.Name = name;
		}

		public IValue MakeInstance(params IValue[] values) {
			return this;
		}

		public IValue GetMember(String name, Scope scope) {
			if (this.TryGetMember(name, out IValue result)) {
				return result;
			}

			throw new LumenException($"fne {name} {this.Name}");
		}

		public void SetMember(String name, IValue value, Scope scope) {

		}

		public override String ToString() {
			if (this.Type.TryGetMember("toText", out IValue value)
				&& value.TryConvertToFunction(out Fun converter)) {
				return converter.Call(new Scope(), this).ToString();
			}

			return this.Name;
		}

		public Boolean TryGetMember(String name, out IValue result) {
			if (this.Parent.TryGetMember(name, out result)) {
				return true;
			}

			return this.Type.TryGetMember(name, out result);
		}

		public Boolean IsParentOf(IValue value) {
			return value == this;
		}

		public Boolean HasImplementation(Module typeClass) {
			return this.Parent.HasImplementation(typeClass);
		}
	}
}
