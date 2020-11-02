using System;
using System.Linq;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class Module : BaseValueImpl, IType {
		public String Name { get; set; }
		public Dictionary<String, Value> Members { get; private set; }
		public List<Module> Mixins { get; private set; }
		public EntitiyType EntitiyType { get; set; }

		public override IType Type => Prelude.Any;

		public Module() {
			this.Members = new Dictionary<String, Value>();
			this.Mixins = new List<Module>();
		}

		public Module(String name) {
			this.Members = new Dictionary<String, Value>();
			this.Mixins = new List<Module>();
			this.Name = name;
		}

		public Boolean Contains(String name) {
			return this.Members.ContainsKey(name) || this.Mixins.Any(i => i.Contains(name));
		}

		public Value GetMember(String name, Scope e) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
			}

			if(this.EntitiyType == EntitiyType.TYPE) {
				throw new LumenException($"Type {this.Name} does not contains a field {name}");
			}

			throw new LumenException($"Module {this.Name} does not contains a field {name}");
		}

		public void SetMember(String name, Value value, Scope e = null) {
			this.Members[name] = value;
		}

		public void SetField(String name, LumenFunc value, Scope e = null) {
			String[] args = name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

			LambdaFun lfun = new LambdaFun(value);

			for (Int32 i = 1; i < args.Length; i++) {
				lfun.Arguments.Add(new Expressions.NamePattern(args[i]));
			}

			this.Members[args[0]] = lfun;
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

		public Boolean IsParentOf(Value value) {
			Value parent = value.Type;

			if (parent == this) {
				return true;
			}

			if (parent is Constructor constructor) {
				return constructor.Parent == this;
			}

			if (parent is SingletonConstructor singleton) {
				return singleton.Parent == this;
			}

			return false;
		}

		public void IncludeMixin(Module mixin) {
			this.Mixins.Add(mixin);
		}

		public Boolean HasMixin(Module typeClass) {
			return this.Mixins.Contains(typeClass);
		}

		public override String ToString() {
			return $"{this.Name}";
		}
	}
}
