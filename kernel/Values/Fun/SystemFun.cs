using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public abstract class SystemFun : Fun, IObject {
		public Dictionary<String, Value> Attributes { get; set; }

		public SystemFun() {
			this.Attributes = new Dictionary<String, Value>();
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			return this.Attributes[name];
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			this.Attributes[name] = value;
		}

		public int CompareTo(object obj) {
			return 0;
		}

		public Value Clone() {
			return this;
		}

		public KType Type => StandartModule.Function;

		public abstract List<FunctionArgument> Arguments { get; set; }

		public abstract Value Run(Scope e, params Value[] args);

		public bool ToBool(Scope e) {
			throw new NotImplementedException();
		}

		public double ToDouble(Scope e) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			return "";
		}
	}
}
