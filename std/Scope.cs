#nullable enable

using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public class Scope {
		public readonly IDictionary<String, IValue> variables;
		public readonly List<Module> usings;
		public readonly Scope? parent;
		private readonly HashSet<String> privates;

		public IEnumerable<String> Privates {
			get {
				foreach (String item in this.privates) {
					yield return item;
				}
			}
		}

		public IEnumerable<String> AvailableNames {
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

		public IEnumerable<(String name, IValue value)> ExportedVariables {
			get {
				foreach (KeyValuePair<String, IValue> variable in this.variables) {
					if (!this.IsPrivate(variable.Key)) {
						yield return  (variable.Key, variable.Value);
					}
				}
			}
		}

		public IValue this[String name] {
			get => this.Get(name);
			set => this.Bind(name, value);
		}
		
		public Scope() {
			this.variables = new Dictionary<String, IValue>();
			this.usings = new List<Module> { Prelude.Instance };
			this.privates = new HashSet<String>();
		}
		
		public Scope(Scope parent) : this() {
			this.parent = parent;
		}

		public void DeclarePrivate(String name) {
			this.privates.Add(name);
		}

		public Boolean IsPrivate(String name) {
			return this.privates.Contains(name);
		}

		public void Remove(String name) {
			this.variables.Remove(name);
		}

		public Boolean IsExsists(String name) {
			if (this.variables.ContainsKey(name) || (this.parent != null && this.parent.IsExsists(name))) {
				return true;
			}

			foreach (IValue use in this.usings) {
				if (use is Module m) {
					if (m.Contains(name)) {
						return true;
					}
				}
			}

			return false;
		}

		public Boolean IsExistsInThisScope(String name) {
			return this.variables.ContainsKey(name);
		}

		public Boolean TryGetFromThisScope(String name, out IValue? result) {
			if (this.variables.TryGetValue(name, out IValue value)) {
				result = value;
				return true;
			}

			result = null;
			return false;
		}

		public Boolean TryGet(String name, out IValue? result) {
			if (this.variables.TryGetValue(name, out IValue value)) {
				result = value;
				return true;
			}

			foreach (IValue i in this.usings) {
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

		public IValue Get(String name) {
			if (this.variables.TryGetValue(name, out IValue value)) {
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
			foreach (KeyValuePair<String, IValue> i in this.variables) {
				if (Helper.Levenshtein(i.Key, name) > 0.4) {
					maybe.Add(i.Key);
				}
			}

			throw new LumenException(Exceptions.UNKNOWN_NAME.F(name)) {
				Note = maybe.Count > 0 ? $"Maybe you mean {Environment.NewLine}{String.Join(Environment.NewLine, maybe)}" : null
			};
		}

		/// <summary>
		/// Binds a value with some name (there is no checking for binding existense)
		/// </summary>
		public void Bind(String name, IValue value) {
			this.variables[name] = value;
		}

		/// <summary>
		/// Adds module into the search path of the scope
		/// </summary>
		public void AddUsing(Module module) {
			this.usings.Add(module);
		}
	}
}
