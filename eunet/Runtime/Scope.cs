using System;
using System.Collections.Generic;

namespace Argent.Xenon.Runtime {
	public class Scope {
		private class Variable {
			public XnObject Value { get; set; }
			public KsTypeable Type { get; set; }

			public Variable(XnObject value, KsTypeable type) {
				this.Value = value;
				this.Type = type;
			}
		}

		private Dictionary<String, Variable> variables = new Dictionary<String, Variable>();
		private List<String> constants = new List<String>();

		public Scope() {
			
		}

		public void DeclareVariable(String name, KsTypeable type) {
			if(this.variables.ContainsKey(name)) {
				throw new XenonException(Exceptions.ATTEMPT_TO_REDEFINE.Format(name));
			}

			this.variables[name] = new Variable(null, type);
		}

		public void DeclareConstant(String name, KsTypeable type, XnObject value) {
			this.DeclareVariable(name, type);
			this.Assign(name, value);
			this.constants.Add(name);
		}

		public void Assign(String name, XnObject value) {
			if (this.variables.TryGetValue(name, out Variable variable)) {
				if(this.constants.Contains(name)) {
					throw new XenonException(Exceptions.ATTEMPT_TO_REDEFINE.Format(name));
				}

				if(!variable.Type.Checker(value)) {
					throw new XenonException(Exceptions.TYPE_CHECK_FAILURE.Format(name));
				}

				variable.Value = value;
				return;
			}

			throw new XenonException(Exceptions.HAS_NOT_BEEN_DECLARED.Format(name));
		}

		public void UnsafeAssign(String name, XnObject value) {
			if (this.variables.TryGetValue(name, out Variable variable)) {
				variable.Value = value;
				return;
			}

			this.variables[name] = new Variable(value, value.Type);
		}

		public XnObject Get(String name) {
			if (this.variables.TryGetValue(name, out Variable variable)) {
				if(variable.Value == null) {
					throw new XenonException(Exceptions.HAS_NOT_BEEN_ASSIGNED.Format(name));
				}

				return variable.Value;
			}

			if(XnStd.EU_STD.variables.TryGetValue(name, out XnObject x)) {
				return x;
			}

			throw new XenonException(Exceptions.HAS_NOT_BEEN_DECLARED.Format(name));
		}

		public Boolean IsExists(String name) {
			return this.variables.ContainsKey(name) || XnStd.EU_STD.variables.ContainsKey(name);
		}
	}
}
