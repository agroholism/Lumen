using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class Type : Module, IMutableType {
		protected List<Class> Implementations { get; private set; }

		public Type(String name) : base(name) {
			this.Implementations = new List<Class>();
		}

		public override Boolean HasMember(String name) {
			return this.Members.ContainsKey(name) || this.Implementations.Any(i => i.HasMember(name));
		}

		public IEnumerable<String> AvailableNames {
			get {
				foreach (KeyValuePair<String, Value> item in this.Members) {
					yield return item.Key;
				}

				foreach (IMutableType implementation in this.Implementations) {
					foreach (String item in implementation.AvailableNames) {
						yield return item;
					}
				}
			}
		}

		public override Boolean TryGetMember(String name, out Value result) {
			if (this.Members.TryGetValue(name, out result)) {
				return true;
			}

			foreach (Module i in this.Implementations) {
				if (i.TryGetMember(name, out result)) {
					return true;
				}
			}

			if (this != Prelude.Any && Prelude.Any.TryGetMember(name, out result)) {
				return true;
			}

			return false;
		}

		public Boolean IsParentOf(Value value) {
			Value type = value.Type;

			if(this == Prelude.Any) {
				return true;
			}

			if (type == this) {
				return true;
			}

			if (type is IConstructor constructor) {
				return constructor.Parent == this;
			}

			return false;
		}

		public void AppendImplementation(Class classModule) {
			classModule.OnImplement(this);
			this.Implementations.Add(classModule);
		}

		public Boolean HasImplementation(Class classModule) {
			if (this.Implementations.Contains(classModule)) {
				return true;
			}

			return this.Implementations.Any(x => x.HasImplementation(classModule));
		}

		public override String ToString() {
			return $"<type {this.Name}>";
		}
	}
}
