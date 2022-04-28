using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class Module : BaseValueImpl, IType {
		public String Name { get; protected set; }
		public Dictionary<String, IValue> Members { get; protected set; }
		internal List<Module> Mixins { get; private set; }
		private readonly HashSet<String> privates;

		public override IType Type => Prelude.Module;

		public Module() {
			this.Members = new Dictionary<String, IValue>();
			this.Mixins = new List<Module>();
			this.privates = new HashSet<string>();
		}

		public Module(String name) : this() {
			this.Name = name;
		}

		public void DeclarePrivate(String name) {
			this.privates.Add(name);
		}

		public Boolean IsPrivate(String name) {
			return this.privates.Contains(name);
		}

		public Boolean Contains(String name) {
			return this.Members.ContainsKey(name) || this.Mixins.Any(i => i.Contains(name));
		}

		private IEnumerable<String> AvailableNames {
			get {
				foreach (var item in this.Members) {
					yield return item.Key;
				}


				foreach (Module use in this.Mixins) {
					foreach (KeyValuePair<String, IValue> item in use.Members) {
						yield return item.Key;
					}
				}
			}
		}

		public List<String> FindClosestNames(Double maxValue, String key) {
			List<String> maybe = this.AvailableNames
								.Where(i => Helper.Levenshtein(i, key) < maxValue)
								.OrderBy(i => Helper.Levenshtein(i, key))
								.Distinct()
								.ToList();
			return maybe;
		}

		public Boolean TryGetMember(String name, out IValue result) {
			result = null;

			if (!this.IsPrivate(name) && this.Members.TryGetValue(name, out result)) {
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

		public IValue GetMember(String name, Scope _) {
			if (this.TryGetMember(name, out IValue result)) {
				return result;
			}

			if (this.IsPrivate(name)) {
				throw new LumenException(Prelude.PrivacyError.constructor, $"can not use private name \"{name}\" from module \"{this}\"");
			}

			List<String> maybe = this.FindClosestNames(5, name);

			String note = null;

			if (maybe.Count == 1) {
				note = $"Perhaps you meant '{maybe[0]}'?";
			}
			else if (maybe.Count > 3) {
				note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe.Take(3))}";
			}
			else if (maybe.Count > 1) {
				note = $"Perhaps you meant one of these names: {Environment.NewLine}\t{String.Join(Environment.NewLine + "\t", maybe)}";
			}

			throw new LumenException($"module \"{this}\" does not contains a name \"{name}\"") {
				Note = note
			};
		}

		public void SetMember(String name, IValue value, Scope _ = null) {
			this.Members[name] = value;
		}

		public void SetMemberIfAbsent(String name, IValue value) {
			if (!this.Members.ContainsKey(name)) {
				this.Members[name] = value;
			}
		}

		public Boolean IsParentOf(IValue value) {
			IValue parent = value.Type;

			if(this == Prelude.Any) {
				return true;
			}

			if (parent == this) {
				return true;
			}

			if (parent is IConstructor constructor) {
				return constructor.Parent == this;
			}

			return value.Type.HasImplementation(this);
		}

		public void AppendImplementation(Class classModule) {
			classModule.OnImplement(this);
			this.Mixins.Add(classModule);
		}

		public Boolean HasImplementation(Module classModule) {
			if (this.Mixins.Contains(classModule)) {
				return true;
			}

			return this.Mixins.Any(x => x.HasImplementation(classModule));
		}

		public override String ToString() {
			if (this.TryGetMember(Constants.PARENT_MODULE_SPECIAL_NAME, out IValue parent) && parent is Module module) {
				return $"{module}.{this.Name}";
			}

			return $"{this.Name}";
		}
	}
}
