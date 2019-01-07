using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class RecordValue : IObject {
		public Dictionary<String, Value> items;

		public IObject Prototype {
			get => this.items["@prototype"] as IObject;
			set => this.items["@prototype"] = value;
		}

		public Boolean IsParentOf(Value value) {
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

		public override String ToString() {
			return this.ToString(null);
		}

		public IObject Type => Prototype;

		public RecordValue() {
			this.items = new Dictionary<String, Value> {
				["@prototype"] = null
			};
		}

		public RecordValue(IObject prototype) {
			this.items = new Dictionary<String, Value> {
				["@prototype"] = prototype
			};
		}

		public Boolean TryGet(String name, out Value result) {
			result = null;

			if(this.items.TryGetValue(name, out result)) {
				return true;
			}

			if(this.Prototype != null) {
				if(this.Prototype.TryGet(name, out result)) {
					return true;
				}
			}

			return false;
		}

		public Value Get(String name, Scope e) {
			if(this.items.TryGetValue(name, out var result)) {
				return result;
			}

			if(this.Prototype != null) {
				if(this.Prototype.TryGet(name, out result)) {
					return result;
				}
			}

			throw new Exception($"record does not contains a field {name}", stack: e);
		}

		public void Set(String name, Value value, Scope e) {
			this.items[name] = value;
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			Scope ex = new Scope(e) { This = this };

			if(this.TryGet("str", out Value value)) {
				return ((Fun)value).Run(new Scope(e) { This = this }).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу std.str", stack: e);
		}

		public Value Clone() {
			return this;
		}

		public override Boolean Equals(Object obj) {
			/*if(obj is Value value) {
				if(value is RecordValue objn) {
					if(objn.Type == Type) {
						for (Int32 i = 0; i < this.value.Length; i++) {
							if (!this.value[i].Equals(objn.value[i])) {
								return false;
							}
						}
						return true;
					}
				}
			}
			*/
			return base.Equals(obj);
		}

		public override Int32 GetHashCode() {
			return base.GetHashCode();
		}
	}
}