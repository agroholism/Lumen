using System;
using System.Collections.Generic;

namespace Lumen.Lang {
	public interface ICompoundValue : Value {
		Value GetMember(String name);

		Boolean TryGetMember(String name, out Value result);

		Boolean HasMember(String name);
	}

	public interface IMutableCompoundValue : ICompoundValue {
		void SetMember(String name, Value value);

		void SetMemberIfAbsent(String name, Value value);
	}

	public interface IModule : ICompoundValue {

	}

	public interface IMutableModule : IMutableCompoundValue {

	}

	public interface IType : IModule {
		Boolean IsParentOf(Value value);

		Boolean HasImplementation(Class cls);
	}

	public interface IMutableType : IMutableModule, IType {
		IEnumerable<String> AvailableNames { get; }

		void AppendImplementation(Class cls);
	}
}
