using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Std {
	public class Objectn : IObject {
		public Value[] value;
		public Record type;

		public override String ToString() {
			return this.ToString(null);
		}

		public Record Type => this.type;

		public Objectn(Int32 size, Record parent) {
			this.value = new Value[size];
			this.type = parent;
		}

		private Int32 Index(List<FunctionArgument> pairs, String searched) {
			Int32 result = 0;

			foreach (FunctionArgument pair in pairs) {
				if (pair.name == searched) {
					return result;
				}
				result++;
			}

			return -1;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			Int32 index = Index(this.type.meta.Fields, name);

			if (index != -1) {
				return this.value[index];
			}

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e) {
					This = this
				};
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Index(this.type.meta.Fields, name);

			if (index != -1) {
				this.value[index] = value;
				return;
			}

			if (this.type.AttributeExists("set_" + name) && this.type.GetAttribute("set_" + name, e) is Fun property) {
				Scope s = new Scope(e) {
					This = this
				};
				property.Run(s, value);
				return;
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public Int32 CompareTo(Object obj) {
			throw new NotImplementedException();
		}

		public String ToString(Scope e) {
			Scope ex = new Scope(e);
			ex.Set("this", this);
			if (this.type.AttributeExists("to_s")) {
				return ((Fun)this.type.GetAttribute("to_s", e)).Run(ex).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу std.str", stack: e);
		}

		public Value Clone() {
			Objectn result = new Objectn(this.value.Length, this.type);
			for (Int32 i = 0; i < this.value.Length; i++) {
				result.value[i] = this.value[i];
			}
			return result;
		}

		public override Boolean Equals(Object obj) {
			if(obj is Value value) {
				if(value is Objectn objn) {
					if(objn.Type == Type) {
						for (Int32 i = 0; i < this.value.Length; i++) {
							if (!this.value[i].Equals(objn.value[i])) {
								return false;
							}
						}
						return true;
					}
				}
			}

			return base.Equals(obj);
		}
	}
}