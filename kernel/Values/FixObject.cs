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

		private Int32 Index(Dictionary<String, Record> pairs, String searched) {
			Int32 result = 0;

			foreach (var pair in pairs) {
				if (pair.Key == searched) {
					return result;
				}
				result++;
			}

			return -1;
		}

		public Value Get(string name, AccessModifiers mode, Scope e) {
			Int32 index = Index(this.type.meta.Fields, name);

			if (index != -1)
				return value[index];

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(string name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Index(this.type.meta.Fields, name);

			if (index != -1) {
				this.value[index] = value;
				return;
			}

			if (this.type.AttributeExists("set_" + name) && this.type.GetAttribute("set_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				property.Run(s, value);
				return;
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public int CompareTo(object obj) {
			throw new NotImplementedException();
		}

		public List<Value> ToList() {
			throw new NotImplementedException();
		}

		public string ToString(Scope e) {
			Scope ex = new Scope(e);
			ex.Set("this", this);
			if (type.AttributeExists("ToString"))
				return ((Fun)type.GetAttribute("ToString", e)).Run(ex).ToString(e);
			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public Value Clone() {
			Objectn result = new Objectn(value.Length, this.type);
			for (int i = 0; i < this.value.Length; i++) {
				result.value[i] = this.value[i];
			}
			return result;
		}
	}
}