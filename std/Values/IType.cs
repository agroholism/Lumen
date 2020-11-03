using System;

namespace Lumen.Lang {
	public interface IType : Value {
		void SetMember(String name, Value value, Scope scope);

		Value GetMember(String name, Scope scope);

		Boolean TryGetMember(String name, out Value result);

		Boolean IsParentOf(Value value);

		Boolean HasImplementation(Module typeClass);

	}
}
