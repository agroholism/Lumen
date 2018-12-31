using System;
using System.Linq;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Module : Value {
		public Scope scope;
		public Value privateVar;
		public String name;
		public Boolean isNotScope;

		public Module() {
			this.scope = new Scope(null);
		}

		public Module(String name) {
			this.scope = new Scope(null);
			this.name = name;
		}

		public Module(Scope e) {
			if (e.ExistsInThisScope("$")) {
				this.privateVar = e.Get("$");
			}

			e.Remove("$");
			this.scope = e;
		}

		public void DeleteVar(String name) {
			this.scope.variables.Remove(name);
		}

		public Boolean Contains(String name) {
			return this.scope.IsExsists(name);
		}

		public Value Get(String Name) {
			if (Contains(Name)) {
				return this.scope[Name];
			}

			throw new System.Exception("неизвестная переменная " + Name + " в модуле " + this.name);
		}

		public void Set(String Name, Value Object) {
			this.scope[Name] = Object;
		}

		public void SetNonStrict(String Name, Value Object) {
			this.scope[Name] = Object;
		}

		public virtual Boolean TypeImplemented(Record s) {
			foreach(KeyValuePair<String, Value> i in this.scope.variables) {
				if (i.Value is Fun f) {
					if (!s.attributes.ContainsKey(i.Key)) {
						s.SetAttribute(i.Key, f);
					}
				}
			}

			return true;
		}

		public Boolean ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public Double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public Record Type => throw new NotImplementedException();

		public Int32 CompareTo(Object obj) {
			return 0;
		}

		public Value Clone() {
			return this;
		}

		public String ToString(Scope e) {
			return this.name;
		}

		public override String ToString() {
			return this.name;
		}

		public Boolean Match(Value value) {
			return value.Type.includedModules.Contains(this);
		}

		public Boolean Implicit(Value input, Scope scope, out Value output) {
			throw new NotImplementedException();
		}
	}
}
