using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumen.Lang.Std {
	public class Object0 : IObject {
		public KType type;
		public KType Type => this.type;

		public Object0(KType type) {
			this.type = type;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		/// <summary> Устанавливает поле объекта. </summary> 
		/// <param name="name"> Имя поля. </param> 
		/// <param name="value"> Значение. </param>
		/// <param name="mode"> Модификатор доступа. </param>
		/// <param name="e"> Область видимости, из которой осуществляется доступ. </param>
		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			if (this.type.AttributeExists("set_" + name) && this.type.GetAttribute("set_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				property.Run(s, value);
				return;
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		/// <summary> Компаратор. </summary>
		public Int32 CompareTo(Object obj) {
			if (obj is Value value) {
				if (this.type.AttributeExists("<=>") && this.type.GetAttribute("<=>", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
					return (Int32)(double)Converter.ToBigFloat(fun.Run(s, value), null);
				}
			}

			return 0;
		}

		public String ToString(Scope e) {
			if (this.type.AttributeExists("to_s") && this.type.GetAttribute("to_s", e) is Fun fun) {
				Scope s = new Scope(e);
				s.This = this;
				return fun.Run(s).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public override Boolean Equals(Object obj) {
			if (obj is Value) {
				if (this.type.AttributeExists("==") && this.type.GetAttribute("==", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
					return Converter.ToBoolean(fun.Run(s));
				}
			}

			return ReferenceEquals(obj, this);
		}

		public override Int32 GetHashCode() {
			return 5 | this.type.GetHashCode();
		}

		public Value Clone() {
			return (Value)this.MemberwiseClone();
		}
	}

	public class Object1 : IObject {
		public Value element;
		public KType type;

		public KType Type => this.type;

		public Object1(KType type) {
			this.element = Const.NULL;
			this.type = type;
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			if (index == 0) {
				return this.element;
			}

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			if (index == 0) {
				this.element = value;
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

		/// <summary> Компаратор. </summary>
		public Int32 CompareTo(Object obj) {
			if (obj is Value value) {
				if (this.type.AttributeExists("<=>") && this.type.GetAttribute("<=>", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
				//	return (Int32)Converter.ToDouble(fun.Run(s, value), null);
				}
			}

			return 0;
		}

		public String ToString(Scope e) {
			if (this.type.AttributeExists("to_s") && this.type.GetAttribute("to_s", e) is Fun fun) {
				Scope s = new Scope(e);
				s.This = this;
				return fun.Run(s).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public override Boolean Equals(Object obj) {
			if (obj is Value) {
				if (this.type.AttributeExists("==") && this.type.GetAttribute("==", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
					return Converter.ToBoolean(fun.Run(s));
				}
			}

			return ReferenceEquals(obj, this);
		}

		public override Int32 GetHashCode() {
			return this.element.GetHashCode() | this.type.GetHashCode();
		}

		public Value Clone() {
			return new Object1(this.type) {
				element = this.element
			};
		}
	}

	public class Object2 : IObject {
		/// <summary> Первое поле. </summary>
		public Value element1;
		/// <summary> Второе поле. </summary>
		public Value element2;
		/// <summary> Класс. </summary>
		public KType type;
		/// <summary> Возвращает тип объекта. </summary>
		public KType Type => this.type;

		/// <summary> Конструктор объекта. </summary> 
		/// <param name="type"> Тип. </param>
		public Object2(KType type) {
			this.element1 = Const.NULL;
			this.element2 = Const.NULL;
			this.type = type;
		}

		/// <summary> Возвращает поле объекта </summary> 
		/// <param name="name"> Имя поля </param> 
		/// <param name="mode"> Модификатор доступа </param>
		/// <param name="e"> Область видимости, из которой осуществляется доступ. </param>
		public Value Get(String name, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					return this.element1;
				case 1:
					return this.element2;
			}

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		/// <summary> Устанавливает поле объекта. </summary> 
		/// <param name="name"> Имя поля. </param> 
		/// <param name="value"> Значение. </param>
		/// <param name="mode"> Модификатор доступа. </param>
		/// <param name="e"> Область видимости, из которой осуществляется доступ. </param>
		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					this.element1 = value;
					return;
				case 1:
					this.element2 = value;
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

		/// <summary> Компаратор. </summary>
		public Int32 CompareTo(Object obj) {
			if (obj is Value value) {
				if (this.type.AttributeExists("<=>") && this.type.GetAttribute("<=>", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
//return (Int32)Converter.ToDouble(fun.Run(s, value), null);
				}
			}

			return 0;
		}

		public String ToString(Scope e) {
			if (this.type.AttributeExists("to_s") && this.type.GetAttribute("to_s", e) is Fun fun) {
				Scope s = new Scope(e);
				s.This = this;
				return fun.Run(s).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public override String ToString() {
			return this.ToString(null);
		}

		public override Boolean Equals(Object obj) {
			if (obj is Value) {
				if (this.type.AttributeExists("==") && this.type.GetAttribute("==", null) is Fun fun) {
					Scope s = new Scope(null);
					s.This = this;
					return Converter.ToBoolean(fun.Run(s));
				}
			}

			return ReferenceEquals(obj, this);
		}

		public override Int32 GetHashCode() {
			return this.element1.GetHashCode() ^ this.element2.GetHashCode() | this.type.GetHashCode();
		}

		public Value Clone() {
			return new Object2(this.type) {
				element1 = this.element1,
				element2 = this.element2
			};
		}
	}

	public class Object3 : IObject {
		public Value element1;
		public Value element2;
		public Value element3;
		public KType type;

		public KType Type => this.type;

		public Object3(KType type) {
			element1 = Const.NULL;
			element2 = Const.NULL;
			element3 = Const.NULL;
			this.type = type;
		}

		public Value Get(string name, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					return this.element1;
				case 1:
					return this.element2;
				case 2:
					return this.element3;
			}

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(string name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					this.element1 = value;
					return;
				case 1:
					this.element2 = value;
					return;
				case 2:
					this.element3 = value;
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

		public override String ToString() {
			return this.ToString(null);
		}

		public string ToString(Scope e) {
			Scope ex = new Scope(e);
			ex.Set("this", this);
			if (type.AttributeExists("to_s")) {
				return ((Fun)type.GetAttribute("to_s", e)).Run(ex).ToString(e);
			}

			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public Value Clone() {
			return new Object3(this.type) { element1 = element1, element2 = element2, element3 = element3 };
		}
	}

	public class Object4 : IObject {
		internal Value element1;
		internal Value element2;
		internal Value element3;
		internal Value element4;
		public KType type;

		public KType Type => this.type;
		public override String ToString() {
			return this.ToString(null);
		}
		public Object4(KType type) {
			element1 = Const.NULL;
			element2 = Const.NULL;
			element3 = Const.NULL;
			element4 = Const.NULL;
			this.type = type;
		}

		public Value Get(string name, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					return this.element1;
				case 1:
					return this.element2;
				case 2:
					return this.element3;
				case 3:
					return this.element4;
			}

			if (this.type.AttributeExists("get_" + name) && this.type.GetAttribute("get_" + name, e) is Fun property) {
				Scope s = new Scope(e);
				s.This = this;
				return property.Run(s);
			}

			throw new Exception($"value of type {this.type.meta.Name} not contains a field {name}", stack: e);
		}

		public void Set(string name, Value value, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

			switch (index) {
				case 0:
					this.element1 = value;
					return;
				case 1:
					this.element2 = value;
					return;
				case 2:
					this.element3 = value;
					return;
				case 3:
					this.element4 = value;
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

		public string ToString(Scope e) {
			Scope ex = new Scope(e);
			ex.Set("this", this);
			throw new Exception("невозможно преобразовать объект к типу Kernel.String", stack: e);
		}

		public Value Clone() {
			return new Object4(this.type) {
				element1 = element1,
				element2 = element2,
				element3 = element3,
				element4 = element4
			};
		}
	}

	public class Objectn : IObject {
		public Value[] value;
		public KType type;
		public override String ToString() {
			return this.ToString(null);
		}
		public KType Type => this.type;

		public Objectn(Int32 size, KType parent) {
			this.value = new Value[size];
			this.type = parent;
		}

		public Value Get(string name, AccessModifiers mode, Scope e) {
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

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
			Int32 index = Array.IndexOf(this.type.meta.Fields, name);

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