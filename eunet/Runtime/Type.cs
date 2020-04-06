using System;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class KsType : Function, KsTypeable {
		internal Dictionary<String, KsTypeable> arguments;
		public String Name { get; set; }
		public Func<XnObject, Boolean> Checker { get; set; }
		public Namespace EigenNamespace { get; set; }

		public KsTypeable Type => XnStd.TypeType;

		public XnObject Run(Scope scope, params XnObject[] arguments) {
			return new Atom(this.Checker(arguments[0]) ? 1 : 0);
		}

		public Boolean Satisfaction(XnObject ksObject) {
			return Checker(ksObject);
		}
	}
}
