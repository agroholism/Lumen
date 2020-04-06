using System;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class Namespace : XnObject {
		public Dictionary<String, XnObject> variables = new Dictionary<String, XnObject>();

		public KsTypeable Type => XnStd.FunctionType;
		//public KsTypeable Type => ;

		public XnObject this[String name] {
			get => this.variables[name];
			set => this.variables[name] = value;
		}

		public Namespace() {

		}

		public void Set(String name, XnObject value) {
			this.variables[name] = value;
		}
	}
}
