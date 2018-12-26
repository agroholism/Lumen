
using System;
using System.Collections.Generic;

namespace Lumen.Lang.Std {
	public class Expando : IObject {
		public KType type;
		public Dictionary<String, Value> Fields { get; set; }
		public List<String> Final { get; set; }
		public KType Type => type;
		public Expando Prototype => this["prototype", null] as Expando;

		public Value this[String key, Scope e] {
			get {
				return Get(key, AccessModifiers.PUBLIC, e);
			}
			set {
				Set(key, value, AccessModifiers.PUBLIC, e);
			}
		}

		public Expando(KType type = null) {
			this.Fields = new Dictionary<String, Value>() { ["prototype"] = ExpandoType.BASE };
			this.Final = new List<String>();
			this.type = type ?? StandartModule.Expando;
		}

		public Expando(Value value, KType type = null) {
			this.Fields = new Dictionary<String, Value> { ["prototype"] = value };
			this.type = type ?? StandartModule.Expando;
		}

		public Boolean IsChildOf(Expando v) {
			Expando proto = this.Prototype;

			if (proto == v) {
				return true;
			}

			while (proto != ExpandoType.BASE) {
				if (proto == v) {
					return true;
				}
				proto = proto.Prototype;
			}

			return false;
		}

		public virtual Boolean IsExists(String name) {
			return this.Fields.ContainsKey(name) || this.Prototype.IsExists(name);
		}

		public Value Get(String name, AccessModifiers mode, Scope e) {
			if (this.Fields.ContainsKey(name)) {
				return this.Fields[name];
			}

			if (this.Prototype.IsExists(name)) {
				return this.Prototype.Get(name, AccessModifiers.PUBLIC, e);
			}

			if (this.Fields.ContainsKey("get_" + name) && this.Fields["get_" + name] is Fun f) {
				Scope sc = new Scope(e);
				sc.This = this;
				return f.Run(sc);
			}

			if (this.Prototype.IsExists("get_" + name) && this.Prototype.Get("get_" + name, AccessModifiers.PUBLIC, e) is Fun property) {
				Scope sc = new Scope(e);
				sc.This = this;
				return property.Run(sc);
			}

			Scope s = new Scope(e);
			s.This = this;

			return ((Fun)Get("missing", AccessModifiers.PUBLIC, e)).Run(s, new KString(name));
		}

		public void Set(String name, Value value, AccessModifiers mode, Scope e) {
			if (IsExists("frosen?") && Converter.ToBoolean(Get("frosen?", AccessModifiers.PUBLIC, e)) == true) {
				throw new Exception("can't modifity frosen object", stack: e);
			}

			if (this.Fields.ContainsKey("set_" + name) && this.Fields["set_" + name] is Fun f) {
				Scope s = new Scope(e);
				s.This = this;
				f.Run(s, value);
			}
			else {
				this.Fields[name] = value;
			}
		}

		public String ToString(Scope e) {
			if (Fields.ContainsKey("to_s")) {
				Scope s = new Scope(e);
				s.This = this;
				return ((Fun)this.Fields["to_s"]).Run(s).ToString(e);
			}

			if (Fields["prototype"] is Expando obj) {
				if (obj.IsExists("to_s")) {
					Scope s = new Scope(e);
					s.This = this;
					return ((Fun)obj.Get("to_s", AccessModifiers.PUBLIC, e)).Run(s).ToString(e);
				}
			}

			throw new Exception("невозможно преобразовать объект к типу Kernel.Number", stack: e);
		}

		public int CompareTo(object obj) {
			if (obj is Value) {
				Scope e = new Scope(null);
				e.Set("this", this);
			//	return (int)Converter.ToDouble(((Fun)Type.Get("<=>", null)).Run(e, (Value)obj), null);
			}
			throw new Exception("notcomparable");
		}

		public Value Clone() {
			Expando res = new Expando(this.type);

			foreach (KeyValuePair<String, Value> i in this.Fields) {
				res.Fields.Add(i.Key, i.Value);
			}

			return res;
		}


		public override String ToString() {
			return this.ToString(null);
		}
	}
}
