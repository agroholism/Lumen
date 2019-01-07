using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Record : IObject {
		public IDictionary<String, Fun> attributes;
		public List<Module> includedModules;
		public TypeMetadata meta;

		public Record() {
			this.includedModules = new List<Module>();
			this.attributes = new Dictionary<String, Fun>();
		}

		/// <summary> Возвращает аттрибут экземпляра </summary>
		/// <param name="name"> Имя аттрибута </param>
		/// <param name="mode"> Модификатор области видимости. </param>
		public Fun GetAttribute(String name, Scope e) {
			if (this.attributes.TryGetValue(name, out Fun result)) {
				return result;
			}

			throw new Exception($"value of type '{this.meta.Name}' does not have a function/property '{name}'", stack: e);
		}

		public void SetAttribute(String name, Fun attr) {
			this.attributes[name] = attr;
		}

		public Boolean AttributeExists(String name) {
			return this.attributes.ContainsKey(name);
		}

		public virtual IObject Type => StandartModule._Type;

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return this.meta.Name;
		}

		public override String ToString() {
			return this.meta.Name;
		}

		public Value Clone() {
			return this;
		}

		public Value Get(String name, Scope e) {
			return this.GetAttribute(name, e);
		}

		public void Set(String name, Value value, Scope e) {
			this.SetAttribute(name, value as Fun);
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

			if(value.Type == this) {
				return true;
			}

			return false;
		}

		public Boolean TryGet(String name, out Value result) {
			result = null;

			if (this.AttributeExists(name)) {
				result = Get(name, null);
				return true;
			}

			return false;
		}
	}
}
