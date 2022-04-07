using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class Scope {
		public readonly IDictionary<String, Value> variables;
		public readonly List<Module> usings;
		public readonly Scope parent;
		public Dictionary<String, List<Value>> attributes;

		private IEnumerable<String> AvailableNames {
			get {
				foreach (var item in this.variables) {
					yield return item.Key;
				}


				foreach (Module use in this.usings) {
					foreach(var item in use.Members) {
						yield return item.Key;
					}
				}

				if (this.parent != null) {
					foreach (var item in parent.AvailableNames) {
						yield return item;
					}
				}
			}
		}

		public Value this[String name] {
			get => this.Get(name);
			set => this.Bind(name, value);
		}

		public Scope() {
			this.variables = new Dictionary<String, Value>();
			this.usings = new List<Module> { Prelude.Instance };
			this.attributes = new Dictionary<String, List<Value>>();
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

		public Boolean TryGetFromThisScope(String name, out Value result) {
			if (this.variables.TryGetValue(name, out Value value)) {
				result = value;
				return true;
			}

			result = null;
			return false;
		}

		public Boolean TryGet(String name, out Value result) {
			if (this.variables.TryGetValue(name, out Value value)) {
				result = value;
				return true;
			}

			foreach (Value i in this.usings) {
				if (i is Module m) {
					if (m.TryGetMember(name, out result)) {
						return true;
					}
					continue;
				}
			}

			if (this.parent != null) {
				return this.parent.TryGet(name, out result);
			}

			result = null;
			return false;
		}

		public void SetAttribute(String id, Value val) {
			if (this.attributes.TryGetValue(id, out List<Value> attributesList)) {
				attributesList.Add(val);
			}

			this.attributes[id] = new List<Value> { val };
		}

		public Value Get(String name) {
			if (this.variables.TryGetValue(name, out Value value)) {
				return value;
			}

			foreach (Module i in this.usings) {
				if (i.TryGetMember(name, out value)) {
					return value;
				}

				continue;
			}

			if (this.parent != null) {
				return this.parent.Get(name);
			}

			List<String> maybe = new List<String>();
			foreach (KeyValuePair<String, Value> i in this.variables) {
				if (Helper.Levenshtein(i.Key, name) > 0.4) {
					maybe.Add(i.Key);
				}
			}

			throw new LumenException(Exceptions.UNKNOWN_NAME.F(name)) {
				Note = maybe.Count > 0 ? $"Maybe you mean {Environment.NewLine}{String.Join(Environment.NewLine, maybe)}" : null
			};
		}

		public void Bind(String name, Value value) {
			this.variables[name] = value;
		}

		public void AddUsing(Module obj) {
			this.usings.Add(obj);
		}

		public List<String> FindClosestNames(Double maxValue, String key) {
			List<String> maybe = this.AvailableNames
								.Where(i => Helper.Levenshtein(i, key) < maxValue)
								.OrderBy(i => Helper.Levenshtein(i, key))
								.Distinct()
								.ToList();
			return maybe;
		}
	}
}
