using System;
using System.Collections.Generic;

namespace Lumen.Lang.Expressions {
	public class ClosureManager {
		private readonly List<String> declared;
		public Scope Scope { get; private set; }

		public Boolean HasYield { get; set; }
		public Boolean HasTailRecursion { get; set; }

		public IEnumerable<String> Declarations {
			get => this.declared;
		}

		public ClosureManager(Scope scope) {
			this.Scope = scope;
			this.declared = new List<String>();
		}

		public void Declare(String name) {
			if (!this.IsDeclared(name)) {
				this.declared.Add(name);
			}
		}

		public void Declare(IEnumerable<String> names) {
			foreach (String name in names) {
				this.Declare(name);
			}
		}

		public void Assimilate(ClosureManager another) {
			foreach (String declaration in another.Declarations) {
				this.Declare(declaration);
			}

			this.HasTailRecursion |= another.HasTailRecursion;
			this.HasYield |= another.HasYield;
		}

		public Boolean IsDeclared(String name) {
			if (name == "rec" || name == "_") {
				return true;
			}
			return this.declared.Contains(name);
		}

		public ClosureManager Clone() {
			ClosureManager manager = new ClosureManager(this.Scope);
			manager.Declare(this.declared);
			manager.HasYield = this.HasYield;
			return manager;
		}
	}
}
