using System;

namespace Lumen.Lang {
	public interface IType : IValue {
		void SetMember(String name, IValue value, Scope scope);

		IValue GetMember(String name, Scope scope);

		Boolean TryGetMember(String name, out IValue result);

		Boolean IsParentOf(IValue value);

		Boolean HasImplementation(Module typeClass);

	}
}
