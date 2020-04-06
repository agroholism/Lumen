using System;

namespace Argent.Xenon.Runtime {
	public interface KsTypeable : ImmutObject {
		String Name { get; }

		Boolean Satisfaction(XnObject ksObject);

		Func<XnObject, Boolean> Checker { get; }
	}
}
