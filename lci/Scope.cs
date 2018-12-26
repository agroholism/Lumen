using System;
using System.Collections.Generic;

namespace StandartLibrary {
	public class Scope {
		public IDictionary<String, Value> variables;
		public List<Module> usings;
		public List<String> constants;
		public Scope parent;

		public Value this[String name] {
			get => Get(name);
			set => Set(name, value);
		}

		public Value This {
			get => Get("this");
			set => Set("this", value);
		}

		public Scope() {
			this.variables = new Dictionary<String, Value>();
			this.constants = new List<String>();
			this.usings = new List<Module>();
		}

		public Scope(Scope parent) : this() {
			this.parent = parent;
		}

		public void Remove(String name) {
			this.variables.Remove(name);
		}

		public Boolean IsExsists(String name) {
			if (this.variables.ContainsKey(name) || (this.parent != null && this.parent.IsExsists(name))) {
				return true;
			}

			foreach (Value use in this.usings) {
				if (use is Module m) {
					if (m.Contains(name)) {
						return true;
					}
				}
			}

			return false;
		}

		public Boolean ExistsInThisScope(String name) {
			return this.variables.ContainsKey(name);
		}
		
		public Boolean TryGet(String name, out Value result) {
			if (this.variables.TryGetValue(name, out Value value)) {
				result = value;
				return true;
			}

			foreach (Value i in this.usings) {
				if (i is Module m) {
					if (m.Contains(name)) {
						result = m.Get(name);
						return true;
					}
					else {
						continue;
					}
				}
			}

			if (this.parent != null) {
				return this.parent.TryGet(name, out result);
			}

			result = null;
			return false;
		}
		
		public Value Get(String name) {
			if (this.variables.TryGetValue(name, out Value value)) {
				return value;
			}

			foreach (Module i in this.usings) {
				if (i.Contains(name)) {
					return i.Get(name);
				}
				else {
					continue;
				}
			}

			if (this.parent != null) {
				return this.parent.Get(name);
			}

			throw new System.Exception("неизвестная переменная " + name);
		}
		
		public void SetUnsafe(String name, Value value) {
			this.variables[name] = value;
		}
		
		public virtual void Set(String name, Value value) {
			if (!this.constants.Contains(name)) {
				this.variables[name] = value;
			}
			else {
				throw new Exception("изменить константу '" + name + "' невозможно", stack: this);
			}
		}
	
		public void AddConstant(String name) {
			if (!this.constants.Contains(name)) {
				this.constants.Add(name);
			}
		}

		public Boolean IsConstant(String id) {
			return this.constants.Contains(id);
		}

		public void AddUsing(Module obj) {
			this.usings.Add(obj);
		}
	}
}
