using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Lang {
	public class FunctionalType : BaseValueImpl, IType {
		public override IType Type => Prelude.Function;

		public string Name { get; private set; }
		private readonly Fun predicate;

		public FunctionalType(string name, Fun predicate) {
			this.Name = name;	
			this.predicate = predicate;
		}

		public IValue GetMember(string name, Scope scope) {
			throw new NotImplementedException();
		}

		public bool HasImplementation(Module typeClass) {
			return false;
		}

		public bool IsParentOf(IValue value) {
			return this.predicate.Call(new Scope(), value).ToBoolean();
		}

		public void SetMember(string name, IValue value, Scope scope) {
			throw new NotImplementedException();
		}

		public bool TryGetMember(string name, out IValue result) {
			throw new NotImplementedException();
		}

		public override string ToString() {
			return this.Name;
		}
	}
}
