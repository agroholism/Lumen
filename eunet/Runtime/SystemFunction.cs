using System;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public delegate XnObject SysF(Scope scope, params XnObject[] arguments);

	public class SystemFunction : Function {
		public Dictionary<String, KsTypeable> arguments;
		private SysF sys;

		public KsTypeable Type => XnStd.FunctionType;

		public SystemFunction(SysF sys) {
			this.sys = sys;
		}

		public XnObject Run(Scope scope, params XnObject[] arguments) {
			if (this.arguments.Count != arguments.Length) {
				throw new XenonException("invalid arg count");
			}

			Int32 index = 0;
			foreach (KeyValuePair<String, KsTypeable> i in this.arguments) {
				if (i.Value.Checker(arguments[index])) {
					scope.DeclareVariable(i.Key, i.Value);
					scope.Assign(i.Key, arguments[index]);
				}
				else {
					throw new XenonException("invalid arg type");
				}
				index++;
			}

			return this.sys(scope, arguments);
		}
	}
}
