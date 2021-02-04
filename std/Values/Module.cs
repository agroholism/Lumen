using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang {
	public class Module : BaseValueImpl, IMutableModule {
		public String Name { get; protected set; }
		public Dictionary<String, Value> Members { get; protected set; }

		public override IType Type => Prelude.Function; // fix this later

		public Module(String name) {
			this.Name = name;
			this.Members = new Dictionary<String, Value>();
		}

		public virtual Boolean HasMember(String name) {
			return this.Members.ContainsKey(name);
		}

		public virtual Boolean TryGetMember(String name, out Value result) {
			return this.Members.TryGetValue(name, out result);
		}

		public Value GetMember(String name) {
			if (this.TryGetMember(name, out Value result)) {
				return result;
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

			throw new LumenException($"Module {this.Name} does not contains a name \"{name}\"") {
				Note = note
			};
		}

		public void SetMember(String name, Value value) {
			this.Members[name] = value;
		}

		public void SetMemberIfAbsent(String name, Value value) {
			if (!this.Members.ContainsKey(name)) {
				this.Members[name] = value;
			}
		}

		protected List<String> FindClosestNames(Double maxValue, String key) {
			return this.Members.Keys
				.Where(i => Helper.Levenshtein(i, key) < maxValue)
				.OrderBy(i => Helper.Levenshtein(i, key))
				.ToList();
		}

		public override String ToString() {
			return $"<module {this.Name}>";
		}
	}
}
