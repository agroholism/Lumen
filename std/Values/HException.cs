using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Exception : System.Exception, IObject {
		public Record type;
		public Int32 line;
		public String file;
		public Scope stack;
		public Expando DATA;
		public String mess;

		public override String Message => this.mess;

		public Exception(String message, Record type = null, Scope stack = null) {
			this.type = type ?? StandartModule.Exception;
			this.stack = stack;
			this.DATA = new Expando();
			this.mess = message;
		}

		public static void BuildCallStack(Scope e, List<Value> result) {
			if (e.ExistsInThisScope("self")) {
				result.Add(e.Get("self"));
			}

			if (e.parent == null) {
				return;
			}

			BuildCallStack(e.parent, result);
		}

		public IObject Type => this.type;

		public Int32 CompareTo(Object obj) {
			if (obj is Value) {
				Scope e = new Scope(null);
				e.Set("this", this);
				//return (Int32)Converter.ToDouble(((Fun)this.Type.Get("<=>", null)).Run(e, (Value)obj), null);
			}
			throw new Exception("notcomparable", stack: null);
		}

		public String ToString(Scope e) {
			return $"{this.type.meta.Name}: {this.Message}";
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public Value Clone() {
			return this;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			if(name == "message") {
				return (Str)this.Message;
			}
			return this.DATA.Get(name, mode, e);
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			if(name == "message") {
				this.mess = value.ToString(e);
				return;
			}
			this.DATA.Set(name, value, mode, e);
		}

		public Value Get(String name, Scope e) {
			throw new NotImplementedException();
		}

		public void Set(String name, Value value, Scope e) {
			throw new NotImplementedException();
		}

		public Boolean TryGet(String name, out Value result) {
			throw new NotImplementedException();
		}

		public Boolean IsParentOf(Value value) {
			throw new NotImplementedException();
		}
	}
}
