using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class Module : BaseValueImpl, IType {
		public String Name { get; protected set; }
		public Dictionary<String, Value> Members { get; protected set; }
		protected List<Module> Mixins { get; private set; }

		public override IType Type => Prelude.Function;

		public Module() {
			this.Members = new Dictionary<String, Value>();
			this.Mixins = new List<Module>();
		}

		public Module(String name) : this() {
			this.Name = name;
		}

		public Boolean Contains(String name) {
			return this.Members.ContainsKey(name) || this.Mixins.Any(i => i.Contains(name));
		}

		public Boolean TryGetMember(String name, out Value result) {
			if (this.Members.TryGetValue(name, out result)) {
				return true;
			}

			foreach (Module i in this.Mixins) {
				if (i.TryGetMember(name, out result)) {
					return true;
				}
			}

			if (this != Prelude.Any && Prelude.Any.TryGetMember(name, out result)) {
				return true;
			}

			return false;
		}

		public Value GetMember(String name, Scope _) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			throw new LumenException($"Module {this.Name} does not contains a field {name}");
		}

		public void SetMember(String name, Value value, Scope _ = null) {
			this.Members[name] = value;
		}

		public Boolean IsParentOf(Value value) {
			Value parent = value.Type;

			if (parent == this) {
				return true;
			}

			if (parent is IConstructor constructor) {
				return constructor.Parent == this;
			}

			return false;
		}

		public void AppendImplementation(Module classModule) {
			this.Mixins.Add(classModule);
		}

		public Boolean HasImplementation(Module classModule) {
			if (this.Mixins.Contains(classModule)) {
				return true;
			}

			return this.Mixins.Any(x => x.HasImplementation(classModule));
		}

		public override String ToString() {
			return $"{this.Name}";
		}
	}
}
