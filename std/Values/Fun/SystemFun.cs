using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public abstract class SystemFun : Fun, IObject {
		public Dictionary<String, Value> Attributes { get; set; }

		public SystemFun() {
			this.Attributes = new Dictionary<String, Value>();
		}

		public virtual Boolean IsParentOf(Value value) {
			if (value is IObject parent) {
				while (true) {
					if (parent.TryGet("@prototype", out var v)) {
						parent = v as IObject;
						if (parent == this) {
							return true;
						}
					}
					else {
						break;
					}
				}
			}
			return false;
		}

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public Value Clone() {
			return this;
		}

		public IObject Type {
			get {
				this.Attributes.TryGetValue("@prototype", out Value result);
				return result as IObject;
			}
			set => this.Attributes["@prototype"] = value;
		}

		public abstract List<FunctionArgument> Arguments { get; set; }

		public abstract Value Run(Scope e, params Value[] args);

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return "";
		}

		public Value Get(String name, Scope e) {
			if (this.Attributes.TryGetValue(name, out var result)) {
				return result;
			}

			if (this.Type != null) {
				if (this.Type.TryGet(name, out result)) {
					return result;
				}
			}

			throw new Exception($"record does not contains a field {name}", stack: e);
		}

		public void Set(String name, Value value, Scope e) {
			this.Attributes[name] = value;
		}

		public Boolean TryGet(String name, out Value result) {
			result = null;

			if (this.Attributes.TryGetValue(name, out result)) {
				return true;
			}

			if (this.Type != null) {
				if (this.Type.TryGet(name, out result)) {
					return true;
				}
			}

			return false;
		}
	}
}
