using System;

namespace Lumen.Lang {
	public class SingletonConstructor : BaseValueImpl, IConstructor {
		public String Name { get; set; }

		public override IType Type => this.Parent; 

		public Type Parent { get; set; }

		public SingletonConstructor(String name, Type parent) {
			this.Parent = parent;
			this.Name = name;
		}

		public Value MakeInstance(params Value[] values) {
			return this;
		}

		public Value GetMember(String name) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			throw new LumenException($"fne {name} {this.Name}");
		}

		public override String ToString() {
			if (this.Type.TryGetMember("toText", out Value value)
				&& value.TryConvertToFunction(out Fun converter)) {
				return converter.Call(new Scope(), this).ToString();
			}

			return this.Name;
		}

		public Boolean TryGetMember(String name, out Value result) {
			if (this.Parent.TryGetMember(name, out result)) {
				return true;
			}

			return this.Type.TryGetMember(name, out result);
		}

		public Boolean IsParentOf(Value value) {
			return value == this;
		}

		public Boolean HasImplementation(Class typeClass) {
			return this.Parent.HasImplementation(typeClass);
		}

		public Boolean HasMember(String name) {
			return this.Parent.HasMember(name);
		}
	}
}
