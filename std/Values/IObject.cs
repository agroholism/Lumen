using System;

namespace Lumen.Lang {
    /// <summary> Интерфейс, реализуемый всеми объектами. </summary>
    public interface IType : Value {
        void SetMember(String name, Value value, Scope scope);

		Value GetMember(String name, Scope scope);

		Boolean TryGetMember(String name, out Value result);

		Boolean IsParentOf(Value value);

		Boolean HasMixin(Module typeClass);

	}
}
