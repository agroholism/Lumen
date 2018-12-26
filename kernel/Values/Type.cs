using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Std {
	public class KType : Value {
		public IDictionary<String, Value> typeAttributes;
		public IDictionary<String, Fun> attributes;
		public String[] variables;
		public List<Module> includedModules;
		public TypeMetadata meta;

		public KType() {
			this.includedModules = new List<Module>();
			this.typeAttributes = new Dictionary<String, Value>();
			this.attributes = new Dictionary<String, Fun>();
			this.variables = new String[0];
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

		public Value Get(String name, Scope e) {
			if (this.typeAttributes.TryGetValue(name, out Value result)) {
				return result;
			}

			if (this.typeAttributes.ContainsKey("get_" + name) && this.typeAttributes["get_" + name] is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			if (this.Contains("name_missing") && this.Get("name_missing", e) is Fun func) {
				Scope s = new Scope(e);
				s.This = e.This;
				return func.Run(s, (KString)name);
			}

			throw new System.Exception($"type '{this.meta.Name}' does not have a field '{name}'");
		}

		public void Set(String name, Value obj, Scope e = null) {
			this.typeAttributes[name] = obj;
		}

		public Boolean Contains(String Name) {
			if (variables.Contains(Name) || typeAttributes.ContainsKey(Name))
				return true;

			return false;
		}

		public virtual KType Type => StandartModule._Type;

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

		public Boolean Implicit(Value input, Scope scope, out Value output) {
			output = null;

			if (!this.Contains("implicit")) {
				return false;
			}

			try {
				output = (this.Get("implicit", scope) as Fun).Run(new Scope(scope), input);

				return true;
			}
			catch (System.Exception e) {
				return false;
			}
		}
	}
}
